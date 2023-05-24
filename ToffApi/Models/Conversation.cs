using MongoDB.Bson.Serialization.Attributes;

namespace ToffApi.Models;

public class Conversation
{
    public Conversation(List<Guid> memberIds)
    {
        MemberIds = memberIds;
        Messages = new List<Message>();
    }
    [BsonId]
    public Guid ConversationId{ get; set; }
    public List<Guid> MemberIds { get; set; }
    [BsonIgnore]
    public List<Message> Messages { get; set; }
}