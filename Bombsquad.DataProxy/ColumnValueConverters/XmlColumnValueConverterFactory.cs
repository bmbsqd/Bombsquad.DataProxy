using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Bombsquad.DataProxy.ColumnValueConverters
{
	public class XmlColumnValueConverterFactory : IColumnValueConverterFactory
	{
		public bool TryCreate<T>( IDataProxyContext context, out IColumnValueConverter<T> converter )
		{
			if( typeof(T) == typeof(XmlReader) )
			{
				converter = (IColumnValueConverter<T>) new XmlReaderColumnValueConverter();
				return true;
			}

			if( typeof(T) == typeof(XmlDocument) )
			{
				converter = (IColumnValueConverter<T>) new XmlDocumentColumnValueConverter();
				return true;
			}

			if( typeof(T) == typeof( XDocument ) )
			{
				converter = (IColumnValueConverter<T>) new XDocumentColumnValueConverter();
				return true;
			}

			converter = null;
			return false;
		}

		public class XmlReaderColumnValueConverter : IColumnValueConverter<XmlReader>
		{
			public XmlReader Read( SqlDataReader reader, int ordinal )
			{
				return reader.IsDBNull( ordinal ) ? null : reader.GetXmlReader( ordinal );
			}

			public async Task<XmlReader> ReadAsync( SqlDataReader reader, int ordinal )
			{
				return await reader.IsDBNullAsync( ordinal ) ? null : reader.GetXmlReader( ordinal );
			}
		}

		public class XmlDocumentColumnValueConverter : IColumnValueConverter<XmlDocument>
		{
			public XmlDocument Read( SqlDataReader reader, int ordinal )
			{
				if ( reader.IsDBNull( ordinal ) )
				{
					return null;
				}
				
				var doc = new XmlDocument();
				doc.Load( reader.GetXmlReader( ordinal ) );
				return doc;
			}

			public async Task<XmlDocument> ReadAsync( SqlDataReader reader, int ordinal )
			{
				if ( await reader.IsDBNullAsync( ordinal ) )
				{
					return null;
				}

				var doc = new XmlDocument();
				doc.Load( reader.GetXmlReader( ordinal ) );
				return doc;
			}
		}

		public class XDocumentColumnValueConverter : IColumnValueConverter<XDocument>
		{
			public XDocument Read( SqlDataReader reader, int ordinal )
			{
				if ( reader.IsDBNull( ordinal ) )
				{
					return null;
				}
				
				return XDocument.Load( reader.GetXmlReader( ordinal ) );
			}

			public async Task<XDocument> ReadAsync( SqlDataReader reader, int ordinal )
			{
				if ( await reader.IsDBNullAsync( ordinal ) )
				{
					return null;
				}

				return XDocument.Load( reader.GetXmlReader( ordinal ) );
			}
		}
	}
}