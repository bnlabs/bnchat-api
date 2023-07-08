namespace ToffApi.Command.CommandBuses;

public class SendDmMessageCommand
{
    public SendDmMessageCommand()
    {
        
    }
    
    public Guid SenderId { get; set; } 
    public Guid ReceiverId { get; set; }
    public string SenderName { get; set; }
    public string Content { get; set; }

}