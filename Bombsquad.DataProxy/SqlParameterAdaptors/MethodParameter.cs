namespace Bombsquad.DataProxy.SqlParameterAdaptors
{
	public abstract class MethodParameter<T>
	{
		protected MethodParameter( string parameterName )
		{
			ParameterName = parameterName;
		}

		public string ParameterName { get; private set; }
	}
}