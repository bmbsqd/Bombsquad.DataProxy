namespace Bombsquad.DataProxy.ColumnValueConverters
{
	public class CompositeColumnValueConverterFactory : IColumnValueConverterFactory
	{
		private readonly IColumnValueConverterFactory[] m_factories;

		public CompositeColumnValueConverterFactory(params IColumnValueConverterFactory[] factories)
		{
			m_factories = factories;
		}

		public bool TryCreate<T>( IDataProxyContext context, out IColumnValueConverter<T> converter )
		{
			foreach ( var factory in m_factories )
			{
				if(factory.TryCreate( context, out converter ))
				{
					return true;
				}
			}

			converter = null;
			return false;
		}
	}
}