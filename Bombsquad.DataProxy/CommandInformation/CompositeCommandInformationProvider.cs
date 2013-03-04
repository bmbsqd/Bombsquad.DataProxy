using System.Reflection;

namespace Bombsquad.DataProxy.CommandInformation
{
	public class CompositeCommandInformationProvider : ICommandInformationProvider
	{
		private readonly ICommandInformationProvider[] m_providers;

		public CompositeCommandInformationProvider(params ICommandInformationProvider[] providers)
		{
			m_providers = providers;
		}

		public bool TryGetCommandInformation( MethodInfo method, out ICommandInformation commandInformation )
		{
			foreach ( var provider in m_providers )
			{
				if(provider.TryGetCommandInformation( method, out commandInformation ))
				{
					return true;
				}
			}

			commandInformation = null;
			return false;
		}
	}
}