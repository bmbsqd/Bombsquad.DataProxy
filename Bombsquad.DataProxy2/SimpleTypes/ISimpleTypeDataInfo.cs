using System.Data;
using System.Linq.Expressions;

namespace Bombsquad.DataProxy2.SimpleTypes
{
	public interface ISimpleTypeDataInfo
	{
		SqlDbType SqlDbType { get; }
		Expression SetSqlDataRecordValueExpression { get; }
		bool VariableLength { get; }
	}
}