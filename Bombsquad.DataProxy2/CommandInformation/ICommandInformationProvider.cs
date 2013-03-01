using System.Reflection;

namespace Bombsquad.DataProxy2.CommandInformation
{
	public interface ICommandInformationProvider
	{
		bool TryGetCommandInformation( MethodInfo method, out ICommandInformation commandInformation );
	}
}