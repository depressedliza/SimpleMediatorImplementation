using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xm.TestTask.Attr;
using Xm.TestTask.Interfaces;
using Xm.TestTask.Mediators;
using Xm.TestTask.Models;

namespace Xm.TestTask.Extensions;

public static class AddMediatorExtension
{
    private static readonly Type HandlerType = typeof(IHandler<,>);

    public static void AddMediator(this IServiceCollection collection)
    {
        Dictionary<string, HandlerDetails> handlers = new();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var handlerInterface = GetHandlerInterface(type);

            if (handlerInterface is null || !type.IsClass)
            {
                continue;
            }

            var attr = type.GetCustomAttributes(typeof(HandlerType)).FirstOrDefault();
            if (attr is null)
            {
                throw new Exception($"Handler must be provide attribute {nameof(HandlerType)}");
            }

            var key = ((HandlerType)attr).Action;
            var res = handlers.TryAdd(key, new HandlerDetails
            {
                ImplementType = type,
                MsgType = handlerInterface.GetGenericArguments()[0],
                ReturnType = handlerInterface.GetGenericArguments()[1]
            });

            if (!res)
            {
                throw new Exception("Duplicate key value");
            }
            collection.TryAdd(new ServiceDescriptor(type, type, ServiceLifetime.Transient));
        }

        collection.AddSingleton<IMediator>(x => new Mediator(handlers, x.GetRequiredService));
    }
    
    private static Type GetHandlerInterface(Type implementType)
    {
        return implementType
            .GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.IsInterface && x.GetGenericTypeDefinition() == HandlerType);
    }
}
