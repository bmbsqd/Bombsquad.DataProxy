using System.Threading.Tasks;

namespace Bombsquad.DataProxy.DataRowReaders
{
	public interface IDataRowReader<T>
	{
		T Read( IDataReaderContext context );
		Task<T> ReadAsync( IDataReaderContext context );
	}
}