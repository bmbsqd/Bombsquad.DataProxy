using System.Data;

namespace Bombsquad.DataProxy2.CommandInformation
{
	public interface ICommandInformation
	{
		string CommandText { get; }
		CommandType CommandType { get; }
	}
}