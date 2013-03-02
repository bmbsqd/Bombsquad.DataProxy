Bombsquad.DataProxy
===================

Example code
------------

public interface IUserDatamapper
{
  [Query("user.UsersListAll")]
  public Task<IUser[]> ListUsersAsync();

  [Query("user.UsersFind")]
  public Task<IUser> FindUserAsync(int userId);
}

public interface IUser
{
  int UserId { get; }
  string FirstName { get; }
  string LastName { get; }
  string EmailAddress { get; }
}

var connectionFactory = new ConnectionFactory( ConfigurationManager.ConnectionStrings["myConnectionString"] );
var proxyFactory = new DataMapperProxyClassFactory( connectionFactory, new DataProxyConfiguration() );
var userDatamapper = proxyFactory.GetOrCreate<IUserDatamapper>();
var allUsers = await userDatamapper.ListUsersAsync();
