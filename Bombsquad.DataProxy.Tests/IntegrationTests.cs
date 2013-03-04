using System;
using System.Data;
using System.Data.SqlTypes;
using Bombsquad.DataProxy.Tests.Models;
using NUnit.Framework;

namespace Bombsquad.DataProxy.Tests
{
	[TestFixture]
	public class IntegrationTests
	{
		private ConnectionFactory m_connectionFactory;
		private DataMapperProxyClassFactory m_proxyFactory;

		[SetUp]
		public void SetUp()
		{
			m_connectionFactory = new ConnectionFactory( "server=localhost;database=Bombsquad.DataProxy.TestDB;integrated Security=SSPI;" );
			m_proxyFactory = new DataMapperProxyClassFactory( m_connectionFactory, new DataProxyConfiguration() );

			var commandScriptParts = TestResources.IntegrationTestInitScript.Split( new[] { "GO" }, StringSplitOptions.None );

			using ( var connection = m_connectionFactory.CreateConnection() )
			{
				connection.Open();

				foreach ( var commandScriptPart in commandScriptParts )
				{
					using ( var command = connection.CreateCommand() )
					{
						command.CommandType = CommandType.Text;
						command.CommandText = commandScriptPart;
						command.ExecuteNonQuery();
					}
				}
			}
		}

		[TearDown]
		public void TearDown()
		{
			Assert.That( m_connectionFactory.CurrentConnection.State, Is.EqualTo( ConnectionState.Closed ) );
		}

		[Test]
		public void FullUserCanBeLoadedFromMultipleResultsets()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var fullUser = userMapper.GetFullUser(1);

			Assert.That( fullUser, Is.Not.Null );
			Assert.That( fullUser.User, Is.Not.Null );
			TestHelpers.AssertThatUserIsRolfGöranBengtsson( fullUser.User );

			Assert.That( fullUser.Messages, Is.Not.Null );
			Assert.That( fullUser.Messages.Length, Is.EqualTo( 1 ) );
			Assert.That( fullUser.Messages[0].MessageId, Is.EqualTo( 1 ) );
			Assert.That( fullUser.Messages[0].Subject, Is.EqualTo( "Hello my friend!" ) );
			Assert.That( fullUser.Messages[0].Body, Is.EqualTo( "Hello Roffe! I hope you are well. Cheers!" ) );
			Assert.That( fullUser.Messages[0].FromUserId, Is.EqualTo( 2 ) );
			Assert.That( fullUser.Messages[0].SentDate.Date, Is.EqualTo( DateTime.Now.Date ) );
		}

		[Test]
		public void MultipleUsersCanBeFound()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var users = userMapper.FindUsers( new[] { 2, 1 } );
			
