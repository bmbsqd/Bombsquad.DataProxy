using System;

namespace Bombsquad.DataProxy.Tests.Models
{
	public class NewUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public DateTime? BirthDate { get; set; }
	}
}