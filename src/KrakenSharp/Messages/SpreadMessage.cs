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
using Newtonsoft.Json.Linq;

namespace KrakenSharp.Messages
{
    /// <summary>
    /// Spread feed to show best bid and ask price for a currency pair
    /// </summary>
    public sealed class SpreadMessage : EventArgs
    {
        /// <summary>
        /// Gets the ChannelID of pair-spreads subscription.
        /// </summary>
        /// <value>
        /// The ChannelID of pair-spreads subscription.
        /// </value>
        public long ChannelId { get; private set; }

        public string Pair { get; private set; }

        /// <summary>
        /// Gets the Bid price.
        /// </summary>
        /// <value>
        /// The Bid price.
        /// </value>
        public decimal Bid { get; private set; }

        /// <summary>
        /// Gets the Ask price.
        /// </summary>
        /// <value>
        /// The Ask price.
        /// </value>
        public decimal Ask { get; private set; }

        /// <summary>
        /// Gets the Time, seconds since epoch.
        /// </summary>
        /// <value>
        /// The Time, seconds since epoch.
        /// </value>
        public decimal Time { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="SpreadMessage"/> class from being created.
        /// </summary>
        private SpreadMessage()
        {
        }

        /// <summary>
        /// Creates from string.
        /// </summary>
        /// <param name="rawSpreadMessage">The raw spread message.</param>
        /// <returns></returns>
        public static SpreadMessage Parse(string json, SubscriptionStatusResponse subscription)
        {
            var spreadMessage = KrakenDataMessageHelper.EnsureRawMessageIsJArray(json);
            var spreadTokens = spreadMessage[1] as JArray;
            return new SpreadMessage
            {
                ChannelId = (long)spreadMessage.First,
                Pair = subscription.Pair,
                Bid = Convert.ToDecimal(spreadTokens[0]),
                Ask = Convert.ToDecimal(spreadTokens[1]),
                Time = Convert.ToDecimal(spreadTokens[2]),
            };
        }
    }
}
