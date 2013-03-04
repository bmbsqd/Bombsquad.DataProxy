using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Bombsquad.DataProxy.CommandInformation;
using Bombsquad.DataProxy.SqlParameterAdaptors;
using Bombsquad.DataProxy.Utils;

namespace Bombsquad.DataProxy
{
	public class DataMapperProxyClassFactory
	{
		private readonly IConnectionFactory m_connectionFactory;
		private readonly DataProxyConfiguration m_configuration;
		private readonly ConcurrentDictionary<Type, object> m_proxyClassFactoryCache;

		public DataMapperProxyClassFactory( IConnectionFactory connectionFactory, DataProxyConfiguration configuration )
		{
			m_connectionFactory = connectionFactory;
			m_configuration = configuration;
			m_proxyClassFactoryCache = new ConcurrentDictionary<Type, object>();
		}

		public DataMapperProxyClassFactory( IConnectionFactory connectionFactory )
			: this( connectionFactory, new DataProxyConfiguration() )
		{
		}

		public TDataMapperInterface GetOrCreate<TDataMapperInterface>( IConnectionFactory connectionFactory = null )
		{
			var dataMapperInstanceFactory =
				(Func<SqlExecutable, IDataProxyContext, TDataMapperInterface>) m_proxyClassFactoryCache.GetOrAdd( typeof( TDataMapperInterface ), interfaceType =>
				{
					var typeBuilder = m_configuration.GetModuleBuilder( "DataMappers" ).DefineType( typeof( TDataMapperInterface ).Name + "Implementation", TypeAttributes.Public );
					typeBuilder.AddInterfaceImplementation( typeof( TDataMapperInterface ) );

					var sqlExecutableField = typeBuilder.DefineField( "m_sqlExecutable", typeof( SqlExecutable ), FieldAttributes.Private );
					var dataProxyContextField = typeBuilder.DefineField( "m_context", typeof( IDataProxyContext ), FieldAttributes.Private );

					CreateConstructor( typeBuilder, sqlExecutableField, dataProxyContextField );

					foreach ( var interfaceMethod in typeof( TDataMapperInterface ).GetMethods() )
					{
						var parameters = interfaceMethod.GetParameters().Select( p => p.ParameterType ).ToArray();
						var method = typeBuilder.DefineMethod( interfaceMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, interfaceMethod.ReturnType, parameters );
						typeBuilder.DefineMethodOverride( method, interfaceMethod );

						var staticMethod = GenerateStaticImplementationMethod( typeBuilder, interfaceMethod );
						GenerateCallToStaticImplementationMethod( method.GetILGenerator(), sqlExecutableField, dataProxyContextField, parameters, staticMethod );
					}

					return CreateInstanceFactory<TDataMapperInterface>( typeBuilder.CreateType() );
				} );

			return dataMapperInstanceFactory( new SqlExecutable( connectionFactory ?? m_connectionFactory, m_configuration ), m_configuration );
		}

