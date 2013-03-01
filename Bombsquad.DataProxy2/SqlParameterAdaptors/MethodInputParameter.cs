namespace Bombsquad.DataProxy2.SqlParameterAdaptors
{
	public class MethodInputParameter<T> : MethodParameter<T>
	{
		public MethodInputParameter( string parameterName, T parameterValue ) : base( parameterName )
		{
			ParameterValue = parameterValue;
		}

		internal object UntypedParameterValue
		{
			get
			{
				return ParameterValue;
			}
		}

		public T ParameterValue { get; internal set; }
	}
}