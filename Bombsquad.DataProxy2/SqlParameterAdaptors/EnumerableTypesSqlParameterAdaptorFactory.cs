using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Bombsquad.DataProxy2.SimpleTypes;
using Bombsquad.DataProxy2.Utils;
using Microsoft.SqlServer.Server;

namespace Bombsquad.DataProxy2.SqlParameterAdaptors
{
	public class EnumerableTypesSqlParameterAdaptorFactory : ISqlParameterAdaptorFactory
	{
		public bool TryCreate<T>( IDataProxyContext context, out ISqlParameterAdaptor<T> sqlParameterAdaptor )
		{
			Type enumerableElementType;
			if ( !ReflectionHelper.TryGetIEnumerableElementType( typeof( T ), out enumerableElementType ) )
			{
				sqlParameterAdaptor = null;
				return false;
			}

			var typeBuilder = context.GetModuleBuilder( "SqlParameterAdaptors" ).DefineType( enumerableElementType.Name + "EnumerableSqlParameterAdaptor-" + Guid.NewGuid().ToString("n"),
				TypeAttributes.Public, typeof( EnumerableTypesSqlParameterAdaptorBase<,> ).MakeGenericType( typeof( T ), enumerableElementType ) );

			GenerateGetColumnDefinitionsMethod( typeBuilder, enumerableElementType, context );

			var type = typeBuilder.CreateType();
			var factory = Expression.Lambda<Func<ISqlParameterAdaptor<T>>>( Expression.New( type.GetConstructors().Single() ) ).Compile();
			sqlParameterAdaptor = factory();
			return true;
		}

		private void GenerateGetColumnDefinitionsMethod( TypeBuilder typeBuilder, Type enumerableElementType, IDataProxyContext context )
		{
			var complexColumnDefinitionInterfaceType = typeof( IComplexColumnDefinition<> ).MakeGenericType( enumerableElementType );
			var returnType = complexColumnDefinitionInterfaceType.MakeArrayType();

			var methodBuilder = typeBuilder.DefineMethod( "GetColumnDefinitions", MethodAttributes.Family | MethodAttributes.Virtual, returnType, new Type[0] );
			typeBuilder.DefineMethodOverride( methodBuilder, typeBuilder.BaseType.GetMethod( "GetColumnDefinitions", BindingFlags.NonPublic | BindingFlags.Instance ) );
			var staticMethodBuilder = typeBuilder.DefineMethod( "GetColumnDefinitionsImplementation", MethodAttributes.Private | MethodAttributes.Static, returnType,
				new Type[0] );

			GenerateGetColumnDefinitionsMethodImplementation( complexColumnDefinitionInterfaceType, enumerableElementType, context ).CompileToMethod( staticMethodBuilder );

			var ilGenerator = methodBuilder.GetILGenerator();
			ilGenerator.Emit( OpCodes.Call, staticMethodBuilder );
			ilGenerator.Emit( OpCodes.Ret );
		}

		private LambdaExpression GenerateGetColumnDefinitionsMethodImplementation( Type complexColumnDefinitionInterfaceType, Type enumerableElementType, IDataProxyContext context )
		{
			var complexColumnDefinitionGenericType = typeof( ComplexColumnDefinition<,> );
			var expressions = new List<Expression>();

			ISimpleTypeDataInfo simpleTypeDataInfo;
			if ( context.TryGetSimpleTypeMapping( enumerableElementType, out simpleTypeDataInfo ) )
			{
				var constructor = complexColumnDefinitionGenericType.MakeGenericType( enumerableElementType, enumerableElementType ).GetConstructors().Single();
				var parameter = Expression.Parameter( enumerableElementType );
				expressions.Add( CreateComplexColumnDefinitionInitializationExpression( constructor, "Value", simpleTypeDataInfo,
					Expression.Lambda( parameter, parameter ) ) );
			}
			else
			{
				foreach ( var property in enumerableElementType.GetProperties( BindingFlags.Public | BindingFlags.Instance ) )
				{
					if ( context.TryGetSimpleTypeMapping( property.PropertyType, out simpleTypeDataInfo ) )
					{
						var constructor = complexColumnDefinitionGenericType.MakeGenericType( enumerableElementType, property.PropertyType ).GetConstructors().Single();
						var parameter = Expression.Parameter( enumerableElementType );
						expressions.Add( CreateComplexColumnDefinitionInitializationExpression( constructor, property.Name, simpleTypeDataInfo,
							Expression.Lambda( Expression.Property( parameter, property ), parameter ) ) );
					}
				}
			}

			return Expression.Lambda( Expression.NewArrayInit( complexColumnDefinitionInterfaceType, expressions ) );
		}

