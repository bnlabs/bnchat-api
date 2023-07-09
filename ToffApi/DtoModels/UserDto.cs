using System.ComponentModel.DataAnnotations;
 
namespace ToffApi.DtoModels
{
    public class UserDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        public string PictureUrl { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
