using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy2.ColumnValueConverters
{
	public class BinaryArrayColumnValueConverterFactory : IColumnValueConverterFactory
	{
		public bool TryCreate<T>( IDataProxyContext context, out IColumnValueConverter<T> converter )
		{
			if( typeof(T) == typeof(byte[]) )
			{
				converter = (IColumnValueConverter<T>) new ByteArrayColumnValueConverter();
				return true;
			}

			if( typeof(T) == typeof(Stream) )
			{
				converter = (IColumnValueConverter<T>) new StreamColumnValueConverter();
				return true;
			}

			converter = null;
			return false;
		}

		private class ByteArrayColumnValueConverter : IColumnValueConverter<byte[]>
		{
			public byte[] Read( SqlDataReader reader, int ordinal )
			{
				return reader.IsDBNull( ordinal ) ? null : (byte[])reader[ordinal];
			}

			public async Task<byte[]> ReadAsync( SqlDataReader reader, int ordinal )
			{
				return await reader.IsDBNullAsync( ordinal ) ? null : (byte[]) reader[ordinal];
			}
		}

		private class StreamColumnValueConverter : IColumnValueConverter<Stream>
		{
			public Stream Read( SqlDataReader reader, int ordinal )
			{
				return reader.IsDBNull( ordinal ) ? null : reader.GetStream( ordinal );
			}

			public async Task<Stream> ReadAsync( SqlDataReader reader, int ordinal )
			{
				return await reader.IsDBNullAsync( ordinal ) ? null : reader.GetStream( ordinal );
			}
		}
	}
}