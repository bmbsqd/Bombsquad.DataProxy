using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy.Async
{
	public interface IAsyncEnumerator<out T> : IEnumerator<T>
	{
		Task<bool> MoveNextAsync();
	}
}