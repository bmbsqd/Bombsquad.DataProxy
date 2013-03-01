namespace Bombsquad.DataProxy2.DataReaderAdaptors
{
	public class CompositeDataReaderAdaptorFactory : IDataReaderAdaptorFactory
	{
		private readonly IDataReaderAdaptorFactory[] m_factories;

		public CompositeDataReaderAdaptorFactory( params IDataReaderAdaptorFactory[] factories )
		{
			m_factories = factories;
		}

		public bool TryCreate<T>( IDataProxyContext context, out IDataReaderAdaptor<T> dataReaderAdaptor )
		{
			foreach ( var factory in m_factories )
			{
				if ( factory.TryCreate( context, out dataReaderAdaptor ) )
				{
					return true;
				}
			}
			dataReaderAdaptor = null;
			return false;
		}
	}
}