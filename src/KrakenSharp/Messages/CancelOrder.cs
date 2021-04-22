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
using System.Linq;
using Newtonsoft.Json;

namespace KrakenSharp.Messages
{

    /// <summary>
    /// Cancel order or list of orders.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For every cancelOrder message, an update message 'closeOrderStatus' is sent. 
    /// For multiple orderid in cancelOrder, multiple update messages for 'closeOrderStatus' will be sent.
    /// </para>
    /// <para>
    /// For example, if a cancelOrder request is sent for cancelling three orders[A, B, C], then if two 
    /// update messages for 'closeOrderStatus' are received along with an error such as 'EOrder: Unknown order', 
    /// then it would imply that the third order is not cancelled.The error message could be different based on 
    /// the condition which was not met by the 'cancelOrder' request.
    /// </para>
    /// </remarks>
    /// <seealso cref="Kraken.WebSockets.Messages.PrivateKrakenMessage" />
    public class CancelOrderCommand : PrivateKrakenMessage
    {
        /// <summary>
        /// The event name
        /// </summary>
        public const string EventName = "cancelOrder";

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOrderCommand"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="transactions">The transactions.</param>
        /// <exception cref="ArgumentNullException">transactions</exception>
        public CancelOrderCommand(string token, IEnumerable<string> transactions)
            : base(EventName, token)
        {
            Transactions = transactions?.ToArray() ?? throw new ArgumentNullException(nameof(transactions));
        }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <remarks>
        /// Optional - client originated requestID sent as acknowledgment in the message response
        /// </remarks>
        /// <value>
        /// The request identifier.
        /// </value>
        [JsonProperty("reqid")]
        public int? RequestId { get; set; }

        /// <summary>
        /// Gets the transactions.
        /// </summary>
        /// <remarks>
        /// Array of order IDs to be canceled. These can be user reference IDs.
        /// </remarks>
        /// <value>
        /// The transactions.
        /// </value>
        [JsonProperty("txid")]
        public string[] Transactions { get; }
    }

    /// <summary>
    /// Response event to a <see cref="CancelOrderCommand"/>
    /// </summary>
    /// <seealso cref="KrakenMessage" />
    public sealed class CancelOrderStatusResponse : KrakenMessage
    {
        public const string EventName = "cancelOrderStatus";

        /// <summary>
        /// Prevents a default instance of the <see cref="CancelOrderStatusEvent"/> class from being created.
        /// </summary>
        public CancelOrderStatusResponse()
            : base(EventName)
        { }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <remarks>
        /// Optional - client originated requestID sent as acknowledgment in the message response
        /// </remarks>
        /// <value>
        /// The request identifier.
        /// </value>
        [JsonProperty("reqid")]
        public int? RequestId { get; set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <remarks>
        ///  "ok" or "error"
        /// </remarks>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        [JsonConverter(typeof(StatusConverter))]
        public Status Status { get; private set; }

        /// <summary>
        /// Gets the error message (if unsuccessful)
        /// </summary>
        /// <value>
        /// The error message (if unsuccessful)
        /// </value>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; private set; }


        public static CancelOrderStatusResponse Timeout(int? requestId = null)
        {
            return new CancelOrderStatusResponse()
            {
                RequestId = requestId,
                Status = Status.Error,
                ErrorMessage = "Timeout",
            };
        }

    }
}
