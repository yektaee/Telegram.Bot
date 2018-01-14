using System;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot.Console.Args;
using Telegram.Bot.Types.Enums;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Console
{
    /// <summary>
    /// A console application interface to use the Telegram Bot API
    /// </summary>
    public interface IUpdater
    {
        /// <summary>
        /// Indicates if receiving updates
        /// </summary>
        bool IsReceiving { get; }

        /// <summary>
        /// The current message offset
        /// </summary>
        int MessageOffset { get; }

        IUpdateDispatcher UpdateDispatcher { get; }

        ITelegramBotClient Client { get; }

        /// <summary>
        /// Timeout for requests
        /// </summary>
        TimeSpan Timeout { get; set; }

        #region Events

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
        void Start(
            int offset = default,
            int limit = default,
            IEnumerable<UpdateType> allowedUpdates = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop update receiving
        /// </summary>
        void Stop();
    }
}
