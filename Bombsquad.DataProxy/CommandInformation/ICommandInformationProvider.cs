using System.Reflection;

namespace Bombsquad.DataProxy.CommandInformation
{
	public interface ICommandInformationProvider
	{
		bool TryGetCommandInformation( MethodInfo method, out ICommandInformation commandInformation );
	}
}