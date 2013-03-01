using System.Collections.Generic;

namespace Bombsquad.DataProxy2.Async
{
	public interface IAsyncEnumerable<out T> : IEnumerable<T>
	{
		IAsyncEnumerator<T> GetAsyncEnumerator();
	}
}