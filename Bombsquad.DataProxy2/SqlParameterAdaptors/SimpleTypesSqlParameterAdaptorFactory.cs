using System;
using System.Data;
using System.Data.SqlClient;
using Bombsquad.DataProxy2.SimpleTypes;

namespace Bombsquad.DataProxy2.SqlParameterAdaptors
{
	public class SimpleTypesSqlParameterAdaptorFactory : ISqlParameterAdaptorFactory
	{
		public bool TryCreate<T>( IDataProxyContext context, out ISqlParameterAdaptor<T> sqlParameterAdaptor )
		{
			var type = typeof( T ).IsEnum ? typeof( T ).GetEnumUnderlyingType() : typeof( T );

			ISimpleTypeDataInfo simpleTypeDataInfo;
			if ( context.TryGetSimpleTypeMapping( type, out simpleTypeDataInfo ) )
			{
				sqlParameterAdaptor = new SimpleTypesSqlParameterAdaptor<T>( simpleTypeDataInfo );
				return true;
			}

			sqlParameterAdaptor = null;
			return false;
		}

		private class SimpleTypesSqlParameterAdaptor<T> : ISqlParameterAdaptor<T>
		{
			private readonly ISimpleTypeDataInfo m_simpleTypeDataInfo;

			public SimpleTypesSqlParameterAdaptor( ISimpleTypeDataInfo simpleTypeDataInfo )
			{
				m_simpleTypeDataInfo = simpleTypeDataInfo;
			}

			public void AddSqlInputParameters( SqlParameterCollection parameters, MethodInputParameter<T> inputParameter )
			{
				var parameter = parameters.Add( inputParameter.ParameterName, m_simpleTypeDataInfo.SqlDbType );
				parameter.Direction = ParameterDirection.Input;
				parameter.Value = inputParameter.UntypedParameterValue ?? DBNull.Value;
			}

			public void AddSqlOutputParameters( SqlParameterCollection parameters, MethodOutputParameter<T> outputParameter )
			{
				var parameter = parameters.Add( outputParameter.ParameterName, m_simpleTypeDataInfo.SqlDbType );
				parameter.Direction = ParameterDirection.Output;
				outputParameter.GetSqlParameterValue = () => (T) parameter.Value;
			}
		}
	}
}