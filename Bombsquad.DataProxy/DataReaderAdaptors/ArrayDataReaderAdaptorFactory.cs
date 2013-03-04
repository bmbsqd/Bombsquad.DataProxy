using System;
using System.Linq;
using System.Threading.Tasks;
using Bombsquad.DataProxy.Async;

namespace Bombsquad.DataProxy.DataReaderAdaptors
{
	public class ArrayDataReaderAdaptorFactory : BaseEnumerableDataReaderAdaptorFactory
	{
		protected override bool TryGetElementType( Type type, out Type elementType )
		{
			if(type.IsArray)
			{
				elementType = type.GetElementType();
				return true;
			}

			elementType = null;
			return false;
		}

		protected override IDataReaderAdaptor<T> CreateDataReaderAdaptor<T, TElement>(IDataProxyContext context)
		{
			return (IDataReaderAdaptor<T>) new ArrayDataReaderAdaptor<TElement>(context);
		}

		private class ArrayDataReaderAdaptor<TElement> : IDataReaderAdaptor<TElement[]>
		{
			private readonly IDataProxyContext m_context;

			public ArrayDataReaderAdaptor( IDataProxyContext context )
			{
				m_context = context;
			}

			public TElement[] Read( IExecuteReaderContext connectionAndReaderContext )
			{
				return new DataReaderEnumerable<TElement>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<TElement>() ).ToArray();
			}

			public Task<TElement[]> ReadAsync( IExecuteReaderContext connectionAndReaderContext )
			{
				var enumerable = new DataReaderEnumerable<TElement>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<TElement>() );
				return enumerable.ToArrayAsync();
			}
		}
	}
}