using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy
{
	public interface IDataReaderContext
	{
		TValue GetValue<TValue>( int ordinal );
		Task<TValue> GetValueAsync<TValue>( int ordinal );
		SqlDataReader DataReader { get; }
		IDictionary<int,string> OrdinalMap { get; }
	}
}