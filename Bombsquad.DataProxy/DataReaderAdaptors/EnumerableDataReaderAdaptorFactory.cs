using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy.DataReaderAdaptors
{
	public class EnumerableDataReaderAdaptorFactory : BaseEnumerableDataReaderAdaptorFactory
	{
		protected override bool TryGetElementType( Type type, out Type elementType )
		{
			if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<> ))
			{
				elementType = type.GetGenericArguments().Single();
				return true;
			}

			elementType = null;
			return false;
		}

		protected override IDataReaderAdaptor<T> CreateDataReaderAdaptor<T, TElement>(IDataProxyContext context)
		{
			return (IDataReaderAdaptor<T>) new EnumerableDataReaderAdaptor<TElement>(context);
		}
		
		private class EnumerableDataReaderAdaptor<TElement> : IDataReaderAdaptor<IEnumerable<TElement>>
		{
			private readonly IDataProxyContext m_context;

			public EnumerableDataReaderAdaptor( IDataProxyContext context )
			{
				m_context = context;
			}

			public IEnumerable<TElement> Read( IExecuteReaderContext connectionAndReaderContext )
			{
				return new DataReaderEnumerable<TElement>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<TElement>() );
			}

			public async Task<IEnumerable<TElement>> ReadAsync( IExecuteReaderContext connectionAndReaderContext )
			{
				// TODO: Don't know what to do here. Cant do yield return and await...
				// Either we run everything async and buffer the data in memory as an array,
				// or we do the connecting asynchronously and the data fetching synchronously.

				await connectionAndReaderContext.OpenConnectionAndExecuteReaderAsync();
				return new DataReaderEnumerable<TElement>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<TElement>() );
			}
		}
	}
}