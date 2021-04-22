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
    /// Open orders. Feed to show all the open orders belonging to the user authenticated API key.
    /// </summary>
    /// <remarks>
    /// Initial snapshot will provide list of all open orders and then any updates to the open orders
    /// list will be sent. For status change updates, such as 'closed', the fields <code>orderid</code>
    /// and <code>status</code> will be present in the payload.
    /// </remarks>
    public class OpenOrdersMessage : EventArgs
    {
        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        /// <value>
        /// The name of the channel.
        /// </value>
        public string ChannelName { get; private set; }

        /// <summary>
        /// Gets the list of open orders.
        /// </summary>
        public IList<Order> Orders { get; private set; }

        public long Sequence { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="OpenOrdersMessage"/> class from being created.
        /// </summary>
        private OpenOrdersMessage()
        {
        }

        /// <summary>
        /// Creates from string.
        /// </summary>
        /// <param name="rawMessage">The raw message.</param>
        /// <returns></returns>
        internal static OpenOrdersMessage CreateFromJArray(JArray message)
        {
            var orders = message[0]
                .Select(x => (x as JObject)?.ToObject<Dictionary<string, JObject>>())
                .Select(items => items != null && items.Count == 1 ?
                    new
                    {
                        TradeId = items.Keys.ToArray()[0],
                        TradeObject = items.Values.ToArray()[0]
                    } :
                    null)
                .Where(x => x != null)
                .Select(x => Order.CreateFromJObject(x.TradeId, x.TradeObject))
                .ToList();

            return new OpenOrdersMessage()
            {
                Orders = orders,
                ChannelName = message[1].Value<string>(),
                Sequence = message[2].Value<long>("sequence")
            };
        }
    }

    /// <summary>
    /// Open orders. Feed to show all the open orders belonging to the user authenticated API key.
    /// </summary>
    /// <remarks>
    /// Initial snapshot will provide list of all open orders and then any
    /// updates to the open orders list will be sent. For status change updates,
    /// such as 'closed', the fields orderid and status will be present in the payload.
    /// </remarks>
    public class Order
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Order"/> class from being created.
        /// </summary>
        private Order()
        {
        }

        /// <summary>
        /// Creates from new instance json object.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderObject">The order object.</param>
        /// <returns></returns>
        internal static Order CreateFromJObject(string orderId, JObject orderObject)
        {
            return new Order()
            {
                OrderId = orderId,
                RefId = orderObject?.Value<string>("refid"),
                UserRef = orderObject?.Value<long>("userref"),
                Status = orderObject?.Value<string>("status"),
                OpenTimestamp = orderObject?.Value<decimal?>("opentm"),
                StartTimestamp = orderObject?.Value<decimal?>("starttm"),
                ExpireTimestamp = orderObject?.Value<decimal?>("expiretm"),
                Description = OrderDescription.CreateFromJObject(orderObject?.Value<JObject>("descr")),
                Volume = orderObject?.Value<decimal?>("vol"),
                VolumeExecuted = orderObject?.Value<decimal>("vol_exec"),
                Cost = orderObject?.Value<decimal?>("cost"),
                Fee = orderObject?.Value<decimal?>("fee"),
                Price = orderObject?.Value<decimal?>("price"),
                StopPrice = orderObject?.Value<decimal?>("stopprice"),
                LimitPrice = orderObject?.Value<decimal?>("limitprice"),
                Miscellaneous = orderObject?.Value<string>("misc"),
                OrderFlags = orderObject?.Value<string>("oflags")
            };
        }

        /// <summary>
        /// Gets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public string OrderId { get; private set; }

        /// <summary>
        /// Gets the referral order transaction id that created this order.
        /// </summary>
        /// <value>
        /// The referral order transaction id that created this order.
        /// </value>
        public string RefId { get; private set; }

        /// <summary>
        /// Gets the user reference id.
        /// </summary>
        /// <value>
        /// The user reference id.
        /// </value>
        public long? UserRef { get; private set; }

        /// <summary>
        /// Gets the status of order.
        /// </summary>
        /// <value>
        /// The status of order.
        /// </value>
        public string Status { get; private set; }

        /// <summary>
        /// Gets the unix timestamp of when order was placed.
        /// </summary>
        /// <value>
        /// The unix timestamp of when order was placed.
        /// </value>
        public decimal? OpenTimestamp { get; private set; }

        /// <summary>
        /// Gets the unix timestamp of order start time (if set).
        /// </summary>
        /// <value>
        /// The unix timestamp of order start time (if set).
        /// </value>
        public decimal? StartTimestamp { get; private set; }

        /// <summary>
        /// Gets the unix timestamp of order end time (if set).
        /// </summary>
        /// <value>
        /// The unix timestamp of order end time (if set).
        /// </value>
        public decimal? ExpireTimestamp { get; private set; }

        /// <summary>
        /// Gets the order description info.
        /// </summary>
        /// <value>
        /// The order description info.
        /// </value>
        public OrderDescription Description { get; private set; }

        /// <summary>
        /// Gets the volume of order (base currency unless viqc set in orderflags).
        /// </summary>
        /// <value>
        /// The volume of order (base currency unless viqc set in orderflags).
        /// </value>
        public decimal? Volume { get; private set; }

        /// <summary>
        /// Gets the volume executed (base currency unless viqc set in oflags).
        /// </summary>
        /// <value>
        /// The volume executed (base currency unless viqc set in oflags).
        /// </value>
        public decimal? VolumeExecuted { get; private set; }

        /// <summary>
        /// Gets the total cost (quote currency unless unless viqc set in oflags).
        /// </summary>
        /// <value>
        /// The total cost (quote currency unless unless viqc set in oflags).
        /// </value>
        public decimal? Cost { get; private set; }
        /// <summary>
        /// Gets the total fee (quote currency).
        /// </summary>
        /// <value>
        /// The total fee (quote currency).
        /// </value>
        public decimal? Fee { get; private set; }

        /// <summary>
        /// Gets the average price (quote currency unless viqc set in oflags).
        /// </summary>
        /// <value>
        /// The average price (quote currency unless viqc set in oflags).
        /// </value>
        public decimal? Price { get; private set; }
        /// <summary>
        /// Gets the stop price (quote currency, for trailing stops).
        /// </summary>
        /// <value>
        /// The stop price (quote currency, for trailing stops).
        /// </value>
        public decimal? StopPrice { get; private set; }
        /// <summary>
        /// Gets the triggered limit price (quote currency, when limit based order type triggered).
        /// </summary>
        /// <value>
        /// The triggered limit price (quote currency, when limit based order type triggered).
        /// </value>
        public decimal? LimitPrice { get; private set; }

        /// <summary>
        /// Gets the comma delimited list of miscellaneous info.
        /// </summary>
        /// <value>
        /// The comma delimited list of miscellaneous info.
        /// </value>
        /// <remarks>
        /// stopped     = triggered by stop price
        /// touched     = triggered by touch price
        /// liquidation = liquidation
        /// partial     = partial fill
        /// </remarks>
        public string Miscellaneous { get; private set; }

        /// <summary>
        /// Gets the comma delimited list of order flags (optional).
        /// </summary>
        /// <value>
        /// The comma delimited list of order flags (optional).
        /// </value>
        /// <remarks>
        /// viqc  = volume in quote currency (not available for leveraged orders)
        /// fcib  = prefer fee in base currency
        /// fciq  = prefer fee in quote currency
        /// nompp = no market price protection
        /// post  = post only order (available when ordertype = limit)
        /// </remarks>
        public string OrderFlags { get; private set; }
    }

    /// <summary>
    /// Order description info
    /// </summary>
    public class OrderDescription
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="OrderDescription"/> class from being created.
        /// </summary>
        private OrderDescription()
        { }

        /// <summary>
        /// Creates from <see cref="JObject"/>.
        /// </summary>
        /// <param name="orderDescriptionObject">The order description object.</param>
        /// <returns></returns>
        internal static OrderDescription CreateFromJObject(JObject orderDescriptionObject)
        {
            return new OrderDescription
            {
                Pair = orderDescriptionObject?.Value<string>("pair") ?? string.Empty,
                Type = orderDescriptionObject?.Value<string>("type") ?? string.Empty,
                OrderType = orderDescriptionObject?.Value<string>("ordertype") ?? string.Empty,
                Price = orderDescriptionObject?.Value<decimal?>("price"),
                SecondPrice = orderDescriptionObject?.Value<decimal?>("price2"),
                Leverage = orderDescriptionObject?.Value<string>("leverage") ?? string.Empty,
                Order = orderDescriptionObject?.Value<string>("order") ?? string.Empty,
                Close = orderDescriptionObject?.Value<string>("close") ?? string.Empty,
            };
        }

        /// <summary>
        /// Gets the asset pair.
        /// </summary>
        /// <value>
        /// The asset pair.
        /// </value>
        public string Pair { get; private set; }

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
        /// Gets the primary price.
        /// </summary>
        /// <value>
        /// The primary price.
        /// </value>
        public decimal? Price { get; private set; }

        /// <summary>
        /// Gets the secondary price.
        /// </summary>
        /// <value>
        /// The secondary price.
        /// </value>
        public decimal? SecondPrice { get; private set; }

        /// <summary>
        /// Gets the amount of leverage.
        /// </summary>
        /// <value>
        /// The amount of leverage.
        /// </value>
        public string Leverage { get; private set; }

        /// <summary>
        /// Gets the order description.
        /// </summary>
        /// <value>
        /// The order description.
        /// </value>
        public string Order { get; private set; }

        /// <summary>
        /// Gets the conditional close order description (if conditional close set).
        /// </summary>
        /// <value>
        /// The conditional close order description (if conditional close set).
        /// </value>
        public string Close { get; private set; }
    }

    public class OpenOrdersStatusChangeMessage : EventArgs
    {

        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        /// <value>
        /// The name of the channel.
        /// </value>
        public string ChannelName { get; private set; }

        /// <summary>
        /// Gets the list of open orders.
        /// </summary>
        public IList<OrderStatus> Orders { get; private set; }

        public long Sequence { get; set; }

        /// <summary>
        /// Creates from string.
        /// </summary>
        /// <param name="rawMessage">The raw message.</param>
        /// <returns></returns>
        internal static OpenOrdersStatusChangeMessage CreateFromJArray(JArray message)
        {
            var orders = message[0]
                .Select(x => (x as JObject)?.ToObject<Dictionary<string, JObject>>())
                .Select(items => items != null && items.Count == 1 ?
                    new
                    {
                        TradeId = items.Keys.ToArray()[0],
                        TradeObject = items.Values.ToArray()[0]
                    } :
                    null)
                .Where(x => x != null)
                .Select(x => OrderStatus.CreateFromJObject(x.TradeId, x.TradeObject))
                .ToList();

            return new OpenOrdersStatusChangeMessage()
            {
                Orders = orders,
                ChannelName = message[1].Value<string>(),
                Sequence = message[2].Value<long>("sequence")
            };
        }
    }

    public class OrderStatus
    {

        /// <summary>
        /// Creates from new instance json object.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderObject">The order object.</param>
        /// <returns></returns>
        internal static OrderStatus CreateFromJObject(string orderId, JObject orderObject)
        {
            return new OrderStatus()
            {
                OrderId = orderId,
                Status = orderObject?.Value<string>("status"),
            };
        }

        /// <summary>
        /// Gets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public string OrderId { get; private set; }

        /// <summary>
        /// Gets the status of order.
        /// </summary>
        /// <value>
        /// The status of order.
        /// </value>
        public string Status { get; private set; }

    }

}
