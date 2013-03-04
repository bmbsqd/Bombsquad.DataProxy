using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Linq;

namespace Bombsquad.DataProxy.DataRowReaders
{
	public class InterfaceOfComplexTypeDataRowReaderFactory : ComplexTypeDataRowReaderFactory
	{
		public override bool TryCreate<T>( IDataProxyContext contex, out IDataRowReader<T> dataRowReader )
		{
			if ( !typeof( T ).IsInterface )
			{
				dataRowReader = null;
				return false;
			}

			var type = CreateInterfaceImplementation<T>( contex );

			var method = GetType().GetMethod( "CallBase", BindingFlags.NonPublic | BindingFlags.Instance ).MakeGenericMethod( new[]
			{
				typeof( T ), type
			} );
			dataRowReader = (IDataRowReader<T>) method.Invoke( this, new object[]
			{
				contex
			} );
			return true;
		}

		private IDataRowReader<TInterface> CallBase<TInterface, TImplementation>( IDataProxyContext contex ) where TImplementation : TInterface
		{
			IDataRowReader<TImplementation> dataRowReader;
			return base.TryCreate( contex, out dataRowReader ) ? new WrappingDataRowReader<TInterface, TImplementation>( dataRowReader ) : null;
		}

		private class WrappingDataRowReader<TInterface, TImplementation> : IDataRowReader<TInterface> where TImplementation : TInterface
		{
			private readonly IDataRowReader<TImplementation> m_inner;

			public WrappingDataRowReader( IDataRowReader<TImplementation> inner )
			{
				m_inner = inner;
			}

			public TInterface Read( IDataReaderContext context )
			{
				return m_inner.Read( context );
			}

			public async Task<TInterface> ReadAsync( IDataReaderContext context )
			{
				return await m_inner.ReadAsync( context );
			}
		}

		public Type CreateInterfaceImplementation<T>( IDataProxyContext contex )
		{
			var typeBuilder = contex.GetModuleBuilder( "DataRowReaders" ).DefineType( typeof( T ).Name + "Implementation" );
			typeBuilder.AddInterfaceImplementation( typeof( T ) );

			var properties = new[]
			{
				typeof( T )
			}.Union( typeof( T ).GetInterfaces() ).SelectMany( t => t.GetProperties( BindingFlags.Public | BindingFlags.Instance ) );

			foreach ( var property in properties )
			{
				var propertyBuilder = typeBuilder.DefineProperty( property.Name, PropertyAttributes.None, property.PropertyType, new Type[0] );

				var field = typeBuilder.DefineField( "m_" + property.Name, property.PropertyType, FieldAttributes.Private );

				var interfaceGetMethod = property.GetGetMethod();
				var getMethod = CreateGetMethod( typeBuilder, property, field );
				typeBuilder.DefineMethodOverride( getMethod, interfaceGetMethod );
				propertyBuilder.SetGetMethod( getMethod );

				var interfaceSetMethod = property.GetSetMethod();
				var setMethod = CreateSetMethod( typeBuilder, property, field );
				if ( interfaceSetMethod != null )
				{
					typeBuilder.DefineMethodOverride( setMethod, interfaceSetMethod );
				}
				propertyBuilder.SetSetMethod( setMethod );
			}

			return typeBuilder.CreateType();
		}

		private static MethodBuilder CreateSetMethod( TypeBuilder typeBuilder, PropertyInfo property, FieldInfo field )
		{
			var setMethod = typeBuilder.DefineMethod( property.Name + "_Set", MethodAttributes.Public | MethodAttributes.Virtual, typeof( void ), new[]
			{
				property.PropertyType
			} );
			var ilGenerator = setMethod.GetILGenerator();
			ilGenerator.Emit( OpCodes.Ldarg_0 );
			ilGenerator.Emit( OpCodes.Ldarg_1 );
			ilGenerator.Emit( OpCodes.Stfld, field );
			ilGenerator.Emit( OpCodes.Ret );
			return setMethod;
		}

		private static MethodBuilder CreateGetMethod( TypeBuilder typeBuilder, PropertyInfo property, FieldInfo field )
		{
			var getMethod = typeBuilder.DefineMethod( property.Name + "_Get", MethodAttributes.Public | MethodAttributes.Virtual, property.PropertyType, new Type[0] );
			var ilGenerator = getMethod.GetILGenerator();
			ilGenerator.Emit( OpCodes.Ldarg_0 );
			ilGenerator.Emit( OpCodes.Ldfld, field );
			ilGenerator.Emit( OpCodes.Ret );
			return getMethod;
		}
	}
}