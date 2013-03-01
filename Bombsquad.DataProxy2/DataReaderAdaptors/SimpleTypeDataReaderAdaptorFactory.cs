using System.Threading.Tasks;
using Bombsquad.DataProxy2.ColumnValueConverters;

namespace Bombsquad.DataProxy2.DataReaderAdaptors
{
	public class SimpleTypeDataReaderAdaptorFactory : IDataReaderAdaptorFactory
	{
		public bool TryCreate<T>( IDataProxyContext context, out IDataReaderAdaptor<T> dataReaderAdaptor )
		{
			var columnValueConverter = context.GetOrCreateColumnValueConverter<T>();
			if ( columnValueConverter != null )
			{
				dataReaderAdaptor = new SimpleTypeDataReaderAdaptor<T>( columnValueConverter );
				return true;
			}

			dataReaderAdaptor = null;
			return false;
		}

		private class SimpleTypeDataReaderAdaptor<T> : IDataReaderAdaptor<T>
		{
			private readonly IColumnValueConverter<T> m_columnValueConverter;

			public SimpleTypeDataReaderAdaptor( IColumnValueConverter<T> columnValueConverter )
			{
				m_columnValueConverter = columnValueConverter;
			}

			public T Read( IExecuteReaderContext connectionAndReaderContext )
			{
				connectionAndReaderContext.OpenConnectionAndExecuteReader();

				using ( connectionAndReaderContext )
				{
					if ( !connectionAndReaderContext.Reader.Read() )
					{
						return default(T);
					}
					return m_columnValueConverter.Read( connectionAndReaderContext.Reader, 0 );
				}
			}

			public async Task<T> ReadAsync( IExecuteReaderContext connectionAndReaderContext )
			{
				await connectionAndReaderContext.OpenConnectionAndExecuteReaderAsync();

				using ( connectionAndReaderContext )
				{
					if ( !await connectionAndReaderContext.Reader.ReadAsync() )
					{
						return default(T);
					}
					return await m_columnValueConverter.ReadAsync( connectionAndReaderContext.Reader, 0 );
				}
			}
		}
	}
}