using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy.ColumnValueConverters
{
	public class StructColumnValueConverterFactory : IColumnValueConverterFactory
	{
		public bool TryCreate<T>( IDataProxyContext context, out IColumnValueConverter<T> converter )
		{
			if(typeof(T).IsValueType)
			{
				var converterType = typeof( StructColumnValueConverter<> ).MakeGenericType( typeof( T ) );
				var factory = Expression.Lambda<Func<IColumnValueConverter<T>>>( Expression.New( converterType.GetConstructors().Single() ) ).Compile();
				converter = factory();
				return true;
			}

			converter = null;
			return false;
		}

		private class StructColumnValueConverter<TValue> : IColumnValueConverter<TValue> where TValue : struct
		{
			public TValue Read( SqlDataReader reader, int ordinal )
			{
				return reader.GetFieldValue<TValue>( ordinal );
			}

			public Task<TValue> ReadAsync( SqlDataReader reader, int ordinal )
			{
				return reader.GetFieldValueAsync<TValue>( ordinal );
			}
		}
	}
}