		private MethodBuilder GenerateStaticImplementationMethod( TypeBuilder typeBuilder, MethodInfo interfaceMethod )
		{
			var interfaceParameters = interfaceMethod.GetParameters();

			var parameters = new List<Type>();
			parameters.Add( typeof( SqlExecutable ) );
			parameters.Add( typeof( IDataProxyContext ) );
			parameters.AddRange( interfaceParameters.Select( ip => ip.ParameterType ) );

			var methodBuilder = typeBuilder.DefineMethod( interfaceMethod.Name + "Implementation", MethodAttributes.Public | MethodAttributes.Static,
				interfaceMethod.ReturnType, parameters.ToArray() );

			MethodInfo sqlExecutableMethod = GetSqlExecutableMethod( interfaceMethod.ReturnType );

			var parametersAsExpressions = parameters.Select( Expression.Parameter ).ToArray();

			var blockParameterExpressions = new List<ParameterExpression>();
			var blockExpressions = new List<Expression>();
			var setupCommandActionExpressions = new List<Func<Expression,Expression>>();
			var postBlockExpressions = new List<Expression>();

			ParameterExpression resultParameter = Expression.Parameter( sqlExecutableMethod.ReturnType, "result" );
			blockParameterExpressions.Add( resultParameter );

			for ( var i = 0; i < interfaceParameters.Length; i++ )
			{
				var interfaceParameter = interfaceParameters[i];
				var parameterName = Expression.Constant( interfaceParameter.Name );
				var parameterAsExpression = parametersAsExpressions[ i + 2 ];

				if ( interfaceParameter.GetCustomAttribute<IgnoredParameterAttribute>() != null )
				{
					continue;
				}

				if ( interfaceParameter.IsOut )
				{
					var parameterType = typeof( MethodOutputParameter<> ).MakeGenericType( interfaceParameter.ParameterType.GetElementType() );
					var parameterTypeConstructor = parameterType.GetConstructors().Single();
					var parameter = Expression.Parameter( parameterType );
					blockParameterExpressions.Add( parameter );
					blockExpressions.Add( Expression.Assign( parameter, Expression.New( parameterTypeConstructor, parameterName ) ) );
					postBlockExpressions.Add( Expression.Assign( parameterAsExpression, Expression.Invoke( Expression.Property( parameter, parameterType.GetProperty( "GetSqlParameterValue" ) ) ) ) );
					setupCommandActionExpressions.Add( p => GetGenerateAddSqlParameterMethodCallExpression( interfaceParameter.ParameterType.GetElementType(), "AddSqlOutputParameters", p, parameter, parametersAsExpressions[1] ) );
				}
				else
				{
					var parameterType = typeof( MethodInputParameter<> ).MakeGenericType( interfaceParameter.ParameterType );
					var parameterTypeConstructor = parameterType.GetConstructors().Single();
					var parameter = Expression.Parameter( parameterType );
					blockParameterExpressions.Add( parameter );
					blockExpressions.Add( Expression.Assign( parameter, Expression.New( parameterTypeConstructor, parameterName, parameterAsExpression ) ) );
					setupCommandActionExpressions.Add( p => GetGenerateAddSqlParameterMethodCallExpression( interfaceParameter.ParameterType, "AddSqlInputParameters", p, parameter, parametersAsExpressions[1] ) );
				}
			}

			blockExpressions.Add( Expression.Assign( resultParameter, Expression.Call( parametersAsExpressions.First(), sqlExecutableMethod, new[] { CreateSetupCommandActionExpression( interfaceMethod, setupCommandActionExpressions ) } ) ) );
			blockExpressions.AddRange( postBlockExpressions );
			blockExpressions.Add( resultParameter );

			var lambda = Expression.Lambda( Expression.Block( blockParameterExpressions, blockExpressions ), parametersAsExpressions );
			lambda.CompileToMethod( methodBuilder );

			return methodBuilder;
		}

		private Expression GetGenerateAddSqlParameterMethodCallExpression( Type type, string methodName, Expression sqlParameterCollectionParameter, Expression inputParameter, ParameterExpression dataProxyContextParameter )
		{
			var genericMethod = ReflectionHelper.GetMethodInfo<IDataProxyContext>( r => r.GetOrCreateSqlParameterAdaptor<int>() ).GetGenericMethodDefinition().MakeGenericMethod(type);
			var sqlParameterAdaptorRegistryParameter = Expression.Call( dataProxyContextParameter, genericMethod );
			var method = typeof( ISqlParameterAdaptor<> ).MakeGenericType( type ).GetMethod( methodName, BindingFlags.Public | BindingFlags.Instance );
			return Expression.Call( sqlParameterAdaptorRegistryParameter, method, sqlParameterCollectionParameter, inputParameter );
		}

