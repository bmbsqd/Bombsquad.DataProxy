namespace Bombsquad.DataProxy.SqlParameterAdaptors
{
	public interface ISqlParameterAdaptorFactory
	{
		bool TryCreate<T>( IDataProxyContext context, out ISqlParameterAdaptor<T> sqlParameterAdaptor );
	}
}