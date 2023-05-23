using AspNetCore.Identity.MongoDbCore.Models;

namespace Toff.Models;

public class User : MongoIdentityUser<Guid>
{
    public User()
    {

    }


}