using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bombsquad.DataProxy.Async;

namespace Bombsquad.DataProxy.DataReaderAdaptors
{
	public class ListTypesDataReaderAdaptorFactory : BaseEnumerableDataReaderAdaptorFactory
	{
		private static readonly HashSet<Type> SupportedTypes = new HashSet<Type>
		{
			typeof( IReadOnlyCollection<> ),
			typeof( IList<> ),
			typeof( ICollection<> ),
			typeof( IReadOnlyList<> )
		};

		protected override bool TryGetElementType( Type type, out Type elementType )
		{
			if ( type.IsGenericType && SupportedTypes.Contains( type.GetGenericTypeDefinition() ) )
			{
				elementType = type.GetGenericArguments().Single();
				return true;
			}

			elementType = null;
			return false;
		}

		protected override IDataReaderAdaptor<T> CreateDataReaderAdaptor<T, TElement>( IDataProxyContext context )
		{
			return new ReadOnlyCollectionDataReaderAdaptor<TElement, T>( context );
		}

		private class ReadOnlyCollectionDataReaderAdaptor<TElement, T> : IDataReaderAdaptor<T>
		{
			private readonly IDataProxyContext m_context;

			public ReadOnlyCollectionDataReaderAdaptor( IDataProxyContext context )
			{
				m_context = context;
			}

			public T Read( IExecuteReaderContext connectionAndReaderContext )
			{
				var list = new DataReaderEnumerable<TElement>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<TElement>() ).ToList();
				return (T) (object) list; // TODO: Find better solution...
			}

			public async Task<T> ReadAsync( IExecuteReaderContext connectionAndReaderContext )
			{
				await connectionAndReaderContext.OpenConnectionAndExecuteReaderAsync();
				var list = await new DataReaderEnumerable<TElement>( m_context, connectionAndReaderContext, m_context.GetOrCreateDataRowReader<TElement>() ).ToListAsync();
				return (T) (object) list; // TODO: Find better solution...
			}
		}
	}
}