using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Console.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Console
{
    public class Updater : IUpdater
    {
        private CancellationTokenSource _receivingCancellationTokenSource;
        private Task _receivingTask;

        /// <inheritdoc />
        public bool IsReceiving { get; private set; }

        /// <inheritdoc />
        public int MessageOffset { get; private set; }

        public IUpdateDispatcher UpdateDispatcher { get; }

        public TimeSpan Timeout
        {
            get => Client.Timeout;
            set => Client.Timeout = value;
        }

        public ITelegramBotClient Client { get; }

        #region Events

        /// <inheritdoc />
        public event EventHandler<ReceiveErrorEventArgs> OnReceiveError;

        /// <inheritdoc />
        public event EventHandler<ReceiveGeneralErrorEventArgs> OnReceiveGeneralError;

        #endregion

        /// <summary>
        /// Create a new <see cref="Updater"/> instance.
        /// </summary>
        /// <param name="client"><see cref="ITelegramBotClient"/> instance</param>
        /// <param name="updateDispatcher"></param>
        public Updater(ITelegramBotClient client, IUpdateDispatcher updateDispatcher)
        {
            Client = client;
            UpdateDispatcher = updateDispatcher;
        }

        /// <inheritdoc />
        public void Start(
            int offset = default,
            int limit = default,
            IEnumerable<UpdateType> allowedUpdates = default,
            CancellationToken cancellationToken = default)
        {
            _receivingCancellationTokenSource = new CancellationTokenSource();
            cancellationToken.Register(() => _receivingCancellationTokenSource.Cancel());

            _receivingTask = ReceiveAsync(
                offset: offset,
                limit: limit,
                allowedUpdates: allowedUpdates,
                cancellationToken:_receivingCancellationTokenSource.Token);
        }

        private async Task ReceiveAsync(
            int offset = default,
            int limit = default,
            IEnumerable<UpdateType> allowedUpdates = default,
            CancellationToken cancellationToken = default)
        {
            IsReceiving = true;

            if (offset != default)
            {
                MessageOffset = offset;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                var timeout = Convert.ToInt32(Client.Timeout.TotalSeconds);

                try
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    var updates = await Client.GetUpdatesAsync(
                            offset: MessageOffset,
                            limit: limit,
                            timeout: timeout,
                            allowedUpdates: allowedUpdates,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    foreach (var update in updates)
                    {
                        MessageOffset = update.Id + 1;
                        UpdateDispatcher.Enqueue(update);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (ApiRequestException apiException)
                {
                    OnReceiveError?.Invoke(this, apiException);
                }
                catch (Exception generalException)
                {
                    OnReceiveGeneralError?.Invoke(this, generalException);
                }
            }

            IsReceiving = false;
        }

        /// <inheritdoc />
        public void Stop()
        {
            _receivingCancellationTokenSource.Cancel();
        }
    }
}
