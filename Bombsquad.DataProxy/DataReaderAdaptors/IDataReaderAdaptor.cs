using System.Threading.Tasks;

namespace Bombsquad.DataProxy.DataReaderAdaptors
{
	public interface IDataReaderAdaptor<T>
	{
		T Read( IExecuteReaderContext connectionAndReaderContext );
		Task<T> ReadAsync( IExecuteReaderContext connectionAndReaderContext );
	}
}