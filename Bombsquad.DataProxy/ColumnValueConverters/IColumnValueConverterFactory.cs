namespace Bombsquad.DataProxy.ColumnValueConverters
{
	public interface IColumnValueConverterFactory
	{
		bool TryCreate<T>( IDataProxyContext context, out IColumnValueConverter<T> converter );
	}
}