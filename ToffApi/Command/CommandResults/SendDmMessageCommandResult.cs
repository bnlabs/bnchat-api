using ToffApi.Models;

namespace ToffApi.Command.CommandResults;

public class SendDmMessageCommandResult
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string SenderName { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public List<Embed> Embeds { get; set; }
    public bool NewConversation { get; set; }
}