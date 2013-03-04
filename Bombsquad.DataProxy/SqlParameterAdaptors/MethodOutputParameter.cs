using System;

namespace Bombsquad.DataProxy.SqlParameterAdaptors
{
	public class MethodOutputParameter<T> : MethodParameter<T>
	{
		public MethodOutputParameter( string parameterName )
			: base( parameterName )
		{
		}

		public Func<T> GetSqlParameterValue { get; set; }
	}
}