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
    /// Order book levels. Following the snapshot, level updates will be published.
    /// </summary>
    public sealed class BookMessage : EventArgs
    {
        /// <summary>
        /// Gets the ChannelID of pair-order book levels subscription.
        /// </summary>
        /// <value>
        /// The ChannelID of pair-order book levels subscription.
        /// </value>
        public long ChannelId { get; private set; }

        public string Pair { get; private set; }

        /// <summary>
        /// Gets the Ask array of level updates..
        /// </summary>
        /// <value>
        /// The Ask array of level updates..
        /// </value>
        public PriceLevel[] Asks { get; private set; }

        /// <summary>
        /// Gets the Bid array of level updates..
        /// </summary>
        /// <value>
        /// The Bid array of level updates..
        /// </value>
        public PriceLevel[] Bids { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="BookUpdateMessage"/> class from being created.
        /// </summary>
        private BookMessage()
        {
        }


        /// <summary>
        /// Creates from string in snapshot format.
        /// </summary>
        /// <param name="rawBookSnapshotMessage">The raw book snapshot message.</param>
        /// <returns></returns>
        public static BookMessage ParseSnapshot(string jsonSnapshotMessage, SubscriptionStatusResponse subscription)
        {
            var bookSnapshotTokens = KrakenDataMessageHelper.EnsureRawMessageIsJArray(jsonSnapshotMessage);
            var detailTokens = (JObject)bookSnapshotTokens[1];
            return new BookMessage
            {
                ChannelId = (long)bookSnapshotTokens.First,
                Pair = subscription.Pair,
                Asks = ((JArray)detailTokens["as"]).OfType<JArray>().Select(level => PriceLevel.CreateFromJArray(level)).ToArray(),
                Bids = ((JArray)detailTokens["bs"]).OfType<JArray>().Select(level => PriceLevel.CreateFromJArray(level)).ToArray()
            };
        }

        /// <summary>
        /// Creates from string in update format.
        /// </summary>
        /// <param name="rawBookUpdateMessage">The raw book update message.</param>
        /// <returns></returns>
        public static BookMessage ParseUpdate(string jsonUpdateMessage, SubscriptionStatusResponse subscription)
        {
            var bookUpdateMessage = KrakenDataMessageHelper.EnsureRawMessageIsJArray(jsonUpdateMessage);

            var asks = bookUpdateMessage.Skip(1).OfType<JObject>().FirstOrDefault(x => x.ContainsKey("a"));
            var bids = bookUpdateMessage.Skip(1).OfType<JObject>().FirstOrDefault(x => x.ContainsKey("b"));

            return new BookMessage
            {
                ChannelId = (long)bookUpdateMessage.First,
                Pair = subscription.Pair,
                Asks = asks != null ? asks["a"].OfType<JArray>().Select(level => PriceLevel.CreateFromJArray(level)).ToArray() : null,
                Bids = bids != null ? bids["b"].OfType<JArray>().Select(level => PriceLevel.CreateFromJArray(level)).ToArray() : null,
            };
        }
    }

    /// <summary>
    /// Price level information
    /// </summary>
    public sealed class PriceLevel
    {
        /// <summary>
        /// Gets the Price level.
        /// </summary>
        /// <value>
        /// The Price level.
        /// </value>
        public decimal Price { get; private set; }

        /// <summary>
        /// Gets the Price level volume..
        /// </summary>
        /// <value>
        /// The Price level volume..
        /// </value>
        public decimal Volume { get; private set; }

        /// <summary>
        /// Gets the Price level last updated, seconds since epoch.
        /// </summary>
        /// <value>
        /// The Price level last updated, seconds since epoch.
        /// </value>
        public decimal Timestamp { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="PriceLevel"/> class from being created.
        /// </summary>
        private PriceLevel()
        {
        }

        /// <summary>
        /// Creates from j array.
        /// </summary>
        /// <param name="priceLevelTokens">The price level tokens.</param>
        /// <returns></returns>
        public static PriceLevel CreateFromJArray(JArray priceLevelTokens)
        {
            return new PriceLevel
            {
                Price = Convert.ToDecimal(priceLevelTokens[0]),
                Volume = Convert.ToDecimal(priceLevelTokens[1]),
                Timestamp = Convert.ToDecimal(priceLevelTokens[2]),
            };
        }
    }

}
