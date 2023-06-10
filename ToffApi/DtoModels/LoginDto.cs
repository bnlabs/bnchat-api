using System.ComponentModel.DataAnnotations;

namespace ToffApi.DtoModels;

public class LoginDto
{
    [Required] 
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string Email { get; set; }

    [Required] 
    public string Password { get; set; }
}