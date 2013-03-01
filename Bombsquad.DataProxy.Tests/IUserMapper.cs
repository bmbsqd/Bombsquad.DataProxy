using System;
using System.Threading.Tasks;
using Bombsquad.DataProxy.Tests.Models;
using Bombsquad.DataProxy2.Async;
using Bombsquad.DataProxy2.CommandInformation;

namespace Bombsquad.DataProxy.Tests
{
	public interface IUserMapper
	{
		[Query( "UsersFullFindById" )]
		FullUser GetFullUser(int userId);

		[Query( "UsersListUserIds" )]
		int[] ListUserIds();

		[Query( "UsersListUserIds" )]
		Task<int[]> ListUserIdsAsync();

		[Query( "UsersListUserIds" )]
		IAsyncEnumerable<int> ListUserIdsAsAsyncEnumerable();

		[Query( "UsersList" )]
		User[] ListUsers();

		[Query( "UsersList" )]
		IUser[] ListUsersAsInterface();

		[Query( "UsersList" )]
		Task<User[]> ListUsersAsync();

		[Query( "UsersList" )]
		IAsyncEnumerable<User> ListUsersAsAsyncEnumerable();

		[Query( "UsersFindByUserIds" )]
		User[] FindUsers( int[] userIds );

		[Query( "UsersAdd" )]
		int AddUser( string firstName, string lastName, string emailAddress, DateTime? birthDate );

		[Query( "UsersAdd" )]
		Task<int> AddUserAsync( string firstName, string lastName, string emailAddress, DateTime? birthDate );

		[Query( "UsersAddMultiple" )]
		void AddUsers( NewUser[] users );

		[Query( "UsersAddMultiple" )]
		Task AddUsersAsync( NewUser[] users );
	}
}