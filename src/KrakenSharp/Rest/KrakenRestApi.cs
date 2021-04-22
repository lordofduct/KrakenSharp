using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using KrakenSharp.Messages;

namespace KrakenSharp.Rest
{
    public class KrakenRestApi
    {

        #region Properties

        public string Url { get; set; }
        public int Version { get; set; }
        public KrakenCredentials Credentials { get; set; }

        private JsonSerializer _serializer = new JsonSerializer();

        #endregion

        #region Methods

        public async Task<Dictionary<string, AssetPairInfo>> GetAssetPairs()
        {
            // //https://api.kraken.com/0/public/AssetPairs
            var path = $"/{this.Version}/public/AssetPairs";
            var address = $"{this.Url}{path}";
            var request = new HttpRequestMessage(HttpMethod.Post, address);

            return (await GetResponse(request))?.Property("result")?.Value.ToObject<Dictionary<string, AssetPairInfo>>();
        }



        /*
         * PRIVATE API
         */

        public async Task<AuthToken> GetWebsocketToken()
        {
            //https://api.kraken.com/0/private/GetWebSocketsToken
            var request = await CreatePrivateMessage($"/{this.Version}/private/GetWebSocketsToken");
            return (await GetResponse(request))?.Property("result")?.Value.ToObject<AuthToken>();
        }

        public async Task<Dictionary<string, decimal>> GetBalances()
        {
            //https://api.kraken.com/0/private/Balance
            var request = await CreatePrivateMessage($"/{this.Version}/private/Balance");
            return (await GetResponse(request))?.Property("result")?.Value.ToObject<Dictionary<string, decimal>>();
        }

        //public async Task<List<Order> GetOpenOrders()
        //{
        //    //https://api.kraken.com/0/private/OpenOrders
        //    var request = await CreatePrivateMessage($"/{this.Version}/private/OpenOrders");
        //    var orders = await GetResponse(request);
        //    return null;
        //    //return orders.GetProperty("result")?.GetProperty("open")?.Value
        //}

        public async Task<List<Order>> GetOpenOrders()
        {
            //https://api.kraken.com/0/private/OpenOrders
            var request = await CreatePrivateMessage($"/{this.Version}/private/OpenOrders");
            try
            {
                return (await GetResponse(request))?["result"]["open"].ToObject<JObject>()
                                                   ?.Properties()
                                                    .Select(p => Order.CreateFromJObject(p.Name, p.Value.ToObject<JObject>()))
                                                    .ToList();
            }
            catch(Exception)
            {
                return null;
            }
        }


        private async Task<HttpRequestMessage> CreatePrivateMessage(string path, Dictionary<string, string> formContent = null)
        {
            if(formContent == null)
            {
                formContent = new Dictionary<string, string>();
            }

            // generate a 64 bit nonce using a timestamp at tick resolution
            var nonce = DateTime.Now.Ticks;
            formContent.Add("nonce", nonce.ToString());

            var address = $"{this.Url}{path}";

            var content = new FormUrlEncodedContent(formContent);
            var request = new HttpRequestMessage(HttpMethod.Post, address)
            {
                Content = content,
                Headers =
                {
                    {"API-Key", this.Credentials.ApiKey.ToPlainString()},
                    {"API-Sign", Convert.ToBase64String(SecureMessaging.CalculateSignature(await content.ReadAsByteArrayAsync(), nonce, path, this.Credentials.ApiPrivKey)) }
                }
            };

            return request;
        }


        private async Task<JObject> GetResponse(HttpRequestMessage request)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.SendAsync(request);
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var jsonReader = new JsonTextReader(new StreamReader(stream)))
                    {
                        return _serializer.Deserialize<JObject>(jsonReader);
                    }
                }
            }
        }

        #endregion

    }
}
