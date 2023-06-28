using MongoDB.Driver;
using ToffApi.Models;
using Message = ToffApi.Models.Message;

namespace ToffApi.Services.DataAccess;

public class MessageDataAccess : IMessageDataAccess
{
    private readonly string _connectionString;
    private readonly string _databaseName;
    private const string MessageCollection = "messages";
    private const string ConversationCollection = "conversations";
    private const string UserCollection = "users";

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
        msg.Timestamp = DateTime.Now;
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
        var result = await conversationCollection.FindAsync(c => c.MemberIds.Contains(userId));
        return await result.ToListAsync();
    }

    public async Task<List<Conversation>> GetConversationById(Guid conversationId)
    {
        var conversationCollection = ConnectToMongo<Conversation>(ConversationCollection);
        var messageCollection = ConnectToMongo<Message>(MessageCollection);
        var userCollection = ConnectToMongo<User>(UserCollection);
        
        var messages = await messageCollection.Find(msg => 
            msg.ConversationId == conversationId).ToListAsync();
        
        // Retrieve all unique senderIds from the messages
        var senderIds = messages.Select(msg => msg.SenderId).Distinct().ToList();

        // Fetch all senders in a single database query
        var senders = userCollection.Find(sender => senderIds.Contains(sender.Id)).ToEnumerable();

        // Create a dictionary to map senderId to senderName
        var senderMap = senders.ToDictionary(sender => sender.Id, sender => sender.UserName);
        
        // Append senderName to each message
        foreach (var message in messages)
        {
            if (senderMap.TryGetValue(message.SenderId, out var senderName))
            {
                message.SenderName = senderName;
            }
        }
        
        var result = await conversationCollection.Find(c => c.ConversationId == conversationId).ToListAsync();
        result.ToList()[0].Messages = messages.OrderBy(m => m.Timestamp).ToList();
        
        return result;
    }
    
}