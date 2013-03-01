using System.Threading.Tasks;

namespace Bombsquad.DataProxy2.DataRowReaders
{
	public interface IDataRowReader<T>
	{
		T Read( IDataReaderContext context );
		Task<T> ReadAsync( IDataReaderContext context );
	}
}