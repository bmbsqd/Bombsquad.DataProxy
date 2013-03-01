using System;
using System.Reflection.Emit;
using Bombsquad.DataProxy2.ColumnValueConverters;
using Bombsquad.DataProxy2.DataReaderAdaptors;
using Bombsquad.DataProxy2.DataRowReaders;
using Bombsquad.DataProxy2.SimpleTypes;
using Bombsquad.DataProxy2.SqlParameterAdaptors;

namespace Bombsquad.DataProxy2
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