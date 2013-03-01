using System.Data;

namespace Bombsquad.DataProxy2.CommandInformation
{
	internal class CommandInformation : ICommandInformation
	{
		public string CommandText { get; set; }
		public CommandType CommandType { get; set; }
	}
}