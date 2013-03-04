using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Bombsquad.DataProxy.ColumnValueConverters
{
	public class TextColumnValueConverterFactory : IColumnValueConverterFactory
	{
		public bool TryCreate<T>( IDataProxyContext context, out IColumnValueConverter<T> converter )
		{
			if ( typeof( T ) == typeof( string ) )
			{
				converter = (IColumnValueConverter<T>) new StringColumnValueConverter();
				return true;
			}

			if( typeof(T) == typeof(TextReader) )
			{
				converter = (IColumnValueConverter<T>) new TextReaderColumnValueConverter();
				return true;
			}

			converter = null;
			return false;
		}

		private class StringColumnValueConverter : IColumnValueConverter<string>
		{
			public string Read( SqlDataReader reader, int ordinal )
			{
				return reader.IsDBNull( ordinal ) ? null : reader.GetFieldValue<string>( ordinal );
			}

			public async Task<string> ReadAsync( SqlDataReader reader, int ordinal )
			{
				return await reader.IsDBNullAsync( ordinal ) ? null : await reader.GetFieldValueAsync<string>( ordinal );
			}
		}

		private class TextReaderColumnValueConverter : IColumnValueConverter<TextReader>
		{
			public TextReader Read( SqlDataReader reader, int ordinal )
			{
				return reader.IsDBNull( ordinal ) ? null : reader.GetTextReader( ordinal );
			}

			public async Task<TextReader> ReadAsync( SqlDataReader reader, int ordinal )
			{
				return await reader.IsDBNullAsync( ordinal ) ? null : reader.GetTextReader( ordinal );
			}
		}
	}
}