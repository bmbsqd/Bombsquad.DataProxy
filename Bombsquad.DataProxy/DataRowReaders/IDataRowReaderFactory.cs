namespace Bombsquad.DataProxy.DataRowReaders
{
	public interface IDataRowReaderFactory
	{
		bool TryCreate<T>( IDataProxyContext contex, out IDataRowReader<T> dataRowReader );
	}
}