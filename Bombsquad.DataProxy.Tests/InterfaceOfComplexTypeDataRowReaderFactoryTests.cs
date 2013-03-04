using System;
using Bombsquad.DataProxy.DataRowReaders;
using Bombsquad.DataProxy.Tests.Models;
using NUnit.Framework;

namespace Bombsquad.DataProxy.Tests
{
	[TestFixture]
	public class InterfaceOfComplexTypeDataRowReaderFactoryTests
	{
		[Test]
		public void InterfaceImplementationCanBeCreated()
		{
			var factory = new InterfaceOfComplexTypeDataRowReaderFactory();
			var userImplementationType = factory.CreateInterfaceImplementation<IUser>( new DataProxyConfiguration() );

			var user =  (IUser)Activator.CreateInstance( userImplementationType );
			Assert.That( user, Is.Not.Null );

			Assert.That( user.UserId, Is.EqualTo( default( int ) ) );
			Assert.That( user.BirthDate, Is.EqualTo( default(DateTime?) ) );
			Assert.That( user.EmailAddress, Is.EqualTo( default( string ) ) );
			Assert.That( user.FirstName, Is.EqualTo( default( string ) ) );
			Assert.That( user.LastName, Is.EqualTo( default( string ) ) );

			userImplementationType.GetProperty( "UserId" ).GetSetMethod().Invoke( user, new object[] { 1 } );
			Assert.That( user.UserId, Is.EqualTo( 1 ) );

			userImplementationType.GetProperty( "BirthDate" ).GetSetMethod().Invoke( user, new object[] { new DateTime(1901, 1, 1) } );
			Assert.That( user.BirthDate, Is.EqualTo( new DateTime( 1901, 1, 1 ) ) );

			userImplementationType.GetProperty( "EmailAddress" ).GetSetMethod().Invoke( user, new object[] { "roffe@roff.ee" } );
			Assert.That( user.EmailAddress, Is.EqualTo( "roffe@roff.ee" ) );
		}
	}
}