using System.Data;
using System.Reflection;

namespace Bombsquad.DataProxy.CommandInformation
{
	public class MethodNameAsStoredProcedureCommandInformationProvider : ICommandInformationProvider
	{
		public bool TryGetCommandInformation( MethodInfo method, out ICommandInformation commandInformation )
		{
			commandInformation = new CommandInformation
			{
				CommandText = method.Name,
				CommandType = CommandType.StoredProcedure
			};
			return true;
		}
	}
}