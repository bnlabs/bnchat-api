namespace ToffApi.Command.CommandBuses;

public abstract class Command
{
    protected Command(IHttpContextAccessor callerContext)
    {
        CallerContext = callerContext;
    }

    public IHttpContextAccessor CallerContext;
}