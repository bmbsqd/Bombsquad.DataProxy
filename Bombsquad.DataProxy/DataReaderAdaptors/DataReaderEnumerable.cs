using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Bombsquad.DataProxy.Async;
using Bombsquad.DataProxy.DataRowReaders;

namespace Bombsquad.DataProxy.DataReaderAdaptors
{
	public class DataReaderEnumerable<TElement> : IAsyncEnumerable<TElement>
	{
		private readonly IDataProxyContext m_context;
		private readonly IExecuteReaderContext m_connectionAndReaderContext;
		private readonly IDataRowReader<TElement> m_dataRowFactory;

		public DataReaderEnumerable( IDataProxyContext context, IExecuteReaderContext connectionAndReaderContext, IDataRowReader<TElement> dataRowFactory )
		{
			m_context = context;
			m_connectionAndReaderContext = connectionAndReaderContext;
			m_dataRowFactory = dataRowFactory;
		}

		public IAsyncEnumerator<TElement> GetAsyncEnumerator()
		{
			return new Enumerator( m_context, m_connectionAndReaderContext, m_dataRowFactory );
		}

		public IEnumerator<TElement> GetEnumerator()
		{
			return GetAsyncEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private class Enumerator : IAsyncEnumerator<TElement>
		{
			private readonly IDataProxyContext m_context;
			private readonly IExecuteReaderContext m_connectionAndReaderContext;
			private readonly IDataRowReader<TElement> m_dataRowFactory;
			private DataReaderContext m_dataReaderContext;

			public Enumerator( IDataProxyContext context, IExecuteReaderContext connectionAndReaderContext, IDataRowReader<TElement> dataRowFactory )
			{
				m_context = context;
				m_connectionAndReaderContext = connectionAndReaderContext;
				m_dataRowFactory = dataRowFactory;
			}

			public TElement Current { get; private set; }

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public void Dispose()
			{
				m_connectionAndReaderContext.Dispose();
			}

			public async Task<bool> MoveNextAsync()
			{
				if( m_connectionAndReaderContext.Reader == null )
				{
					await m_connectionAndReaderContext.OpenConnectionAndExecuteReaderAsync();
				}

				if ( ! await m_connectionAndReaderContext.Reader.ReadAsync() )
				{
					return false;
				}

				EnsureDataReaderContext();
				Current = await m_dataRowFactory.ReadAsync( m_dataReaderContext );
				return true;
			}

			public bool MoveNext()
			{
				if ( m_connectionAndReaderContext.Reader == null )
				{
					m_connectionAndReaderContext.OpenConnectionAndExecuteReader();
				}

				if ( !m_connectionAndReaderContext.Reader.Read() )
				{
					return false;
				}

				EnsureDataReaderContext();
				Current = m_dataRowFactory.Read( m_dataReaderContext );
				return true;
			}

			private void EnsureDataReaderContext()
			{
				if ( m_dataReaderContext == null )
				{
					m_dataReaderContext = new DataReaderContext( m_context, m_connectionAndReaderContext.Reader );
				}
			}

			public void Reset()
			{
				throw new NotSupportedException( "Cannot reset DataReader" );
			}

			private class DataReaderContext : IDataReaderContext
			{
				private readonly IDataProxyContext m_context;
				private readonly SqlDataReader m_reader;
				private readonly Dictionary<int, string> m_columnNameToOrdinalMap;

				public DataReaderContext( IDataProxyContext context, SqlDataReader reader )
				{
					m_context = context;
					m_reader = reader;
					m_columnNameToOrdinalMap = new Dictionary<int, string>();
					for ( var i = 0; i < reader.FieldCount; i++ )
					{
						m_columnNameToOrdinalMap.Add( i, reader.GetName( i ) );
					}
				}

				public TValue GetValue<TValue>( int ordinal )
				{
					return m_context.GetOrCreateColumnValueConverter<TValue>().Read( m_reader, ordinal );
				}

				public async Task<TValue> GetValueAsync<TValue>( int ordinal )
				{
					return await m_context.GetOrCreateColumnValueConverter<TValue>().ReadAsync( m_reader, ordinal );
				}

				public SqlDataReader DataReader
				{
					get
					{
						return m_reader;
					}
				}

				public IDictionary<int, string> OrdinalMap
				{
					get
					{
						return m_columnNameToOrdinalMap;
					}
				}
			}
		}
	}
}