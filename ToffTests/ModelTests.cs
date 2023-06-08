using ToffApi.Models;

namespace ToffTests;

public class ModelsTests
{
    [Fact]
    public void MessageModelHaveProperties()
    {
        var message = new Message();

        var id = message.GetType().GetProperty("Id");
        var conversationId = message.GetType().GetProperty("ConversationId");
        var senderName = message.GetType().GetProperty("SenderName");
        var senderId = message.GetType().GetProperty("SenderId");
        var content = message.GetType().GetProperty("Content");
        var timestamp = message.GetType().GetProperty("Timestamp");

        Assert.NotNull(id);
        Assert.NotNull(conversationId);
        Assert.NotNull(senderName);
        Assert.NotNull(senderId);
        Assert.NotNull(content);
        Assert.NotNull(timestamp);
    }

    [Fact]
    public void ConversationModelHaveProperties()
    {
        var memberList = new List<Guid>();
        var conversation = new Conversation(memberList);

        var id = conversation.GetType().GetProperty("ConversationId");
        var memberIds = conversation.GetType().GetProperty("MemberIds");
        var messages = conversation.GetType().GetProperty("Messages");
        
        Assert.NotNull(id);
        Assert.NotNull(memberIds);
        Assert.NotNull(messages);
    }




}