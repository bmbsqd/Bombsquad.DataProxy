using System.Data;

namespace Bombsquad.DataProxy.CommandInformation
{
	public interface ICommandInformation
	{
		string CommandText { get; }
		CommandType CommandType { get; }
	}
}