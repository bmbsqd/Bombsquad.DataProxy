using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy2
{
	public class SqlExecutable
	{
		private readonly IConnectionFactory m_connectionFactory;
		private readonly IDataProxyContext m_context;

		public SqlExecutable( IConnectionFactory connectionFactory, IDataProxyContext context )
		{
			m_connectionFactory = connectionFactory;
			m_context = context;
		}

		public int ExecuteNonQuery( Action<SqlCommand> setupCommandAction )
		{
			using ( var connection = m_connectionFactory.CreateConnection() )
			{
				connection.Open();
				using ( var command = connection.CreateCommand() )
				{
					setupCommandAction( command );
					return command.ExecuteNonQuery();
				}
			}
		}

		public async Task<int> ExecuteNonQueryAsync( Action<SqlCommand> setupCommandAction )
		{
			using ( var connection = m_connectionFactory.CreateConnection() )
			{
				await connection.OpenAsync();
				using ( var command = connection.CreateCommand() )
				{
					setupCommandAction( command );
					return await command.ExecuteNonQueryAsync();
				}
			}
		}

		public TReturnType ExecuteReader<TReturnType>( Action<SqlCommand> setupCommandAction )
		{
			var context = new ExecuteReaderContext( m_connectionFactory, setupCommandAction );
			try
			{
				return m_context.GetOrCreateDataReaderAdaptor<TReturnType>().Read( context );
			}
			catch
			{
				context.Dispose();
				throw;
			}
		}

		public async Task<TReturnType> ExecuteReaderAsync<TReturnType>( Action<SqlCommand> setupCommandAction )
		{
			var context = new ExecuteReaderContext( m_connectionFactory, setupCommandAction );
			try
			{
				return await m_context.GetOrCreateDataReaderAdaptor<TReturnType>().ReadAsync( context );
			}
			catch
			{
				context.Dispose();
				throw;
			}
		}
	}
}