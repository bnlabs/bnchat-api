using System.ComponentModel.DataAnnotations;

namespace ToffApi.DtoModels;

public class MessageDto
{
    public Guid Id { get; set; }
    
    [Required] 
    public Guid ConversationId { get; set; }
    [Required] 
    public string SenderName { get; set; }

    [Required]
    public Guid SenderId { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime Timestamp { get; set; }
}