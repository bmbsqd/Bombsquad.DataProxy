using System.Data.SqlClient;

namespace Bombsquad.DataProxy2
{
	public interface IConnectionFactory
	{
		SqlConnection CreateConnection();
	}
}