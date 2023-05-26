using MongoDB.Driver;
using ToffApi.Models;

namespace ToffApi.DataAccess;

public class UserDataAccess : IUserDataAccess
{

    private readonly string _connectionString;
    private readonly string _databaseName;
    private const string UserCollection = "users";

    public UserDataAccess(string connectionString, string databaseName)
    {
        _connectionString = connectionString;
        _databaseName = databaseName;
    }
    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var client = new MongoClient(_connectionString);
        var db = client.GetDatabase(_databaseName);
        return db.GetCollection<T>(collection);
    }

    public async Task<List<User>> GetUserById(Guid userId)
    {
        var userCollection = ConnectToMongo<User>(UserCollection);
        var result = await userCollection.FindAsync(u => u.Id == userId);
        return await result.ToListAsync();
    }
}