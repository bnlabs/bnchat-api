using AspNetCore.Identity.MongoDbCore.Models;

namespace ToffApi.Models;

public class User : MongoIdentityUser<Guid>
{
    public string PictureUrl { get; set; }
    public User()
    {
        
    }


}