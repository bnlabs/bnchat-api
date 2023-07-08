namespace ToffApi.Command.CommandBuses;

public class UpdatePfpCommand : Command
{
    public Guid UserId { get; set; }
    public IFormFile File { get; set; }
}