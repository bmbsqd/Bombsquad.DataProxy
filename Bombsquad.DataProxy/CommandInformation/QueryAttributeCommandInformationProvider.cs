using System.Reflection;
using Bombsquad.DataProxy.Utils;

namespace Bombsquad.DataProxy.CommandInformation
{
	public class QueryAttributeCommandInformationProvider : ICommandInformationProvider
	{
		public bool TryGetCommandInformation( MethodInfo method, out ICommandInformation commandInformation )
		{
			QueryAttribute attribute;
			if( !method.TryGetCustomAttribute( out attribute ) )
			{
				commandInformation = null;
				return false;
			}

			commandInformation = new CommandInformation
			{
				CommandText = attribute.CommandText,
				CommandType = attribute.CommandType
			};
			return true;
		}
	}
}
