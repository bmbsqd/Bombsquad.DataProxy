using System.Reflection;
using Bombsquad.DataProxy2.Utils;

namespace Bombsquad.DataProxy2.CommandInformation
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
