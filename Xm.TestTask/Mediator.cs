using System.Text;
using Newtonsoft.Json;
using Xm.TestTask.Interfaces;
using Xm.TestTask.Models;

namespace Xm.TestTask;

public class Mediator : IMediator
{
    private readonly IDictionary<string, HandlerDetails> _handlerDetails;
    private readonly IServiceProvider _serviceProvider;
    
    private const string HandleMethodName = "Handle";

    public Mediator(IDictionary<string, HandlerDetails> handlerDetails, IServiceProvider serviceProvider)
    {
        _handlerDetails = handlerDetails;
        _serviceProvider = serviceProvider;
    }

    public Task Dispatch(string action, byte[] data)
    {
        var details = GetOrFail(action);
        var handler = _serviceProvider.GetRequiredService(details.ImplementType);
        var method = handler.GetType().GetMethod(HandleMethodName);

        if (details.MsgType == typeof(byte[]))
        {
            return method!.Invoke(handler, new object[] { data }) as Task;
        }

        var strContent = Encoding.Default.GetString(data);
        if (details.MsgType == typeof(string))
        {
            return method!.Invoke(handler, new object[] { strContent }) as Task;
        }

        var content = JsonConvert.DeserializeObject(strContent, details.MsgType);

        if (content is null)
        {
            throw new Exception($"Content is null after deserialize\nStr:{strContent}\nTo:{details.MsgType}");
        }

        return method!.Invoke(handler, new [] { content }) as Task;
    }

    public Task<TResponse> Dispatch<TResponse>(string action, byte[] data)
    {
        var details = GetOrFail(action);

        if (typeof(TResponse) != details.ReturnType)
        {
            throw new Exception($"Incorrect returning value type for this action\nExpected - {details.ReturnType}");
        }
        
        var handler = _serviceProvider.GetRequiredService(details.ImplementType);
        var method = handler.GetType().GetMethod(HandleMethodName);

        if (details.MsgType == typeof(byte[]))
        {
            return method!.Invoke(handler, new object[] { data }) as Task<TResponse>;
        }

        var strContent = Encoding.Default.GetString(data);
        if (details.MsgType == typeof(string))
        {
            return method!.Invoke(handler, new object[] { strContent }) as Task<TResponse>;
        }

        var content = JsonConvert.DeserializeObject(strContent, details.MsgType);

        if (content is null)
        {
            throw new Exception($"Content is null after deserialize\nStr:{strContent}\nTo:{details.MsgType}");
        }

        return method!.Invoke(handler, new [] { content }) as Task<TResponse>;
    }
    
    private HandlerDetails GetOrFail(string action)
    {
        if (!_handlerDetails.ContainsKey(action))
        {
            throw new Exception("Handler for this action not registered");
        }
        
        return _handlerDetails[action];
    }
}
