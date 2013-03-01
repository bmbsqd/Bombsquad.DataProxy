using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy2.DataRowReaders
{
	public class ComplexTypeDataRowReaderFactory : IDataRowReaderFactory
	{
		public virtual bool TryCreate<T>( IDataProxyContext contex, out IDataRowReader<T> dataRowReader )
		{
			var baseType = typeof( BaseComplexTypeDataRowReader<> ).MakeGenericType( typeof( T ) );
			var baseTypeConstructor = baseType.GetConstructor( BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof( IEnumerable<IColumnValueExtractor<T>> ) }, null );

			var typeBuilder = contex.GetModuleBuilder( "DataRowReaders" ).DefineType( typeof( T ).Name + "DataRowReader", TypeAttributes.Public, baseType );

			var method = ImplementGetColumnValueExtractorsMethod<T>( typeBuilder );

			var ctor = typeBuilder.DefineConstructor( MethodAttributes.Public, CallingConventions.Standard, new Type[0] );
			var il = ctor.GetILGenerator();

			var result = il.DeclareLocal( method.ReturnType );
			il.Emit( OpCodes.Call, method );
			il.Emit( OpCodes.Stloc, result );
			il.Emit( OpCodes.Ldarg_0 );
			il.Emit( OpCodes.Ldloc, result );
			il.Emit( OpCodes.Call, baseTypeConstructor );
			il.Emit( OpCodes.Ret );

			var type = typeBuilder.CreateType();
			var factory = Expression.Lambda<Func<IDataRowReader<T>>>( Expression.New( type.GetConstructors().Single() ) ).Compile();
			dataRowReader = factory();
			return true;
		}

		private static MethodBuilder ImplementGetColumnValueExtractorsMethod<T>( TypeBuilder typeBuilder )
		{
			var methodBuilder = typeBuilder.DefineMethod( "GetColumnValueExtractors", MethodAttributes.Static | MethodAttributes.Family,
				typeof( IEnumerable<IColumnValueExtractor<T>> ), new Type[0] );

			var expressions = new List<Expression>();

			foreach ( var propertyInfo in typeof( T ).GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
			{
				var method = typeof( ComplexTypeDataRowReaderFactory ).GetMethod( "GenerateColumnValueExtractorInitializationExpression",
					BindingFlags.NonPublic | BindingFlags.Static );
				var genericMethod = method.MakeGenericMethod( typeof( T ), propertyInfo.PropertyType );
				expressions.Add( (Expression) genericMethod.Invoke( null, new object[]
				{
					propertyInfo
				} ) );
			}

			var lamda = Expression.Lambda( Expression.NewArrayInit( typeof( IColumnValueExtractor<T> ), expressions ) );
			lamda.CompileToMethod( methodBuilder );
			return methodBuilder;
		}

		private static Expression GenerateColumnValueExtractorInitializationExpression<T, TColumn>( PropertyInfo propertyInfo )
		{
			var constructor = typeof( ColumnValueExtractor<T, TColumn> ).GetConstructors().Single();
			var instanceParameter = Expression.Parameter( typeof( T ), "instance" );
			var valueParameter = Expression.Parameter( typeof( TColumn ), "value" );
			var assignPropertyExpression =
				Expression.Lambda<Action<T, TColumn>>( Expression.Assign( Expression.Property( instanceParameter, propertyInfo ), valueParameter ), instanceParameter,
					valueParameter );
			return Expression.New( constructor, Expression.Constant( propertyInfo.Name ), assignPropertyExpression );
		}

		public abstract class BaseComplexTypeDataRowReader<T> : IDataRowReader<T> where T : new()
		{
			private readonly Dictionary<string, IColumnValueExtractor<T>> m_valueExtractors;

			protected BaseComplexTypeDataRowReader( IEnumerable<IColumnValueExtractor<T>> valueExtractors )
			{
				m_valueExtractors = valueExtractors.ToDictionary( v => v.ColumnName, StringComparer.InvariantCultureIgnoreCase );
			}

			public T Read( IDataReaderContext context )
			{
				var instance = new T();

				foreach ( var ordinalMapping in context.OrdinalMap )
				{
					IColumnValueExtractor<T> valueExtractor;
					if ( m_valueExtractors.TryGetValue( ordinalMapping.Value, out valueExtractor ) )
					{
						valueExtractor.SetValue( context, instance, ordinalMapping.Key );
					}
				}

				return instance;
			}

			public async Task<T> ReadAsync( IDataReaderContext context )
			{
				var instance = new T();

				foreach ( var ordinalMapping in context.OrdinalMap )
				{
					IColumnValueExtractor<T> valueExtractor;
					if ( m_valueExtractors.TryGetValue( ordinalMapping.Value, out valueExtractor ) )
					{
						await valueExtractor.SetValueAsync( context, instance, ordinalMapping.Key );
					}
				}

				return instance;
			}
		}

		public interface IColumnValueExtractor<T>
		{
			string ColumnName { get; }
			void SetValue( IDataReaderContext context, T instance, int ordinal );
			Task SetValueAsync( IDataReaderContext context, T instance, int ordinal );
		}

		public class ColumnValueExtractor<T, TColumn> : IColumnValueExtractor<T>
		{
			private readonly string m_columnName;
			private readonly Action<T, TColumn> m_propertySetter;

			public ColumnValueExtractor( string columnName, Action<T, TColumn> propertySetter )
			{
				m_columnName = columnName;
				m_propertySetter = propertySetter;
			}

			public string ColumnName
			{
				get
				{
					return m_columnName;
				}
			}

			public void SetValue( IDataReaderContext context, T instance, int ordinal )
			{
				var value = context.GetValue<TColumn>( ordinal );
				m_propertySetter( instance, value );
			}

			public async Task SetValueAsync( IDataReaderContext context, T instance, int ordinal )
			{
				var value = await context.GetValueAsync<TColumn>( ordinal );
				m_propertySetter( instance, value );
			}
		}
	}
}