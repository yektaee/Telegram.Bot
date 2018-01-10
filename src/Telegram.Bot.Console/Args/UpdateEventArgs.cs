using System;
using Telegram.Bot.Types;

namespace Telegram.Bot.Console.Args
{
    /// <summary>
    /// <see cref="EventArgs"/> containing an <see cref="Types.Update"/>
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class UpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the update.
        /// </summary>
        /// <value>
        /// The update.
        /// </value>
        public Update Update { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEventArgs"/> class.
        /// </summary>
        /// <param name="update">The update.</param>
        internal UpdateEventArgs(Update update)
        {
            Update = update;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Exceptions.ApiRequestException"/> to <see cref="ReceiveErrorEventArgs"/>.
        /// </summary>
        /// <param name="e">The <see cref="Exceptions.ApiRequestException"/></param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator UpdateEventArgs(Update u) => new UpdateEventArgs(u);
    }
}