		private static NewExpression CreateComplexColumnDefinitionInitializationExpression( ConstructorInfo constructor, string columnName,
			ISimpleTypeDataInfo simpleTypeDataInfo, LambdaExpression valueExtractorExpression )
		{
			return Expression.New( constructor, Expression.Constant( columnName ), Expression.Constant( simpleTypeDataInfo.SqlDbType ),
				Expression.Constant( simpleTypeDataInfo.VariableLength ), simpleTypeDataInfo.SetSqlDataRecordValueExpression, valueExtractorExpression );
		}

		public abstract class EnumerableTypesSqlParameterAdaptorBase<T, TElement> : ISqlParameterAdaptor<T> where T : IEnumerable<TElement>
		{
			public void AddSqlInputParameters( SqlParameterCollection parameters, MethodInputParameter<T> inputParameter )
			{
				var parameter = parameters.Add( inputParameter.ParameterName, SqlDbType.Structured );
				parameter.Value = (inputParameter.ParameterValue == null || !inputParameter.ParameterValue.Any()) ? null : GetRecords( inputParameter.ParameterValue );
			}

			private IEnumerable<SqlDataRecord> GetRecords( IEnumerable<TElement> values )
			{
				var columnDefinitions = GetColumnDefinitions();

				var record = new SqlDataRecord( columnDefinitions.Select( cd => cd.CreateMetadata() ).ToArray() );

				foreach ( var value in values.Where( v => v != null ) )
				{
					for ( var i = 0; i < columnDefinitions.Length; i++ )
					{
						columnDefinitions[i].SetValue( record, i, value );
					}

					yield return record;
				}
			}

			protected abstract IComplexColumnDefinition<TElement>[] GetColumnDefinitions();

			public void AddSqlOutputParameters( SqlParameterCollection parameters, MethodOutputParameter<T> outputParameter )
			{
				throw new InvalidOperationException( "Cannot use type " + typeof( T ).FullName + " as output parameter." );
			}
		}

		public interface IComplexColumnDefinition<T>
		{
			SqlMetaData CreateMetadata();
			void SetValue( SqlDataRecord record, int ordinal, T value );
		}

		public class ComplexColumnDefinition<T, TValue> : IComplexColumnDefinition<T>
		{
			public ComplexColumnDefinition( string columnName, SqlDbType sqlDbType, bool variableLength, Action<SqlDataRecord, int, TValue> setter, Func<T, TValue> valueExtractor )
			{
				ColumnName = columnName;
				SqlDbType = sqlDbType;
				Setter = setter;
				VariableLength = variableLength;
				ValueExtractor = valueExtractor;
			}

			public string ColumnName { get; private set; }
			public SqlDbType SqlDbType { get; private set; }
			public bool VariableLength { get; private set; }
			public Func<T, TValue> ValueExtractor { get; private set; }
			public Action<SqlDataRecord, int, TValue> Setter { get; private set; }

			public SqlMetaData CreateMetadata()
			{
				return VariableLength ? new SqlMetaData( ColumnName, SqlDbType, SqlMetaData.Max ) : new SqlMetaData( ColumnName, SqlDbType );
			}

			public void SetValue( SqlDataRecord record, int ordinal, T value )
			{
				Setter( record, ordinal, ValueExtractor( value ) );
			}
		}
	}
}