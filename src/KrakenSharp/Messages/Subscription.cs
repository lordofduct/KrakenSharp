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

    public class SubscribeCommand : KrakenMessage
    {
        /// <summary>
        /// The subscribe message key.
        /// </summary>
        internal const string SubscribeMessageKey = "subscribe";

        /// <summary>
        /// Gets the pairs.
        /// </summary>
        /// <value>The pairs.</value>
        [JsonProperty("pair", Order = 2)]
        public IEnumerable<string> Pairs { get; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        [JsonProperty("subscription", Order = 3)]
        public SubscriptionOptions Options { get; }

        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        [JsonProperty("reqid", Order = 1)]
        public int? RequestId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kraken.WebSockets.Messages.Subscribe"/> class.
        /// </summary>
        /// <param name="pairs">Pairs.</param>
        /// <param name="options">Options.</param>
        /// <param name="requestId">Requst identifier.</param>
        public SubscribeCommand(IEnumerable<string> pairs, SubscriptionOptions options, int? requestId = null)
            : base(SubscribeMessageKey)
        {
            Pairs = pairs;
            Options = options ?? throw new ArgumentNullException(nameof(options));
            RequestId = requestId;
        }
    }

    /// <summary>
    /// Subscribe options.
    /// </summary>
    public class SubscriptionOptions
    {

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        [JsonProperty("interval")]
        public int? Interval { get; set; }

        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>The depth.</value>
        [JsonProperty("depth")]
        public int? Depth { get; set; }

        /// <summary>
        /// Gets the authentication token for private subscriptions.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        [JsonProperty("token")]
        public string Token { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kraken.WebSockets.Messages.SubscribeOptions" /> class.
        /// </summary>
        /// <param name="name">Name. Valid values are: ticker|ohlc|trade|book|spread|ownTrades|*</param>
        /// <param name="token">The authentication token for private subscriptions.</param>
        /// <exception cref="ArgumentNullException">name</exception>
        /// <exception cref="ArgumentOutOfRangeException">name - Allowed values: ticker|ohlc|trade|book|spread|ownTrades|*</exception>
        public SubscriptionOptions(string name, string token = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            //if (SubscribeOptionNames.AllowedNames.All(x => x != name))
            //{
            //    throw new ArgumentOutOfRangeException(nameof(name), name,
            //        $"Allowed values: {string.Join(",", SubscribeOptionNames.AllowedNames)}");
            //}

            Token = token;
        }
    }

    /// <summary>
    /// Subscription status.
    /// </summary>
    public class SubscriptionStatusResponse : KrakenMessage
    {
        public const string EventName = "subscriptionStatus";

        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        /// <value>The channel identifier.</value>
        [JsonProperty("channelID")]
        public long ChannelId { get; internal set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        [JsonProperty("status")]
        [JsonConverter(typeof(SubscriptionStatusConvert))]
        public SubscriptionStatus Status { get; internal set; }

        /// <summary>
        /// Gets the pair.
        /// </summary>
        /// <value>The pair.</value>
        [JsonProperty("pair")]
        public string Pair { get; internal set; }

        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
        [JsonProperty("reqid")]
        public int? RequestId { get; internal set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The error message.</value>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// Gets the subscription.
        /// </summary>
        /// <value>
        /// The subscription.
        /// </value>
        [JsonProperty("subscription")]
        public SubscriptionOptions Subscription { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kraken.WebSockets.Subscription"/> class.
        /// </summary>
        public SubscriptionStatusResponse()
            : base(EventName)
        {
        }
    }

    /// <summary>
    /// This class represents the "umsubscribe" message to be send to the 
    /// websocket API
    /// </summary>
    /// <seealso cref="KrakenMessage" />
    public sealed class Unsubscribe : KrakenMessage
    {
        internal const string EventName = "unsubscribe";

        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
        [JsonProperty("channelID")]
        public long ChannelId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Unsubscribe"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        public Unsubscribe(long channelId)
            : base(EventName)
        {
            ChannelId = channelId;
        }
    }

    public static class SubscribeOptionNames
    {
        /// <summary>
        /// All information.
        /// </summary>
        public const string All = "*";

        /// <summary>
        /// Ticker information includes best ask and best bid prices, 
        /// 24hr volume, last trade price, volume weighted average price, 
        /// etc for a given currency pair. A ticker message is published 
        /// every time a trade or a group of trade happens.
        /// </summary>
        public const string Ticker = "ticker";

        /// <summary>
        /// Open High Low Close (Candle) feed for a currency pair 
        /// and interval period.
        /// </summary>
        public const string OHLC = "ohlc";

        /// <summary>
        /// Open High Low Close (Candle) feed for a currency pair and interval period.
        /// </summary>
        public const string Trade = "trade";

        /// <summary>
        /// Order book levels. On subscription, a snapshot will be published at 
        /// the specified depth, following the snapshot, level updates will be 
        /// published.
        /// </summary>
        public const string Book = "book";

        /// <summary>
        /// Spread feed to show best bid and ask price for a currency pair
        /// </summary>
        public const string Spread = "spread";

        /// <summary>
        /// Own trades, on subscription last 50 trades for the user will be sent, followed by new trades.
        /// </summary>
        public const string OwnTrades = "ownTrades";

        /// <summary>
        /// Open orders, initial snapshot will provide list of all open orders and then any updates to
        /// the open orders list will be sent.
        /// </summary>
        public const string OpenOrders = "openOrders";

        internal static IEnumerable<string> AllowedNames
        {
            get
            {
                yield return All;

                yield return Ticker;
                yield return OHLC;
                yield return Trade;
                yield return Book;
                yield return Spread;
                yield return OwnTrades;
                yield return OpenOrders;
            }
        }

        internal static IEnumerable<string> PrivateNames
        {
            get
            {
                yield return OwnTrades;
                yield return OpenOrders;
            }
        }
    }

}
