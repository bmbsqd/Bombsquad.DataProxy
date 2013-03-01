using System.Threading.Tasks;
using Bombsquad.DataProxy2.ColumnValueConverters;

namespace Bombsquad.DataProxy2.DataRowReaders
{
	public class SimpleTypeDataRowReaderFactory : IDataRowReaderFactory
	{
		public bool TryCreate<T>( IDataProxyContext contex, out IDataRowReader<T> dataRowReader )
		{
			var converter = contex.GetOrCreateColumnValueConverter<T>();
			if ( converter != null )
			{
				dataRowReader = new SimpleTypeDataRowReader<T>( converter );
				return true;
			}

			dataRowReader = null;
			return false;
		}

		private class SimpleTypeDataRowReader<T> : IDataRowReader<T>
		{
			private readonly IColumnValueConverter<T> m_columnValueConverter;

			public SimpleTypeDataRowReader( IColumnValueConverter<T> columnValueConverter )
			{
				m_columnValueConverter = columnValueConverter;
			}

			public T Read( IDataReaderContext context )
			{
				return m_columnValueConverter.Read( context.DataReader, 0 );
			}

			public Task<T> ReadAsync( IDataReaderContext context )
			{
				return m_columnValueConverter.ReadAsync( context.DataReader, 0 );
			}
		}
	}
}