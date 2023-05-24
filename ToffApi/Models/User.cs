using AspNetCore.Identity.MongoDbCore.Models;

namespace ToffApi.Models;

public class User : MongoIdentityUser<Guid>
{
    public User()
    {

    }


}