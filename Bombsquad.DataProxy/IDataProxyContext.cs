using System;
using System.Reflection.Emit;
using Bombsquad.DataProxy.ColumnValueConverters;
using Bombsquad.DataProxy.DataReaderAdaptors;
using Bombsquad.DataProxy.DataRowReaders;
using Bombsquad.DataProxy.SimpleTypes;
using Bombsquad.DataProxy.SqlParameterAdaptors;

namespace Bombsquad.DataProxy
{
	public interface IDataProxyContext
	{
		IColumnValueConverter<T> GetOrCreateColumnValueConverter<T>();
		IDataReaderAdaptor<T> GetOrCreateDataReaderAdaptor<T>();
		IDataRowReader<T> GetOrCreateDataRowReader<T>();
		ISqlParameterAdaptor<T> GetOrCreateSqlParameterAdaptor<T>();
		ModuleBuilder GetModuleBuilder( string moduleName );
		bool TryGetSimpleTypeMapping( Type type, out ISimpleTypeDataInfo simpleTypeDataInfo );
	}
}