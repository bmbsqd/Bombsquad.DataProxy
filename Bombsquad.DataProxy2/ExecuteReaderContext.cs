using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy2
{
	internal class ExecuteReaderContext : IExecuteReaderContext
	{
		private readonly IConnectionFactory m_connectionFactory;
		private readonly Action<SqlCommand> m_setupCommandAction;
		private SqlConnection m_connection;
		private SqlCommand m_command;
		private SqlDataReader m_reader;

		public ExecuteReaderContext( IConnectionFactory connectionFactory, Action<SqlCommand> setupCommandAction )
		{
			m_connectionFactory = connectionFactory;
			m_setupCommandAction = setupCommandAction;
		}

		public SqlDataReader Reader
		{
			get
			{
				return m_reader;
			}
		}

		public void OpenConnectionAndExecuteReader()
		{
			m_connection = m_connectionFactory.CreateConnection();
			m_connection.Open();
			m_command = m_connection.CreateCommand();
			m_setupCommandAction( m_command );
			m_reader = m_command.ExecuteReader();
		}

		public async Task OpenConnectionAndExecuteReaderAsync()
		{
			m_connection = m_connectionFactory.CreateConnection();
			await m_connection.OpenAsync();
			m_command = m_connection.CreateCommand();
			m_setupCommandAction( m_command );
			m_reader = await m_command.ExecuteReaderAsync();
		}

		public void Dispose()
		{
			if ( m_reader != null )
			{
				m_reader.Dispose();
				m_reader = null;
			}

			if ( m_command != null )
			{
				m_command.Dispose();
				m_command = null;
			}

			if ( m_connection != null )
			{
				m_connection.Dispose();
				m_connection = null;
			}
		}
	}
}