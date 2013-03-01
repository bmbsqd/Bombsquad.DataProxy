namespace Bombsquad.DataProxy2.DataRowReaders
{
	public class CompositeDataRowReaderFactory : IDataRowReaderFactory
	{
		private readonly IDataRowReaderFactory[] m_factories;

		public CompositeDataRowReaderFactory(params IDataRowReaderFactory[] factories)
		{
			m_factories = factories;
		}

		public bool TryCreate<T>( IDataProxyContext contex, out IDataRowReader<T> dataRowReader )
		{
			foreach ( var factory in m_factories )
			{
				if(factory.TryCreate( contex, out dataRowReader ))
				{
					return true;
				}
			}

			dataRowReader = null;
			return false;
		}
	}
}