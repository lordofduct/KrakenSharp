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
    public sealed class TickerMessage : EventArgs
    {

        public int ChannelId { get; private set; }
        public string Pair { get; private set; }
        public PriceAndWholeVolume<decimal> Ask { get; private set; }
        public PriceAndWholeVolume<decimal> Bid { get; private set; }
        public PriceAndVolume<decimal> Close { get; private set; }
        public TodayAndLast24Hours<decimal> Volume { get; private set; }

        public static TickerMessage Parse(string json, SubscriptionStatusResponse subscription)
        {
            var tokenArray = KrakenDataMessageHelper.EnsureRawMessageIsJArray(json);
            var message = tokenArray[1];

            return new TickerMessage()
            {
                ChannelId = (int)tokenArray.First,
                Pair = subscription.Pair,
                Ask = PriceAndWholeVolume<decimal>.Parse(message["a"] as JArray),
                Bid = PriceAndWholeVolume<decimal>.Parse(message["b"] as JArray),
                Close = PriceAndVolume<decimal>.Parse(message["c"] as JArray),
                Volume = TodayAndLast24Hours<decimal>.Parse(message["v"] as JArray)
            };
        }

    }


}
