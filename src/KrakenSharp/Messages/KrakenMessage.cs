/*
 * Significant portions of this code was based on code authored by Maik (github user m4cx)
 * in the repository found at:
 * https://github.com/m4cx/kraken-wsapi-dotnet
 * 
 * This code was modifed/refactored based under the permissiveness of the MIT License:
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
 * and associated documentation files (the "Software"), to deal in the Software without restriction, 
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrakenSharp.Messages
{

    /// <summary>
    /// Kraken message event arguments.
    /// </summary>
    public class RawKrakenMessage : EventArgs
    {
        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <value>The event.</value>
        /// <remarks>Can be null or empty string</remarks>
        public string Event { get; }

        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
        public long? ChannelId { get; }

        /// <summary>
        /// Gets the raw content of the message.
        /// </summary>
        /// <value>The content of the raw.</value>
        /// <remarks>Can be null or empty string</remarks>
        public string RawContent { get; }

        /// <summary>
        /// Optional reference to the unwrapped object if it has been unwrapped.
        /// </summary>
        public object UnwrappedObject { get; set; }

        public RawKrakenMessage(string @event, string rawContent, long? channelId = null)
        {
            Event = @event;
            RawContent = rawContent;
            ChannelId = channelId;
        }

    }

    public abstract class KrakenMessage : EventArgs
    {
        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <value>The event.</value>
        [JsonProperty("event", Order = 0)]
        public string Event { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kraken.WebSockets.KrakenMessage"/> class.
        /// </summary>
        /// <param name="eventId">Event type.</param>
        protected KrakenMessage(string eventId)
        {
            Event = eventId;
        }
    }

    /// <summary>
    /// Base class for private messages sent to the Kraken Websockets API
    /// </summary>
    /// <seealso cref="Kraken.WebSockets.Messages.KrakenMessage" />
    public abstract class PrivateKrakenMessage : KrakenMessage
    {
        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        /// <value>
        /// The authentication token.
        /// </value>
        [JsonProperty("token")]
        public string Token { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateKrakenMessage"/> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="token">The token.</param>
        /// <exception cref="ArgumentNullException">If token is null</exception>
        protected PrivateKrakenMessage(string eventType, string token)
            : base(eventType)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }

}
