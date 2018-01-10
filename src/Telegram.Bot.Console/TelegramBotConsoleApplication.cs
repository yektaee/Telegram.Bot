using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Console.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Console
{
    /// <summary>
    /// A console application to use the Telegram Bot API
    /// </summary>
    public class TelegramBotConsoleApplication : ITelegramBotConsoleApplication
    {
        private CancellationTokenSource _receivingCancellationTokenSource;

        /// <inheritdoc />
        public bool IsReceiving { get; private set; }

        /// <inheritdoc />
        public int MessageOffset { get; private set; }

        public TimeSpan Timeout
        {
            get => Client.Timeout;
            set => Client.Timeout = value;
        }

        public ITelegramBotClient Client { get; }

        #region Events

        /// <summary>
        /// Raises the <see cref="OnUpdate" />, <see cref="OnMessage"/>, <see cref="OnInlineQuery"/>, <see cref="OnInlineResultChosen"/> and <see cref="OnCallbackQuery"/> events.
        /// </summary>
        /// <param name="e">The <see cref="UpdateEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUpdateReceived(UpdateEventArgs e)
        {
            OnUpdate?.Invoke(this, e);

            switch (e.Update.Type)
            {
                case UpdateType.Message:
                    OnMessage?.Invoke(this, e);
                    break;

                case UpdateType.InlineQuery:
                    OnInlineQuery?.Invoke(this, e);
                    break;

                case UpdateType.ChosenInlineResult:
                    OnInlineResultChosen?.Invoke(this, e);
                    break;

                case UpdateType.CallbackQuery:
                    OnCallbackQuery?.Invoke(this, e);
                    break;

                case UpdateType.EditedMessage:
                    OnMessageEdited?.Invoke(this, e);
                    break;
            }
        }

        /// <inheritdoc />
        public event EventHandler<UpdateEventArgs> OnUpdate;

        /// <inheritdoc />
        public event EventHandler<MessageEventArgs> OnMessage;

        /// <inheritdoc />
        public event EventHandler<MessageEventArgs> OnMessageEdited;

        /// <inheritdoc />
        public event EventHandler<InlineQueryEventArgs> OnInlineQuery;

        /// <inheritdoc />
        public event EventHandler<ChosenInlineResultEventArgs> OnInlineResultChosen;

        /// <inheritdoc />
        public event EventHandler<CallbackQueryEventArgs> OnCallbackQuery;

        /// <inheritdoc />
        public event EventHandler<ReceiveErrorEventArgs> OnReceiveError;

        /// <inheritdoc />
        public event EventHandler<ReceiveGeneralErrorEventArgs> OnReceiveGeneralError;

        #endregion

        /// <summary>
        /// Create a new <see cref="TelegramBotConsoleApplication"/> instance.
        /// </summary>
        /// <param name="token">API token</param>
        /// <param name="httpClient">A custom <see cref="HttpClient"/></param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="token"/> format is invalid</exception>
        public TelegramBotConsoleApplication(string token, HttpClient httpClient = default)
            : this(new TelegramBotClient(token, httpClient))
        { }

        /// <summary>
        /// Create a new <see cref="TelegramBotConsoleApplication"/> instance.
        /// </summary>
        /// <param name="client"><see cref="ITelegramBotClient"/> instance</param>
        public TelegramBotConsoleApplication(ITelegramBotClient client)
        {
            Client = client;
        }

        /// <inheritdoc />
        public void StartReceiving(
            int offset = default,
            int limit = default,
            IEnumerable<UpdateType> allowedUpdates = default,
            CancellationToken cancellationToken = default)
        {
            _receivingCancellationTokenSource = new CancellationTokenSource();
            cancellationToken.Register(() => _receivingCancellationTokenSource.Cancel());

#pragma warning disable 4014
            ReceiveAsync(
                offset: offset,
                limit: limit,
                allowedUpdates: allowedUpdates,
                cancellationToken:_receivingCancellationTokenSource.Token);
#pragma warning restore 4014
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
                var timeout = Convert.ToInt32(Client.Timeout);

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
                        OnUpdateReceived(update);
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
        public void StopReceiving()
        {
            _receivingCancellationTokenSource.Cancel();
        }
    }
}
