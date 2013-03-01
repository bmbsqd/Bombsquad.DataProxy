namespace Bombsquad.DataProxy2.SqlParameterAdaptors
{
	public interface ISqlParameterAdaptorFactory
	{
		bool TryCreate<T>( IDataProxyContext context, out ISqlParameterAdaptor<T> sqlParameterAdaptor );
	}
}