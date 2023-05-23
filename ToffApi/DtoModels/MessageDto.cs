using System.ComponentModel.DataAnnotations;

namespace ToffApi.DtoModels;

public class MessageDto
{
    public Guid Id { get; set; }

    [Required]
    public Guid ReceiverId { get; set; }
    
    [Required]
    public Guid SenderId { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime TimeStamp { get; set; }
}