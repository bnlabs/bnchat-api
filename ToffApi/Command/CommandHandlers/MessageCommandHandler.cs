using ToffApi.DtoModels;
using ToffApi.Models;
using ToffApi.Query.Queries;
using ToffApi.Query.QueryResults;
using ToffApi.Services.DataAccess;

namespace ToffApi.Command.CommandHandlers;

public class MessageCommandHandler : CommandHandler
{
    private readonly IMessageDataAccess _messageDataAccess;
    private readonly IUserDataAccess _userDataAccess;

    public MessageCommandHandler(IUserDataAccess userDataAccess, IMessageDataAccess messageDataAccess)
    {
        _userDataAccess = userDataAccess;
        _messageDataAccess = messageDataAccess;
    }
    
}