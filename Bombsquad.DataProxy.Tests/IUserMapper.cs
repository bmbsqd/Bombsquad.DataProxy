using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bombsquad.DataProxy.Async;
using Bombsquad.DataProxy.CommandInformation;
using Bombsquad.DataProxy.Tests.Models;

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
		IReadOnlyCollection<User> ListUsersAsReadOnlyCollection();

		[Query( "UsersList" )]
		IList<User> ListUsersAsListInterface();

		[Query( "UsersList" )]
		ICollection<User> ListUsersAsCollectionInterface();

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