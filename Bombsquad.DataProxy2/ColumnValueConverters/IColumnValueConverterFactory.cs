namespace Bombsquad.DataProxy2.ColumnValueConverters
{
	public interface IColumnValueConverterFactory
	{
		bool TryCreate<T>( IDataProxyContext context, out IColumnValueConverter<T> converter );
	}
}