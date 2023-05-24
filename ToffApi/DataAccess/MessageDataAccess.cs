using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ToffApi.Models;
using Message = ToffApi.Models.Message;

namespace ToffApi.DataAccess;

public class MessageDataAccess : IMessageDataAccess
{
    private readonly string _connectionString;
    private readonly string _databaseName;
    private const string MessageCollection = "messages";
    private const string ConversationCollection = "conversations";

    public MessageDataAccess(string connectionString, string databaseName)
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

    public async Task<List<Message>> GetMessagesFromConversation(Guid userId, Guid conversationId)
    {
        var messageCollection = ConnectToMongo<Message>(MessageCollection);
        var results = await messageCollection.FindAsync(msg => 
            msg.ConversationId == conversationId);
        return results.ToList();
    }

    public Task AddMessage(Message msg)
    {
        var messageCollection = ConnectToMongo<Message>(MessageCollection);
        messageCollection.InsertOne(msg);
        return Task.CompletedTask;
    }

    public Task AddConversation(Conversation conversation)
    {
        var conversationCollection = ConnectToMongo<Conversation>(ConversationCollection);
        conversationCollection.InsertOne(conversation);
        return Task.CompletedTask;
    }

    public async Task<List<Conversation>> GetConversationByUserId(Guid userId)
    {
        var conversationCollection = ConnectToMongo<Conversation>(ConversationCollection);
        var result = await conversationCollection.FindAsync(c => c.MemberIds.Any(m => m == userId));
        return await result.ToListAsync();
    }
    
}