using System.Collections.Generic;

namespace Bombsquad.DataProxy.Async
{
	public interface IAsyncEnumerable<out T> : IEnumerable<T>
	{
		IAsyncEnumerator<T> GetAsyncEnumerator();
	}
}