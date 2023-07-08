namespace ToffApi.Query.Queries;

public abstract class Query
{
    protected Query(IHttpContextAccessor callerContext)
    {
        CallerContext = callerContext;
    }

    public IHttpContextAccessor CallerContext;

}