			Assert.That( users.Length, Is.EqualTo( 2 ) );
			TestHelpers.AssertThatUserIsNilsPetterSundgren( users[ 0 ] );
			TestHelpers.AssertThatUserIsRolfGöranBengtsson( users[ 1 ] );
		}

		[Test]
		public void UserCanBeAdded()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();

			var users = userMapper.ListUsers();
			Assert.That( users.Length, Is.EqualTo( 2 ) );

			userMapper.AddUser( "Carl Jan", "Granqvist", "callejanne@finest.se", null );

			users = userMapper.ListUsers();
			Assert.That( users.Length, Is.EqualTo( 3 ) );

			TestHelpers.AssertThatUserIsRolfGöranBengtsson( users[ 0 ] );
			TestHelpers.AssertThatUserIsNilsPetterSundgren( users[ 1 ] );
			TestHelpers.AssertThatUserIsCarlJahnGranqvist( users[ 2 ] );
		}

		[Test]
		public void UserCanBeAddedAsync()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var addUserTask = userMapper.AddUserAsync( "Carl Jan", "Granqvist", "callejanne@finest.se", null );
			addUserTask.Wait();
			Assert.That( addUserTask.Result, Is.EqualTo( 3 ) );
		}

		[Test]
		public void UserCanBeAddedMultiple()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			userMapper.AddUsers( new[]
			{
				new NewUser { FirstName = "Carl Jan", LastName = "Granqvist", EmailAddress = "callejanne@finest.se" },
				new NewUser { FirstName = "Carl-Philip", LastName = "Bernadotte", EmailAddress = "pippo@kungahuset.se" }
			} );

			var users = userMapper.ListUsers();
			Assert.That( users.Length, Is.EqualTo( 4 ) );

			TestHelpers.AssertThatUserIsRolfGöranBengtsson( users[ 0 ] );
			TestHelpers.AssertThatUserIsNilsPetterSundgren( users[ 1 ] );
			TestHelpers.AssertThatUserIsCarlJahnGranqvist( users[ 2 ] );
			TestHelpers.AssertThatUserIsCarlPhilipBernadotte( users[ 3 ] );
		}

		[Test]
		public void UserCanBeAddedMultipleAsync()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var task = userMapper.AddUsersAsync( new[]
			{
				new NewUser { FirstName = "Carl Jan", LastName = "Granqvist", EmailAddress = "callejanne@finest.se" },
				new NewUser { FirstName = "Carl-Philip", LastName = "Bernadotte", EmailAddress = "pippo@kungahuset.se" }
			} );

			task.Wait();

			var users = userMapper.ListUsers();
			Assert.That( users.Length, Is.EqualTo( 4 ) );
		}

		[Test]
		public void UsersIdsCanBeListed()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var userIds = userMapper.ListUserIds();
			Assert.That( userIds.Length, Is.EqualTo( 2 ) );
			Assert.That( userIds[0], Is.EqualTo( 1 ) );
			Assert.That( userIds[1], Is.EqualTo( 2 ) );
		}

		[Test]
		public void UsersIdsCanBeListedAsync()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var task = userMapper.ListUserIdsAsync();

			task.Wait();

			Assert.That( task.Result.Length, Is.EqualTo( 2 ) );
			Assert.That( task.Result[0], Is.EqualTo( 1 ) );
			Assert.That( task.Result[1], Is.EqualTo( 2 ) );
		}

		[Test]
		public void UsersIdsCanBeListedAsyncEnumerable()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var asyncEnumerable = userMapper.ListUserIdsAsAsyncEnumerable();

			using ( var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator() )
			{
				Assert.That( m_connectionFactory.CurrentConnection.State, Is.EqualTo( ConnectionState.Closed ) );

				var moveNextTask = asyncEnumerator.MoveNextAsync();
				moveNextTask.Wait();
				Assert.That( moveNextTask.Result, Is.EqualTo( true ) );
				Assert.That( asyncEnumerator.Current, Is.EqualTo( 1 ) );

				moveNextTask = asyncEnumerator.MoveNextAsync();
				moveNextTask.Wait();
				Assert.That( moveNextTask.Result, Is.EqualTo( true ) );
				Assert.That( asyncEnumerator.Current, Is.EqualTo( 2 ) );

				moveNextTask = asyncEnumerator.MoveNextAsync();
				moveNextTask.Wait();
				Assert.That( moveNextTask.Result, Is.EqualTo( false ) );
			}
		}

		[Test]
		public void UsersCanBeListed()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var users = userMapper.ListUsers();

			Assert.That( users.Length, Is.EqualTo( 2 ) );
			TestHelpers.AssertThatUserIsRolfGöranBengtsson( users[ 0 ] );
			TestHelpers.AssertThatUserIsNilsPetterSundgren( users[ 1 ] );
		}

		[Test]
		public void InterfaceUsersCanBeListed()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var users = userMapper.ListUsersAsInterface();

			Assert.That( users.Length, Is.EqualTo( 2 ) );
			TestHelpers.AssertThatUserIsRolfGöranBengtsson( users[0] );
			TestHelpers.AssertThatUserIsNilsPetterSundgren( users[1] );
		}

		[Test]
		public void UsersCanBeListedAsynchronously()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IUserMapper>();
			var listUsersTask = userMapper.ListUsersAsync();

			listUsersTask.Wait();
			var users = listUsersTask.Result;

			Assert.That( users.Length, Is.EqualTo( 2 ) );
			TestHelpers.AssertThatUserIsRolfGöranBengtsson( users[0] );
			TestHelpers.AssertThatUserIsNilsPetterSundgren( users[ 1 ] );
		}

		[Test]
		public void OutputParametersCanBeRetrieved()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IBasicMapper>();
			int output;
			userMapper.OutputParameterTest( 5, out output );

			Assert.That( output, Is.EqualTo( 10 ) );
		}

		[Test]
		public void DataTypesCanBeUsedAsParametersAndRetrievedFromReader()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IBasicMapper>();
			var guidValue = Guid.NewGuid();
			var dateValue = DateTime.Now;
			var result = userMapper.DataTypesTest( true, 1, 2, 3, 4, 5.1f, 6.2d, 7.3m, dateValue, "Hello", guidValue );

			Assert.That( result.BitValue, Is.EqualTo( true ) );
			Assert.That( result.TinyIntValue, Is.EqualTo( 1 ) );
			Assert.That( result.SmallIntValue, Is.EqualTo( 2 ) );
			Assert.That( result.IntValue, Is.EqualTo( 3 ) );
			Assert.That( result.BigIntValue, Is.EqualTo( 4 ) );
			Assert.That( result.RealValue, Is.EqualTo( 5.1f ) );
			Assert.That( result.FloatValue, Is.EqualTo( 6.2d ) );
			Assert.That( result.DecimalValue, Is.EqualTo( 7.3m ) );
			Assert.That( result.DateTimeValue, Is.EqualTo( new SqlDateTime( dateValue ).Value ) );
			Assert.That( result.StringValue, Is.EqualTo( "Hello" ) );
			Assert.That( result.GuidValue, Is.EqualTo( guidValue ) );
		}

		[Test]
		public void NullableDataTypesWithValuesCanBeUsedAsParametersAndRetrievedFromReader()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IBasicMapper>();
			var guidValue = Guid.NewGuid();
			var dateValue = DateTime.Now;
			var result = userMapper.NullableDataTypesTest( true, 1, 2, 3, 4, 5.1f, 6.2d, 7.3m, dateValue, "Hello", guidValue );

			Assert.That( result.BitValue, Is.EqualTo( true ) );
			Assert.That( result.TinyIntValue, Is.EqualTo( 1 ) );
			Assert.That( result.SmallIntValue, Is.EqualTo( 2 ) );
			Assert.That( result.IntValue, Is.EqualTo( 3 ) );
			Assert.That( result.BigIntValue, Is.EqualTo( 4 ) );
			Assert.That( result.RealValue, Is.EqualTo( 5.1f ) );
			Assert.That( result.FloatValue, Is.EqualTo( 6.2d ) );
			Assert.That( result.DecimalValue, Is.EqualTo( 7.3m ) );
			Assert.That( result.DateTimeValue, Is.EqualTo( new SqlDateTime( dateValue ).Value ) );
			Assert.That( result.StringValue, Is.EqualTo( "Hello" ) );
			Assert.That( result.GuidValue, Is.EqualTo( guidValue ) );
		}

		[Test]
		public void NullableDataTypesWithoutValuesCanBeUsedAsParametersAndRetrievedFromReader()
		{
			var userMapper = m_proxyFactory.GetOrCreate<IBasicMapper>();
			var result = userMapper.NullableDataTypesTest( null, null, null, null, null, null, null, null, null, null, null );

			Assert.That( result.BitValue, Is.Null );
			Assert.That( result.TinyIntValue, Is.Null );
			Assert.That( result.SmallIntValue, Is.Null );
			Assert.That( result.IntValue, Is.Null );
			Assert.That( result.BigIntValue, Is.Null );
			Assert.That( result.RealValue, Is.Null );
			Assert.That( result.FloatValue, Is.Null );
			Assert.That( result.DecimalValue, Is.Null );
			Assert.That( result.DateTimeValue, Is.Null );
			Assert.That( result.StringValue, Is.Null );
			Assert.That( result.GuidValue, Is.Null );
		}
	}
}