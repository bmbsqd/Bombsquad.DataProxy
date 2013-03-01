using System;
using System.Linq;
using System.Threading.Tasks;
using Bombsquad.DataProxy2.Async;

namespace Bombsquad.DataProxy2.DataReaderAdaptors
{
	public class AsyncEnumerableDataReaderAdaptorFactory : BaseEnumerableDataReaderAdaptorFactory
	{
		protected override bool TryGetElementType( Type type, out Type elementType )
		{
			if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( IAsyncEnumerable<> ) )
			{
				elementType = type.GetGenericArguments().Single();
				return true;
			}

			elementType = null;
			return false;
		}

		protected override IDataReaderAdaptor<T> CreateDataReaderAdaptor<T, TElement>(IDataProxyContext dataProxyContext)
		{
			return (IDataReaderAdaptor<T>)new AsyncEnumerableDataReaderAdaptor<TElement>(dataProxyContext);
		}

		private class AsyncEnumerableDataReaderAdaptor<TElement> : IDataReaderAdaptor<IAsyncEnumerable<TElement>>
		{
			private readonly IDataProxyContext m_context;

			public AsyncEnumerableDataReaderAdaptor( IDataProxyContext context )
			{
				m_context = context;
			}

			public IAsyncEnumerable<TElement> Read( IExecuteReaderContext connectionAndReaderContext )
			{
				return new DataReaderEnumerable<TElement>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<TElement>() );
			}

			public async Task<IAsyncEnumerable<TElement>> ReadAsync( IExecuteReaderContext connectionAndReaderContext )
			{
				// This only occurs if the user writes a method with return type: Task<IAsyncEnumerable<T>>, and that makes no sense since IAsyncEnumerable allready is async...
				return new DataReaderEnumerable<TElement>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<TElement>() );
			}
		}
	}
}