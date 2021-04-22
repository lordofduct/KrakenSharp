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
using System.Globalization;
using Newtonsoft.Json;

namespace KrakenSharp.Messages
{
    public enum OrderType
    {
        Limit = 1,
        Market = 2
    }

    public enum Side
    {
        Buy,
        Sell
    }

    public enum Status
    {
        Error = -1,
        OK = 0,
    }

    public enum SubscriptionStatus
    {
        Error = -1,
        Unsubscribed = 0,
        Subscribed = 1,
    }

    /// <summary>
    /// Specialized converter for <see cref="OrderType"/>
    /// </summary>
    /// <seealso cref="JsonConverter" />
    internal sealed class OrderTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(OrderType) || objectType == typeof(OrderType?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => writer.WriteValue(Enum.GetName(typeof(OrderType), value).ToLower());
    }

    internal sealed class SideConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(Side) || objectType == typeof(Side?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => writer.WriteValue(Enum.GetName(typeof(Side), value).ToLower());
    }

    /// <summary>
    /// <see cref="JsonConverter"/> implementation for <see cref="Status"/> enum
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal sealed class StatusConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(Status) || objectType == typeof(Status?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (((string)reader.Value).ToLower()) switch
            {
                "ok" => Status.OK,
                "error" => Status.Error,
                _ => throw new InvalidOperationException($"Value '{reader.Value}' cannot be converted to type '{nameof(Status)}'"),
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }

    internal sealed class SubscriptionStatusConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(SubscriptionStatus) || objectType == typeof(SubscriptionStatus?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (((string)reader.Value).ToLower()) switch
            {
                "unsubscribed" => SubscriptionStatus.Unsubscribed,
                "subscribed" => SubscriptionStatus.Subscribed,
                "error" => SubscriptionStatus.Error,
                _ => throw new InvalidOperationException($"Value '{reader.Value}' cannot be converted to type '{nameof(Status)}'"),
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }

    internal class DecimalToStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(decimal) || objectType == typeof(decimal?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => writer.WriteValue(((decimal)value).ToString(CultureInfo.InvariantCulture));
    }
}
