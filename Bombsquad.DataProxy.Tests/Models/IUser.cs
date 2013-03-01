using System;

namespace Bombsquad.DataProxy.Tests.Models
{
	public interface IUser
	{
		int UserId { get; }
		string FirstName { get; }
		string LastName { get; }
		string EmailAddress { get; }
		DateTime? BirthDate { get; }
	}
}