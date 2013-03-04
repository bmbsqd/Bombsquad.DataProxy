using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy.Async
{
	public static class AsyncEnumerableExtensions
	{
		public static async Task<List<T>> ToListAsync<T>( this IAsyncEnumerable<T> enumerable )
		{
			using ( var enumerator = enumerable.GetAsyncEnumerator() )
			{
				var values = new List<T>();
				while ( await enumerator.MoveNextAsync() )
				{
					values.Add( enumerator.Current );
				}
				return values;
			}
		}

		public static async Task<T[]> ToArrayAsync<T>( this IAsyncEnumerable<T> enumerable )
		{
			return (await enumerable.ToListAsync()).ToArray();
		}

		public static async Task<T> FirstOrDefaultAsync<T>( this IAsyncEnumerable<T> enumerable )
		{
			using ( var enumerator = enumerable.GetAsyncEnumerator() )
			{
				return await enumerator.MoveNextAsync() ? enumerator.Current : default(T);
			}
		}

		public static async Task ForEachAsync<T>( this IAsyncEnumerable<T> enumerable, Func<T, Task> action )
		{
			using ( var enumerator = enumerable.GetAsyncEnumerator() )
			{
				while ( await enumerator.MoveNextAsync() )
				{
					await action( enumerator.Current );
				}
			}
		}
	}
}