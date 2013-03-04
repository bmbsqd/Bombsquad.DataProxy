using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy
{
	public interface IExecuteReaderContext : IDisposable
	{
		SqlDataReader Reader { get; }
		void OpenConnectionAndExecuteReader();
		Task OpenConnectionAndExecuteReaderAsync();
	}
}