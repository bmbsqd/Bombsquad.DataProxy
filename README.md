Bombsquad.DataProxy
===================

Mainly targets developers that are using SQL and don't want to use a full ORM framework  
If you don't like it, contribute to make it better, or use Dapper.net (or other framework) instead.

Example code
------------

```c#
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

public class Demo
{
  public void Run()
  {
    var connectionFactory = new ConnectionFactory( ConfigurationManager.ConnectionStrings["myConnectionString"] );
    var proxyFactory = new DataMapperProxyClassFactory( connectionFactory, new DataProxyConfiguration() );
    var userDatamapper = proxyFactory.GetOrCreate<IUserDatamapper>();
    var allUsers = await userDatamapper.ListUsersAsync();
    var user = await userDatamapper.FindUserAsync(1);
  }
}
```

Some of the features
--------------------
 * Maps columns to properties, and if needed creates implementation of returned interface (in above example IUser)
 * Support for multiple resultsets
 * Maps method arguments to SQL parameters, with support for table valued parameters!
 * Async support, either by returning Task<IEnumerable<TReturnType>> (will be buffered in memory) or IAsyncEnumerable<TReturnType>. 
 * Fully configurable, add your own implementations in the DataProxyConfiguration. For instance, if you want to use name conventions to build sql to send to the server, feel free to write a custom implementation of the ICommandInformationProvider interface
 * Support for output parameters
