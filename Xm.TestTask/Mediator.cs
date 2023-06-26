using System.Text;
using System.Text.Json;
using Xm.TestTask.Interfaces;
using Xm.TestTask.Models;

namespace Xm.TestTask.Mediators;

public class Mediator : IMediator
{
    private readonly IDictionary<string, HandlerDetails> _handlerDetails;
    private readonly Func<Type, object> _serviceResolver;
    
    private const string HandleMethodName = "Handle";

    public Mediator(IDictionary<string, HandlerDetails> handlerDetails, Func<Type, object> serviceResolver)
    {
        _handlerDetails = handlerDetails;
        _serviceResolver = serviceResolver;
    }

    public Task Dispatch(string action, byte[] data)
    {
        var details = GetOrFail(action);
        var handler = _serviceResolver(details.ImplementType);
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
        
        var content = JsonSerializer.Deserialize(strContent, details.MsgType, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

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
        
        var handler = _serviceResolver(details.ImplementType);
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
        
        var content = JsonSerializer.Deserialize(strContent, details.MsgType, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

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
