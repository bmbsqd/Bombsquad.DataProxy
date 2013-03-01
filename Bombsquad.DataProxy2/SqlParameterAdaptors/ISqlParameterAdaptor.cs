using System.Data.SqlClient;

namespace Bombsquad.DataProxy2.SqlParameterAdaptors
{
	public interface ISqlParameterAdaptor<T>
	{
		void AddSqlInputParameters( SqlParameterCollection parameters, MethodInputParameter<T> inputParameter );
		void AddSqlOutputParameters( SqlParameterCollection parameters, MethodOutputParameter<T> outputParameter );
	}
}