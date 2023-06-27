using Xm.TestTask.Attr;
using Xm.TestTask.Interfaces;
using Xm.TestTask.Models;

namespace Xm.TestTask.Handlers;

[HandlerType("SetAction")]
public class SetActionHandler : IHandler<ActionModel, object>
{
    private readonly ILogger<SetActionHandler> _logger;

    public SetActionHandler(ILogger<SetActionHandler> logger)
    {
        _logger = logger;
    }

    public Task<object> Handle(ActionModel data)
    {
        _logger.LogWarning("New action registered");
        return Task.FromResult(new object());
    }
}
