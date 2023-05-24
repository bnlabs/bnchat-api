using MongoDB.Driver;
using Message = ToffApi.Models.Message;

namespace ToffApi.DataAccess;

public class MessageDataAccess : IMessageDataAccess
{
    private readonly string _connectionString;
    private readonly string _databaseName;
    private readonly string _messageCollection;

    public MessageDataAccess(string connectionString, string databaseName, string messageCollection)
    {
        _connectionString = connectionString;
        _databaseName = databaseName;
        _messageCollection = messageCollection;
    }
    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var client = new MongoClient(_connectionString);
        var db = client.GetDatabase(_databaseName);
        return db.GetCollection<T>(collection);
    }

    public async Task<List<Message>> GetMessagesFromConversation(Guid userId, Guid conversationId)
    {
        var messageCollection = ConnectToMongo<Message>(_messageCollection);
        var results = await messageCollection.FindAsync(msg => 
            msg.ConversationId == conversationId && msg.SenderId == userId);
        return results.ToList();
    }

    public Task AddMessage(Message msg)
    {
        var messageCollection = ConnectToMongo<Message>(_messageCollection);
        messageCollection.InsertOne(msg);
        return Task.CompletedTask;
    }
}