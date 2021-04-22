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
using Newtonsoft.Json.Linq;

namespace KrakenSharp.Messages
{
    /// <summary>
    /// Trade feed for a currency pair.
    /// </summary>
    public sealed class TradeMessage : EventArgs
    {
        /// <summary>
        /// Gets the ChannelID of pair-trade subscription.
        /// </summary>
        /// <value>
        /// The ChannelID of pair-trade subscription.
        /// </value>
        public long ChannelId { get; private set; }

        public string Pair { get; private set; }

        /// <summary>
        /// Gets the Array of trades.
        /// </summary>
        /// <value>
        /// The Array of trades.
        /// </value>
        public TradeValues[] Trades { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="TradeMessage"/> class from being created.
        /// </summary>
        private TradeMessage()
        {
        }

        /// <summary>
        /// Creates from string.
        /// </summary>
        /// <param name="rawMessage">The raw message.</param>
        /// <returns></returns>
        public static TradeMessage Parse(string json, SubscriptionStatusResponse subscription)
        {
            var message = KrakenDataMessageHelper.EnsureRawMessageIsJArray(json);
            return new TradeMessage()
            {
                ChannelId = (long)message.First,
                Pair = subscription.Pair,
                Trades = ((JArray)message[1]).OfType<JArray>().Select(tradeJArray => TradeValues.CreateFromJArray(tradeJArray)).ToArray()
            };
        }
    }

    /// <summary>
    /// Trade values
    /// </summary>
    public sealed class TradeValues
    {
        /// <summary>
        /// Gets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public decimal Price { get; private set; }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        /// <value>
        /// The volume.
        /// </value>
        public decimal Volume { get; private set; }

        /// <summary>
        /// Gets the Time, seconds since epoch.
        /// </summary>
        /// <value>
        /// The Time, seconds since epoch.
        /// </value>
        public decimal Time { get; private set; }

        /// <summary>
        /// Gets the Triggering order side (buy/sell), values: b|s.
        /// </summary>
        /// <value>
        /// The Triggering order side (buy/sell), values: b|s.
        /// </value>
        public string Side { get; private set; }

        /// <summary>
        /// Gets the Triggering order type (market/limit), values: m|l.
        /// </summary>
        /// <value>
        /// The Triggering order type (market/limit), values: m|l.
        /// </value>
        public string OrderType { get; private set; }

        /// <summary>
        /// Gets the Miscellaneous.
        /// </summary>
        /// <value>
        /// The Miscellaneous.
        /// </value>
        public string Misc { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="TradeValues"/> class from being created.
        /// </summary>
        private TradeValues()
        {
        }

        public static TradeValues CreateFromJArray(JArray tradeValueTokens)
        {
            return new TradeValues
            {
                Price = Convert.ToDecimal(tradeValueTokens[0]),
                Volume = Convert.ToDecimal(tradeValueTokens[1]),
                Time = Convert.ToDecimal(tradeValueTokens[2]),
                Side = Convert.ToString(tradeValueTokens[3]),
                OrderType = Convert.ToString(tradeValueTokens[4]),
                Misc = Convert.ToString(tradeValueTokens[5]),
            };
        }
    }
}
