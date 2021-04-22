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
    /// Request. Add new order.
    /// </summary>
    /// <seealso cref="PrivateKrakenMessage" />
    public sealed class AddOrderCommand : PrivateKrakenMessage
    {
        public const string EventName = "addOrder";

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOrderCommand"/> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="token">The token.</param>
        public AddOrderCommand(string token, OrderType orderType, Side type, string pair, decimal volume)
            : base(EventName, token)
        {
            OrderType = orderType;
            Type = type;
            Pair = pair ?? throw new ArgumentNullException(nameof(pair));
            Volume = volume;
        }

        /// <summary>
        /// Gets the type of the order.
        /// </summary>
        /// <value>
        /// The type of the order.
        /// </value>
        [JsonProperty("ordertype")]
        [JsonConverter(typeof(OrderTypeConverter))]
        public OrderType OrderType { get; }

        /// <summary>
        /// Gets the Side (buy or sell).
        /// </summary>
        /// <value>
        /// The Side (buy or sell).
        /// </value>
        [JsonConverter(typeof(SideConverter))]
        public Side Type { get; }

        /// <summary>
        /// Gets the Currency pair.
        /// </summary>
        /// <value>
        /// The Currency pair.
        /// </value>
        public string Pair { get; }

        /// <summary>
        /// Gets the Order volume in lots.
        /// </summary>
        /// <value>
        /// The Order volume in lots.
        /// </value>
        [JsonProperty("volume")]
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal Volume { get; }

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
        /// Gets or sets the order price.
        /// </summary>
        /// <remarks>
        /// Optional dependent on order type - order price
        /// </remarks>
        /// <value>
        /// The order price.
        /// </value>
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? Price { get; set; }

        /// <summary>
        /// Gets or sets the secondary price.
        /// </summary>
        /// <remarks>
        /// Optional dependent on order type - order secondary price
        /// </remarks>
        /// <value>
        /// The secondary price.
        /// </value>
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? Price2 { get; set; }

        /// <summary>
        /// Gets or sets the amount of leverage desired.
        /// </summary>
        /// <remarks>
        /// default = none
        /// </remarks>
        /// <value>
        /// The amount of leverage desired.
        /// </value>
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? Leverage { get; set; }

        /// <summary>
        /// Gets or sets the oflags.
        /// </summary>
        /// <remarks>
        /// comma delimited list of order flags:
        /// <ul>
        ///     <li><b>viqc</b> = volume in quote currency (not currently available)</li>
        ///     <li>fcib = prefer fee in base currency</li>
        ///     <li>fciq = prefer fee in quote currency</li>
        ///     <li>nompp = no market price protection</li>
        ///     <li>post = post only order (available when ordertype = limit)</li>
        /// </ul>
        /// </remarks>
        /// <value>
        /// The oflags.
        /// </value>
        public string Oflags { get; set; }

        /// <summary>
        /// Gets or sets the scheduled start time. 
        /// </summary>
        /// <remarks>
        ///     0 = now (default) 
        ///     +<n> = schedule start time <n> seconds from now 
        ///     <n> = unix timestamp of start time.
        /// </remarks>
        /// <value>
        /// The scheduled start time.
        /// </value>
        public string Starttm { get; set; }

        /// <summary>
        /// Gets or sets the expiration time. 
        /// </summary>
        /// <remarks>
        /// 0 = no expiration (default) 
        /// +<n> = expire <n> seconds from now 
        /// <n> = unix timestamp of expiration time.
        /// </remarks>
        /// <value>
        /// The expiration time.
        /// </value>
        public string Expiretm { get; set; }

        /// <summary>
        /// Gets or sets the user reference ID.
        /// </summary>
        /// <remarks>
        /// should be an integer in quotes
        /// </remarks>
        /// <value>
        /// The user reference ID.
        /// </value>
        public string Userref { get; set; }

        /// <summary>
        /// Gets or sets the validate.
        /// </summary>
        /// <remarks>
        /// validate inputs only; do not submit order (not currently available)
        /// </remarks>
        /// <value>
        /// The validate.
        /// </value>
        public string Validate { get; set; }

        /// <summary>
        /// Gets or sets the type of the close order.
        /// </summary>
        /// <value>
        /// The type of the close order.
        /// </value>
        [JsonProperty("close[ordertype]")]
        [JsonConverter(typeof(OrderTypeConverter))]
        public OrderType? CloseOrderType { get; set; }

        /// <summary>
        /// Gets or sets the close price.
        /// </summary>
        /// <value>
        /// The close price.
        /// </value>
        [JsonProperty("close[price]")]
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? ClosePrice { get; set; }

        /// <summary>
        /// Gets or sets the close order secondary price.
        /// </summary>
        /// <value>
        /// The close order secondary price.
        /// </value>
        [JsonProperty("close[price2]")]
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? ClosePrice2 { get; set; }

        /// <summary>
        /// Gets or sets the trading agreement.
        /// </summary>
        /// <remarks>
        /// should be set to "agree" by German residents in order to signify acceptance of the terms of the 
        /// <a href="https://support.kraken.com/hc/en-us/articles/360036157952">Kraken Trading Agreement</a>.
        /// </remarks>
        /// <value>
        /// The trading agreement.
        /// </value>
        [JsonProperty("trading_agreement")]
        public string TradingAgreement { get; set; }
    }

    /// <summary>
    /// Response payload of a <see cref="AddOrderCommand"/>
    /// </summary>
    /// <seealso cref="KrakenMessage" />
    public sealed class AddOrderStatusResponse : KrakenMessage
    {
        /// <summary>
        /// The unique event name.
        /// </summary>
        public const string EventName = "addOrderStatus";

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOrderStatusEvent"/> class.
        /// </summary>
        /// <param name="eventType">Event type.</param>
        public AddOrderStatusResponse() : base(EventName)
        { }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <remarks>
        /// client originated requestID sent as acknowledgment in the message response
        /// </remarks>
        /// <value>
        /// The request identifier.
        /// </value>
        [JsonProperty("reqid")]
        public int? RequestId { get; set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        [JsonConverter(typeof(StatusConverter))]
        public Status Status { get; private set; }

        /// <summary>
        /// Gets the order ID (if successful).
        /// </summary>
        /// <value>
        /// The order ID (if successful).
        /// </value>
        [JsonProperty("txid")]
        public string OrderId { get; private set; }

        /// <summary>
        /// Gets the order description info (if successful).
        /// </summary>
        /// <value>
        /// The order description info (if successful).
        /// </value>
        [JsonProperty("descr")]
        public string Description { get; private set; }

        /// <summary>
        /// Gets the error message. (if unsuccessful)
        /// </summary>
        /// <value>
        /// The error message. (if unsuccessful)
        /// </value>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; private set; }


        public static AddOrderStatusResponse Timeout(int? requestId = null)
        {
            return new AddOrderStatusResponse()
            {
                RequestId = requestId,
                Status = Status.Error,
                OrderId = string.Empty,
                Description = string.Empty,
                ErrorMessage = "Timeout",
            };
        }

    }
}
