using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Console
{
    public class UpdateDispatcher : IUpdateDispatcher
    {
        private readonly IList<Type> _handlers = new List<Type>();
        private readonly ITelegramBotClient _client;
        private readonly BlockingCollection<Update> _updateQueue = new BlockingCollection<Update>();
        private readonly Task _processTask;
        private readonly IUpdateHandlerActivator _updateHandlerActivator;

        public UpdateDispatcher(ITelegramBotClient client)
            : this(client, new DefaultUpdateHandlerActivator())
        { }

        // ReSharper disable once MemberCanBePrivate.Global
        public UpdateDispatcher(ITelegramBotClient client, IUpdateHandlerActivator updateHandlerActivator)
        {
            _client = client;
            _updateHandlerActivator = updateHandlerActivator;
            _processTask = Task.Factory.StartNew(
                ProcessUpdateQueue,
                this,
                TaskCreationOptions.LongRunning);
        }

        public void AddUpdateHandler<THandler>() where THandler : IUpdateHandler
        {
            _handlers.Add(typeof(THandler));
        }

        private void ProcessUpdateQueue()
        {
            foreach (var update in _updateQueue.GetConsumingEnumerable())
            {
                Task.Run(async () => await Dispatch(update));
            }
        }

        private async Task Dispatch(Update update)
        {
            foreach (var handlerType in _handlers)
            {
                using (var scope = _updateHandlerActivator.BeginScope())
                {
                    try
                    {
                        var handler = scope.Resolve(handlerType);
                        if (await handler.CanHandleUpdateAsync(update))
                        {
                            await handler.HandleUpdateAsync(update, _client);
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        private static void ProcessUpdateQueue(object state)
        {
            var telegramLogger = (UpdateDispatcher)state;

            telegramLogger.ProcessUpdateQueue();
        }

        public void Enqueue(Update update)
        {
            if (!_updateQueue.IsAddingCompleted)
            {
                try
                {
                    _updateQueue.Add(update);
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
            }
        }

        public void Dispose()
        {
            try
            {
                _updateQueue?.CompleteAdding();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
