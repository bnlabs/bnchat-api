using ToffApi.Command.CommandBuses;
using ToffApi.Command.CommandResults;
using ToffApi.Services.CloudFlareR2Service;
using ToffApi.Services.DataAccess;

namespace ToffApi.Command.CommandHandlers;

public class UserCommandHandler : CommandHandler
{
    private readonly IUserDataAccess _userDataAccess;
    private readonly IR2Service _r2Service;

    public UserCommandHandler(IUserDataAccess userDataAccess, IR2Service r2Service)
    {
        _userDataAccess = userDataAccess;
        _r2Service = r2Service;
    }

    public async Task<UpdatePfpCommandResult> HandleAsync(UpdatePfpCommand command)
    {
        var pfpUrl = await _r2Service.UploadObject(command.File);
        var url = await _userDataAccess.UpdateUserPfp(command.UserId, pfpUrl);
        var commandResult = new UpdatePfpCommandResult() { Url = url };
        return commandResult;
    }
}