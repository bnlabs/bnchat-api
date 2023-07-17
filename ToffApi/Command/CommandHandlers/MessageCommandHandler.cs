using ToffApi.Command.CommandBuses;
using ToffApi.Command.CommandResults;
using ToffApi.Exceptions;
using ToffApi.Models;
using ToffApi.Query.Queries;
using ToffApi.Query.QueryHandlers;
using ToffApi.Services.DataAccess;
using ToffApi.Services.EmbedGenerator;

namespace ToffApi.Command.CommandHandlers;

public class MessageCommandHandler : CommandHandler
{
    private readonly IMessageDataAccess _messageDataAccess;
    private readonly MessageQueryHandler _messageQueryHandler;

    public MessageCommandHandler(IMessageDataAccess messageDataAccess,
        MessageQueryHandler messageQueryHandler)
    {
        _messageDataAccess = messageDataAccess;
        _messageQueryHandler = messageQueryHandler;
    }

    public async Task<SendDmMessageCommandResult> HandleAsync(SendDmMessageCommand command)
    {
        // check if conversation already exist, if it does, then get conversation
        var commandResult = new SendDmMessageCommandResult();
        var getConversationQuery = new GetConversationBetweenUsersQuery(command.SenderId, command.ReceiverId);
        var msg = new Message();
        try
        {
            var conversation = await _messageQueryHandler.HandleAsync(getConversationQuery);
            commandResult.NewConversation = false;
            msg = new Message()
            {
                ConversationId = conversation.ConversationId,
                SenderId = command.SenderId,
                SenderName = command.SenderName,
                Content = command.Content
            };

            await _messageDataAccess.AddMessage(msg);
        }
        catch (ConversationNotFoundException)
        {
            commandResult.NewConversation = true;
            var memberList = new List<Guid>
            {
                command.SenderId,
                command.ReceiverId
            };

            var c = new Conversation(memberList);
            await _messageDataAccess.AddConversation(c);
            msg = new Message()
            {
                ConversationId = c.ConversationId,
                SenderId = command.SenderId,
                SenderName = command.SenderName,
                Content = command.Content
            };
            await _messageDataAccess.AddMessage(msg);
        }

        commandResult.Id = msg.Id;
        commandResult.SenderId = msg.SenderId;
        commandResult.SenderName = msg.SenderName;
        commandResult.Content = msg.Content;
        commandResult.ConversationId = msg.ConversationId;
        commandResult.Timestamp = msg.Timestamp;
        commandResult.Embeds = EmbedGenerator.GenerateEmbed(command.Content);
        
        if (!string.IsNullOrEmpty(command.ReceiverId.ToString()))
        {
            commandResult.ReceiverId = command.ReceiverId;
        }

        return commandResult;
    }
}