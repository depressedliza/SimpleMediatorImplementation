using Xm.TestTask.Attr;
using Xm.TestTask.DTO;
using Xm.TestTask.Interfaces;

namespace Xm.TestTask.Handlers;

[HandlerType("CreateUser")]
public class CreateUserHandler : IHandler<CreateUserDto, Guid>
{
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(ILogger<CreateUserHandler> logger)
    {
        _logger = logger;
    }

    public Task<Guid> Handle(CreateUserDto data)
    {
        _logger.LogWarning("Saving new user..");
        return Task.FromResult(Guid.NewGuid());
    }
}
