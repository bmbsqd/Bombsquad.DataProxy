using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bombsquad.DataProxy.DataReaderAdaptors
{
	public abstract class BaseEnumerableDataReaderAdaptorFactory : IDataReaderAdaptorFactory
	{
		public bool TryCreate<T>( IDataProxyContext context, out IDataReaderAdaptor<T> dataReaderAdaptor )
		{
			Type elementType;
			if ( TryGetElementType( typeof(T), out elementType ))
			{
				var createDataReaderAdaptorMethod =  GetType().GetMethod( "CreateDataReaderAdaptor", BindingFlags.NonPublic | BindingFlags.Instance ).MakeGenericMethod( typeof( T ), elementType );
				dataReaderAdaptor = (IDataReaderAdaptor<T>)createDataReaderAdaptorMethod.Invoke( this, new object[] { context } );
				return true;
			}

			dataReaderAdaptor = null;
			return false;
		}

		protected abstract bool TryGetElementType(  Type type, out Type elementType );
		protected abstract IDataReaderAdaptor<T> CreateDataReaderAdaptor<T, TElement>(IDataProxyContext context) where T : IEnumerable<TElement>;

	}
}