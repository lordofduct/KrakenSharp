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
    /// Own trades, on subscription last 50 trades for the user will be sent, followed by new trades.
    /// </summary>
    public sealed class OwnTradesMessage : EventArgs
    {
        private OwnTradesMessage()
        { }

        /// <summary>
        /// Creates the <see cref="OwnTradesMessage"/> from it's from string representation coming from the api.
        /// </summary>
        /// <param name="rawMessage">The raw message.</param>
        /// <returns></returns>
        public static OwnTradesMessage CreateFromJArray(JArray message)
        {
            var trades = message[0]
                .Select(x => (x as JObject)?.ToObject<Dictionary<string, JObject>>())
                .Select(items => items != null && items.Count == 1 ?
                    new
                    {
                        TradeId = items.Keys.ToArray()[0],
                        TradeObject = items.Values.ToArray()[0]
                    } :
                    null)
                .Where(x => x != null)
                .Select(x => TradeObject.CreateFromJObject(x.TradeId, x.TradeObject))
                .ToList();

            return new OwnTradesMessage
            {
                ChannelName = message[1].ToString(),
                Trades = trades,
                Sequence = message[2].Value<long>("sequence")
            };

        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string ChannelName { get; private set; }

        /// <summary>
        /// Gets the list of trades.
        /// </summary>
        /// <value>
        /// The list of trades.
        /// </value>
        public List<TradeObject> Trades { get; private set; } = new List<TradeObject>();

        public long Sequence { get; set; }
    }

    /// <summary>
    /// Trade object
    /// </summary>
    public sealed class TradeObject
    {
        /// <summary>
        /// Gets the trade identifier.
        /// </summary>
        /// <value>
        /// The trade identifier.
        /// </value>
        public string TradeId { get; private set; }

        /// <summary>
        /// Gets the order responsible for execution of trade.
        /// </summary>
        /// <value>
        /// The order responsible for execution of trade.
        /// </value>
        public string OrderTxId { get; private set; }

        /// <summary>
        /// Gets the Position trade id.
        /// </summary>
        /// <value>
        /// The Position trade id.
        /// </value>
        public string PosTxId { get; private set; }

        /// <summary>
        /// Gets the Asset pair.
        /// </summary>
        /// <value>
        /// The Asset pair.
        /// </value>
        public string Pair { get; private set; }

        /// <summary>
        /// Gets the unix timestamp of trade.
        /// </summary>
        /// <value>
        /// The unix timestamp of trade.
        /// </value>
        public decimal Time { get; private set; }

        /// <summary>
        /// Gets the type of order (buy/sell).
        /// </summary>
        /// <value>
        /// The type of order (buy/sell).
        /// </value>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the type of the order.
        /// </summary>
        /// <value>
        /// The type of the order.
        /// </value>
        public string OrderType { get; private set; }

        /// <summary>
        /// Gets the average price order was executed at (quote currency).
        /// </summary>
        /// <value>
        /// The average price order was executed at (quote currency).
        /// </value>
        public decimal Price { get; private set; }

        /// <summary>
        /// Gets the total cost of order (quote currency).
        /// </summary>
        /// <value>
        /// The total cost of order (quote currency).
        /// </value>
        public decimal Cost { get; private set; }

        /// <summary>
        /// Gets the total fee (quote currency).
        /// </summary>
        /// <value>
        /// The total fee (quote currency).
        /// </value>
        public decimal Fee { get; private set; }

        /// <summary>
        /// Gets the volume (base currency).
        /// </summary>
        /// <value>
        /// The volume (base currency).
        /// </value>
        public decimal Volume { get; private set; }

        /// <summary>
        /// Gets the initial margin (quote currency).
        /// </summary>
        /// <value>
        /// The initial margin (quote currency).
        /// </value>
        public decimal Margin { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="TradeObject"/> class from being created.
        /// </summary>
        private TradeObject() { }

        public static TradeObject CreateFromJObject(string tradeId, JObject jObject)
        {
            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            return new TradeObject()
            {
                TradeId = tradeId ?? throw new ArgumentNullException(nameof(tradeId)),
                OrderTxId = jObject.Value<string>("ordertxid"),
                PosTxId = jObject.Value<string>("postxid"),
                Pair = jObject.Value<string>("pair"),
                Time = jObject.Value<decimal>("time"),
                Type = jObject.Value<string>("type"),
                OrderType = jObject.Value<string>("ordertype"),
                Price = jObject.Value<decimal>("price"),
                Cost = jObject.Value<decimal>("cost"),
                Fee = jObject.Value<decimal>("fee"),
                Volume = jObject.Value<decimal>("vol"),
                Margin = jObject.Value<decimal>("margin"),
            };
        }
    }
}
