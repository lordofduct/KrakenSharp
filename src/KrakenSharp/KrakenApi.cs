using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using KrakenSharp.Rest;
using KrakenSharp.WebSockets;

namespace KrakenSharp
{
    public class KrakenApi : ICloneable
    {

        public const string DEFAULT_SOCKET_URL = "wss://ws.kraken.com";
        public const string DEFAULT_PRIVATE_SOCKET_URL = "wss://ws-auth.kraken.com";
        public const string DEFAULT_REST_URL = " https://api.kraken.com";

        #region Properties

        public string SocketUrl { get; set; } = DEFAULT_SOCKET_URL;
        public string PrivateSocketUrl { get; set; } = DEFAULT_PRIVATE_SOCKET_URL;
        public string RestUrl { get; set; } = DEFAULT_REST_URL;
        public int Version { get; set; }

        public KrakenCredentials Credentials { get; set; }

        #endregion

        #region CONSTRUCTOR

        public KrakenApi()
        {

        }

        public KrakenApi(KrakenCredentials credentials)
        {
            this.Credentials = credentials;
        }

        #endregion

        #region Methods

        public KrakenRestApi CreateRestConnection()
        {
            return new KrakenRestApi()
            {
                Url = this.RestUrl,
                Version = this.Version,
                Credentials = this.Credentials?.Clone(),
            };
        }

        public PublicKrakenSocketClient CreatePublicSocketConnection()
        {
            return new PublicKrakenSocketClient(this.SocketUrl);
        }

        public PrivateKrakenSocketClient CreatePrivateSocketConnection()
        {
            return new PrivateKrakenSocketClient(this.PrivateSocketUrl);
        }

        #endregion

        #region ICloneable Interface

        public virtual KrakenApi Clone()
        {
            return new KrakenApi()
            {
                SocketUrl = this.SocketUrl,
                PrivateSocketUrl = this.PrivateSocketUrl,
                RestUrl = this.RestUrl,
                Version = this.Version,
                Credentials = this.Credentials?.Clone(),
            };
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

    }
}
