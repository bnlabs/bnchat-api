using Microsoft.Build.Framework;
using ToffApi.Models;

namespace ToffApi.DtoModels;

public class ConversationDto
{
    public Guid ConversationId{ get; set; }
    [Required]
    public List<Guid> MemberIds { get; set; }
    public List<Message> Messages { get; set; }
}