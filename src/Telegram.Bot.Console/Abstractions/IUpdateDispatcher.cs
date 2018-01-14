using System;
using Telegram.Bot.Types;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Console
{
    public interface IUpdateDispatcher : IDisposable
    {
        void AddUpdateHandler<THandler>() where THandler : IUpdateHandler;

        void Enqueue(Update update);
    }
}
