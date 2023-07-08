namespace ToffApi.Command.CommandResults;

public class SendDmMessageCommandResult
{
    public Guid Id;
    public Guid ConversationId;
    public string SenderName;
    public Guid SenderId;
    public string Content;
    public DateTime Timestamp;
}