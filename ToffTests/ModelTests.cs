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
        var timestamp = message.GetType().GetProperty("TimeStamp");

        Assert.NotNull(id);
        Assert.NotNull(conversationId);
        Assert.NotNull(senderName);
        Assert.NotNull(senderId);
        Assert.NotNull(content);
        Assert.NotNull(timestamp);
    }




}