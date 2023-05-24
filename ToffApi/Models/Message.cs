using MongoDB.Bson.Serialization.Attributes;

namespace ToffApi.Models;

public class Message
{
    public Message()
    {

    }
    [BsonId]
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    
    [BsonIgnore]
    public string SenderName { get; set; }
    public Guid SenderId { get; set; }

    public string Content { get; set; }

    public DateTime TimeStamp { get; set; }

}