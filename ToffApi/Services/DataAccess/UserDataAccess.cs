using MongoDB.Driver;
using ToffApi.Models;

namespace ToffApi.Services.DataAccess;

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

    public async Task<List<User>> GetUserByIdAsync(Guid userId)
    {
        var userCollection = ConnectToMongo<User>(UserCollection);
        var result = await userCollection.FindAsync(u => u.Id == userId);
        return await result.ToListAsync();
    }
    public  List<User> GetUserById(Guid userId)
    {
        var userCollection = ConnectToMongo<User>(UserCollection);
        var result = userCollection.Find(u => u.Id == userId);
        return result.ToList();
    }

    public async Task<List<User>> SearchUser(string searchInput)
    {
        var userCollection = ConnectToMongo<User>(UserCollection);
        var result = await userCollection.FindAsync(u => u.UserName.ToLower().Contains(searchInput.ToLower()));
        return result.ToList();
    }

    public async Task<string> UpdateUserPfp(Guid userId, string url)
    {
        var userCollection = ConnectToMongo<User>(UserCollection);
        var userResult = await userCollection.FindAsync(u => u.Id == userId);
        var user = userResult.ToList()[0];
        user.PictureUrl = url;
        
        var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
        var update = Builders<User>.Update.Set(u => u.PictureUrl, url);
        await userCollection.UpdateOneAsync(filter, update);
        return url;
    }
}