using ToffApi.Command.CommandBuses;
using ToffApi.Command.CommandResults;
using ToffApi.Exceptions;
using ToffApi.Models;
using ToffApi.Query.Queries;
using ToffApi.Query.QueryHandlers;
using ToffApi.Query.QueryResults;
using ToffApi.Services.DataAccess;

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
        var getConversationQuery = new GetConversationBetweenUsersQuery(command.SenderId, command.ReceiverId);
        var msg = new Message();
        try
        {
            var conversation = await _messageQueryHandler.HandleAsync(getConversationQuery);
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

        var commandResult = new SendDmMessageCommandResult()
        {
            Id = msg.Id,
            SenderId = msg.SenderId,
            SenderName = msg.SenderName,
            Content = msg.Content,
            ConversationId = msg.ConversationId,
            Timestamp = msg.Timestamp
        };

        return commandResult;
    }
}