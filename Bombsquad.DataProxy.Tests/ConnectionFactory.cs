using System.Data.SqlClient;
using Bombsquad.DataProxy2;

namespace Bombsquad.DataProxy.Tests
{
	public class ConnectionFactory : IConnectionFactory
	{
		private readonly string m_connectionString;

		public ConnectionFactory( string connectionString )
		{
			m_connectionString = connectionString;
		}

		public SqlConnection CreateConnection()
		{
			CurrentConnection = new SqlConnection( m_connectionString );
			return CurrentConnection;
		}

		public SqlConnection CurrentConnection
		{
			get;
			set;
		}
	}
}