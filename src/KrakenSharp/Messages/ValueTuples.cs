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

    public struct PriceAndVolume<T>
    {

        public T Price { get; set; }
        public T LotVolume { get; set; }

        public static PriceAndVolume<T> Parse(JArray jarr)
        {
            return new PriceAndVolume<T>()
            {
                Price = ConvertUtil.ToPrim<T>(jarr[0]),
                LotVolume = ConvertUtil.ToPrim<T>(jarr[1])
            };
        }

    }

    public struct PriceAndWholeVolume<T>
    {

        public T Price { get; set; }
        public T WholeLotVolume { get; set; }
        public T LotVolume { get; set; }

        public static PriceAndWholeVolume<T> Parse(JArray jarr)
        {
            return new PriceAndWholeVolume<T>()
            {
                Price = ConvertUtil.ToPrim<T>(jarr[0]),
                WholeLotVolume = ConvertUtil.ToPrim<T>(jarr[1]),
                LotVolume = ConvertUtil.ToPrim<T>(jarr[2])
            };
        }

    }

    public struct TodayAndLast24Hours<T>
    {

        public T Today { get; set; }
        public T Last24Hours { get; set; }


        public static TodayAndLast24Hours<T> Parse(JArray jarr)
        {
            return new TodayAndLast24Hours<T>()
                {
                    Today = ConvertUtil.ToPrim<T>(jarr[0]),
                    Last24Hours = ConvertUtil.ToPrim<T>(jarr[1])
                };
        }

    }

}
