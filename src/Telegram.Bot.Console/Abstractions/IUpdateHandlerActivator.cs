using System;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Console
{
    public interface IUpdateHandlerActivator
    {
        IUpdateHandler ActivateHandler(Type handlerType);

        IUpdateHandlerActivatorScope BeginScope();
    }
}
