using System;

namespace Telegram.Bot.Console
{
    public interface IUpdateHandlerActivatorScope : IDisposable
    {
        IUpdateHandler Resolve(Type type);
    }
}
