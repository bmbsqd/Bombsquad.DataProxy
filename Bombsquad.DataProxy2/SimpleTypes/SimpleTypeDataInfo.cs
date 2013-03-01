using System.Data;
using System.Linq.Expressions;

namespace Bombsquad.DataProxy2.SimpleTypes
{
	internal class SimpleTypeDataInfo : ISimpleTypeDataInfo
	{
		public SqlDbType SqlDbType { get; set; }
		public Expression SetSqlDataRecordValueExpression { get; set; }
		public bool VariableLength { get; set; }
	}
}