		private Expression CreateSetupCommandActionExpression( MethodInfo method, IEnumerable<Func<Expression, Expression>> setupCommandActionExpressions )
		{
			ICommandInformation commandInformation;
			m_configuration.CommandInformationProvider.TryGetCommandInformation( method, out commandInformation );

			var cmdParameter = Expression.Parameter( typeof( SqlCommand ), "cmd" );
			
			var expressions = new List<Expression>
			{
				Expression.Assign( Expression.Property( cmdParameter, ReflectionHelper.GetProperty<SqlCommand, string>( c => c.CommandText ) ), Expression.Constant( commandInformation.CommandText ) ),
				Expression.Assign( Expression.Property( cmdParameter, ReflectionHelper.GetProperty<SqlCommand, CommandType>( c => c.CommandType ) ), Expression.Constant( commandInformation.CommandType ) )
			};

			var sqlParameterCollectionPropertyExpression = Expression.Property( cmdParameter, ReflectionHelper.GetProperty<SqlCommand, SqlParameterCollection>( c => c.Parameters ) );
			expressions.AddRange( setupCommandActionExpressions.Select( e => e( sqlParameterCollectionPropertyExpression ) ) );

			return Expression.Lambda<Action<SqlCommand>>( Expression.Block( expressions ), cmdParameter );
		}

		private static MethodInfo GetSqlExecutableMethod( Type returnType )
		{
			if ( returnType == typeof( void ) )
			{
				return ReflectionHelper.GetMethodInfo<SqlExecutable>( e => e.ExecuteNonQuery( null ) );
			}

			if ( returnType == typeof( Task ) )
			{
				return ReflectionHelper.GetMethodInfo<SqlExecutable>( e => e.ExecuteNonQueryAsync( null ) );
			}

			if ( returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof( Task<> ) )
			{
				return ReflectionHelper.GetMethodInfo<SqlExecutable>( e => e.ExecuteReaderAsync<string>( null ) ).GetGenericMethodDefinition().MakeGenericMethod( returnType.GetGenericArguments().Single() );
			}

			return ReflectionHelper.GetMethodInfo<SqlExecutable>( e => e.ExecuteReader<string>( null ) ).GetGenericMethodDefinition().MakeGenericMethod( returnType );
		}

		private static void GenerateCallToStaticImplementationMethod( ILGenerator ilGenerator, FieldBuilder sqlExecutableField, FieldBuilder dataProxyContextField, Type[] parameters, MethodInfo staticMethod )
		{
			ilGenerator.Emit( OpCodes.Ldarg_0 );
			ilGenerator.Emit( OpCodes.Ldfld, sqlExecutableField );

			ilGenerator.Emit( OpCodes.Ldarg_0 );
			ilGenerator.Emit( OpCodes.Ldfld, dataProxyContextField );

			for ( var i = 0; i < parameters.Length; i++ )
			{
				ilGenerator.Emit( OpCodes.Ldarg, i + 1 );
			}

			ilGenerator.Emit( OpCodes.Call, staticMethod );
			ilGenerator.Emit( OpCodes.Ret );
		}

		private static Func<SqlExecutable, IDataProxyContext, TDataMapperInterface> CreateInstanceFactory<TDataMapperInterface>( Type type )
		{
			var sqlExecutableParameter = Expression.Parameter( typeof( SqlExecutable ) );
			var dataProxyContextParameter = Expression.Parameter( typeof( IDataProxyContext ) );
			return Expression.Lambda<Func<SqlExecutable, IDataProxyContext, TDataMapperInterface>>( Expression.New( type.GetConstructors().Single(), sqlExecutableParameter, dataProxyContextParameter ), sqlExecutableParameter, dataProxyContextParameter ).Compile();
		}

		private static void CreateConstructor( TypeBuilder typeBuilder, FieldBuilder sqlExecutableField, FieldBuilder dataProxyContextField )
		{
			var constructorSignature = new[]
			{
				typeof( SqlExecutable ),
				typeof( IDataProxyContext )
			};
			var constructor = typeBuilder.DefineConstructor( MethodAttributes.Public, CallingConventions.Standard, constructorSignature );
			var ilGenerator = constructor.GetILGenerator();

			ilGenerator.Emit( OpCodes.Ldarg_0 );
			ilGenerator.Emit( OpCodes.Ldarg_1 );
			ilGenerator.Emit( OpCodes.Stfld, sqlExecutableField );

			ilGenerator.Emit( OpCodes.Ldarg_0 );
			ilGenerator.Emit( OpCodes.Ldarg_2 );
			ilGenerator.Emit( OpCodes.Stfld, dataProxyContextField );

			ilGenerator.Emit( OpCodes.Ret );
		}
	}
}