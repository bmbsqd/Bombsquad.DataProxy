namespace Bombsquad.DataProxy2.SqlParameterAdaptors
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