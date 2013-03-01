using System;
using Bombsquad.DataProxy.Tests.Models;
using NUnit.Framework;

namespace Bombsquad.DataProxy.Tests
{
	public static class TestHelpers
	{
		public static void AssertThatUserIsNilsPetterSundgren( IUser user )
		{
			Assert.That( user.FirstName, Is.EqualTo( "Nils-Petter" ) );
			Assert.That( user.LastName, Is.EqualTo( "Sundgren" ) );
			Assert.That( user.EmailAddress, Is.EqualTo( "nisse-p@gmail.com" ) );
			Assert.That( user.BirthDate.HasValue, Is.EqualTo( false ) );
		}

		public static void AssertThatUserIsRolfGöranBengtsson( IUser user )
		{
			Assert.That( user.FirstName, Is.EqualTo( "Rolf-Göran" ) );
			Assert.That( user.LastName, Is.EqualTo( "Bengtsson" ) );
			Assert.That( user.EmailAddress, Is.EqualTo( "rbg@hotmail.com" ) );
			Assert.That( user.BirthDate, Is.EqualTo( new DateTime( 1948, 5, 1 ) ) );
		}

		public static void AssertThatUserIsCarlJahnGranqvist( IUser user )
		{
			Assert.That( user.FirstName, Is.EqualTo( "Carl Jan" ) );
			Assert.That( user.LastName, Is.EqualTo( "Granqvist" ) );
			Assert.That( user.EmailAddress, Is.EqualTo( "callejanne@finest.se" ) );
			Assert.That( user.BirthDate.HasValue, Is.EqualTo( false ) );
		}

		public static void AssertThatUserIsCarlPhilipBernadotte( IUser user )
		{
			Assert.That( user.FirstName, Is.EqualTo( "Carl-Philip" ) );
			Assert.That( user.LastName, Is.EqualTo( "Bernadotte" ) );
			Assert.That( user.EmailAddress, Is.EqualTo( "pippo@kungahuset.se" ) );
			Assert.That( user.BirthDate.HasValue, Is.EqualTo( false ) );
		}
	}
}