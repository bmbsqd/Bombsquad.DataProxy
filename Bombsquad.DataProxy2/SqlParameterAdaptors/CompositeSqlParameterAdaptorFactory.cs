namespace Bombsquad.DataProxy2.SqlParameterAdaptors
{
	public class CompositeSqlParameterAdaptorFactory : ISqlParameterAdaptorFactory
	{
		private readonly ISqlParameterAdaptorFactory[] m_factories;

		public CompositeSqlParameterAdaptorFactory(params ISqlParameterAdaptorFactory[] factories )
		{
			m_factories = factories;
		}

		public bool TryCreate<T>( IDataProxyContext context, out ISqlParameterAdaptor<T> sqlParameterAdaptor )
		{
			foreach ( var factory in m_factories )
			{
				if(factory.TryCreate( context, out sqlParameterAdaptor ))
				{
					return true;
				}
			}

			sqlParameterAdaptor = null;
			return false;
		}
	}
}