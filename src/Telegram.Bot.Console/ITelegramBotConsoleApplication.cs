using System;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot.Console.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Console
{
    /// <summary>
    /// A console application interface to use the Telegram Bot API
    /// </summary>
    public interface ITelegramBotConsoleApplication
    {
        /// <summary>
        /// Indicates if receiving updates
        /// </summary>
        bool IsReceiving { get; }

        /// <summary>
        /// The current message offset
        /// </summary>
        int MessageOffset { get; }

        /// <summary>
        /// Timeout for requests
        /// </summary>
        TimeSpan Timeout { get; set; }

        #region Events

        /// <summary>
        /// Occurs when an <see cref="Update"/> is received.
        /// </summary>
        event EventHandler<UpdateEventArgs> OnUpdate;

        /// <summary>
        /// Occurs when a <see cref="Message"/> is received.
        /// </summary>
        event EventHandler<MessageEventArgs> OnMessage;

        /// <summary>
        /// Occurs when <see cref="Message"/> was edited.
        /// </summary>
        event EventHandler<MessageEventArgs> OnMessageEdited;

        /// <summary>
        /// Occurs when an <see cref="InlineQuery"/> is received.
        /// </summary>
        event EventHandler<InlineQueryEventArgs> OnInlineQuery;

        /// <summary>
        /// Occurs when a <see cref="ChosenInlineResult"/> is received.
        /// </summary>
        event EventHandler<ChosenInlineResultEventArgs> OnInlineResultChosen;

        /// <summary>
        /// Occurs when an <see cref="CallbackQuery"/> is received
        /// </summary>
        event EventHandler<CallbackQueryEventArgs> OnCallbackQuery;

        /// <summary>
        /// Occurs when an error occurs during the background update pooling.
        /// </summary>
        event EventHandler<ReceiveErrorEventArgs> OnReceiveError;

        /// <summary>
        /// Occurs when an error occurs during the background update pooling.
        /// </summary>
        event EventHandler<ReceiveGeneralErrorEventArgs> OnReceiveGeneralError;

        #endregion Events

        /// <summary>
        /// Start update receiving
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="allowedUpdates">List the types of updates you want your bot to receive.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="Exceptions.ApiRequestException"> Thrown if token is invalid</exception>
        void StartReceiving(
            int offset = default,
            int limit = default,
            IEnumerable<UpdateType> allowedUpdates = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop update receiving
        /// </summary>
        void StopReceiving();
    }
}
