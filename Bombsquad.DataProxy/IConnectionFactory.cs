using System.Data.SqlClient;

namespace Bombsquad.DataProxy
{
	public interface IConnectionFactory
	{
		SqlConnection CreateConnection();
	}
}