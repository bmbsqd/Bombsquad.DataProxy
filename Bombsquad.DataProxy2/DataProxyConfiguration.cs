using System;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using Bombsquad.DataProxy2.ColumnValueConverters;
using Bombsquad.DataProxy2.CommandInformation;
using Bombsquad.DataProxy2.DataReaderAdaptors;
using Bombsquad.DataProxy2.DataRowReaders;
using Bombsquad.DataProxy2.SimpleTypes;
using Bombsquad.DataProxy2.SqlParameterAdaptors;

namespace Bombsquad.DataProxy2
{
	public class DataProxyConfiguration : IDataProxyContext
	{
		private readonly ConcurrentDictionary<Type, object> m_columnConverterCache;
		private readonly ConcurrentDictionary<Type, object> m_dataReaderAdaptorCache;
		private readonly ConcurrentDictionary<Type, object> m_dataRowReaderCache;
		private readonly ConcurrentDictionary<Type, object> m_sqlParameterAdaptorCache;
		private readonly DynamicAssemblyModuleRegistry m_dynamicAssemblyModuleRegistry;

		public DataProxyConfiguration()
		{
			SqlParameterAdaptorFactory = new CompositeSqlParameterAdaptorFactory(
				new SimpleTypesSqlParameterAdaptorFactory(), 
				new EnumerableTypesSqlParameterAdaptorFactory());
			
			CommandInformationProvider = new CompositeCommandInformationProvider(
				new QueryAttributeCommandInformationProvider(),
				new MethodNameAsStoredProcedureCommandInformationProvider());

			DataReaderAdaptorFactory = new CompositeDataReaderAdaptorFactory(
				new SimpleTypeDataReaderAdaptorFactory(),
				new AsyncEnumerableDataReaderAdaptorFactory(),
				new ArrayDataReaderAdaptorFactory(),
				new EnumerableDataReaderAdaptorFactory(),
				new MultipleResultsetDataReaderAdaptorFactory(),
				new SingleDataReaderAdaptorFactory());
			
			SimpleSimpleTypeDataInfoRepository = new SimpleTypeDataInfoRepository();
			
			DataRowReaderFactory = new CompositeDataRowReaderFactory(
				new SimpleTypeDataRowReaderFactory(),
				new InterfaceOfComplexTypeDataRowReaderFactory(), 
				new ComplexTypeDataRowReaderFactory());

			ColumnValueConverterFactory = new CompositeColumnValueConverterFactory(
				new TextColumnValueConverterFactory(),
				new BinaryArrayColumnValueConverterFactory(),
				new NullableColumnValueConverterFactory(),
				new XmlColumnValueConverterFactory(),
				new StructColumnValueConverterFactory());

			m_columnConverterCache = new ConcurrentDictionary<Type, object>();
			m_dataReaderAdaptorCache = new ConcurrentDictionary<Type, object>();
			m_dataRowReaderCache = new ConcurrentDictionary<Type, object>();
			m_sqlParameterAdaptorCache = new ConcurrentDictionary<Type, object>();
			m_dynamicAssemblyModuleRegistry = new DynamicAssemblyModuleRegistry();
		}

		public ISqlParameterAdaptorFactory SqlParameterAdaptorFactory { get; set; }
		public ICommandInformationProvider CommandInformationProvider { get; set; }
		public IDataReaderAdaptorFactory DataReaderAdaptorFactory { get; set; }
		public IDataRowReaderFactory DataRowReaderFactory { get; set; }
		public ISimpleTypeDataInfoRepository SimpleSimpleTypeDataInfoRepository { get; set; }

		public ModuleBuilder GetModuleBuilder( string moduleName )
		{
			return m_dynamicAssemblyModuleRegistry.GetOrCreateModuleBuilder( moduleName );
		}

		public bool TryGetSimpleTypeMapping( Type type, out ISimpleTypeDataInfo simpleTypeDataInfo )
		{
			return SimpleSimpleTypeDataInfoRepository.TryGetTypeMapping( type, out simpleTypeDataInfo );
		}

		public IColumnValueConverterFactory ColumnValueConverterFactory { get; set; }

		public IColumnValueConverter<T> GetOrCreateColumnValueConverter<T>()
		{
			return (IColumnValueConverter<T>) m_columnConverterCache.GetOrAdd( typeof( T ), type =>
			{
				IColumnValueConverter<T> converter;
				return ColumnValueConverterFactory.TryCreate( this, out converter ) ? converter : null;
			} );
		}

		public IDataReaderAdaptor<T> GetOrCreateDataReaderAdaptor<T>()
		{
			return (IDataReaderAdaptor<T>) m_dataReaderAdaptorCache.GetOrAdd( typeof( T ), type =>
			{
				IDataReaderAdaptor<T> dataReaderAdaptor;
				return DataReaderAdaptorFactory.TryCreate( this, out dataReaderAdaptor ) ? dataReaderAdaptor : null;
			} );
		}

		public IDataRowReader<T> GetOrCreateDataRowReader<T>()
		{
			return (IDataRowReader<T>) m_dataRowReaderCache.GetOrAdd( typeof( T ), type =>
			{
				IDataRowReader<T> dataRowReader;
				return DataRowReaderFactory.TryCreate( this, out dataRowReader ) ? dataRowReader : null;
			} );
		}

		public ISqlParameterAdaptor<T> GetOrCreateSqlParameterAdaptor<T>()
		{
			return (ISqlParameterAdaptor<T>) m_sqlParameterAdaptorCache.GetOrAdd( typeof( T ), type =>
			{
				ISqlParameterAdaptor<T> sqlParameterAdaptor;
				return SqlParameterAdaptorFactory.TryCreate( this, out sqlParameterAdaptor ) ? sqlParameterAdaptor : null;
			} );
		}
	}
}