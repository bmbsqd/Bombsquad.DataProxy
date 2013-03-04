using System.Linq;
using System.Threading.Tasks;
using Bombsquad.DataProxy.Async;

namespace Bombsquad.DataProxy.DataReaderAdaptors
{
	public class SingleDataReaderAdaptorFactory : IDataReaderAdaptorFactory
	{
		public bool TryCreate<T>( IDataProxyContext context, out IDataReaderAdaptor<T> dataReaderAdaptor )
		{
			dataReaderAdaptor = new SingleDataReaderAdaptor<T>(context);
			return true;
		}

		private class SingleDataReaderAdaptor<T> : IDataReaderAdaptor<T>
		{
			private readonly IDataProxyContext m_context;

			public SingleDataReaderAdaptor( IDataProxyContext context )
			{
				m_context = context;
			}

			public T Read( IExecuteReaderContext connectionAndReaderContext )
			{
				return new DataReaderEnumerable<T>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<T>() ).FirstOrDefault();
			}

			public Task<T> ReadAsync( IExecuteReaderContext connectionAndReaderContext )
			{
				var enumerable = new DataReaderEnumerable<T>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<T>() );
				return enumerable.FirstOrDefaultAsync();
			}
		}
	}
}