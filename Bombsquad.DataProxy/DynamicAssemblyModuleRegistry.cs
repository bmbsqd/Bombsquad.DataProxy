using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Bombsquad.DataProxy
{
	internal class DynamicAssemblyModuleRegistry
	{
		private readonly AssemblyBuilder m_assemblyBuilder;
		private readonly ConcurrentDictionary<string, ModuleBuilder> m_moduleBuilders;

		public DynamicAssemblyModuleRegistry()
		{
			m_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly( new AssemblyName( "Bombsquad.DataProxy.Generated" ), AssemblyBuilderAccess.Run );
			m_moduleBuilders = new ConcurrentDictionary<string, ModuleBuilder>();
		}

		public ModuleBuilder GetOrCreateModuleBuilder(string moduleName)
		{
			return m_moduleBuilders.GetOrAdd( moduleName, key => m_assemblyBuilder.DefineDynamicModule( key ) );
		}
	}
}