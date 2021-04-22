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
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using KrakenSharp.Messages;

namespace KrakenSharp.WebSockets
{
    class KrakenSocketConnection : IDisposable
    {

        #region Events

        public event EventHandler Connected;
        public event EventHandler<RawKrakenMessage> MessageReceived;

        #endregion

        #region Fields

        private ClientWebSocket _socket;
        private readonly JsonSerializerSettings _jsonsettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        #endregion

        #region CONSTRUCTOR

        public KrakenSocketConnection(string url)
        {
            this.Url = url;
        }

        #endregion

        #region Properties

        public string Url { get; }

        #endregion

        #region Methods

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (_socket != null) { return; }

            _socket = new ClientWebSocket();
            await _socket.ConnectAsync(new Uri(Url), cancellationToken);
            Connected?.Invoke(this, EventArgs.Empty);
            _ = SocketListener(_socket);
        }

        public async Task CloseAsync(CancellationToken cancellationToken = default)
        {
            if(_socket == null) { return; }

            if(_socket.State == WebSocketState.Open)
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection Closed", cancellationToken);
            }
            _socket = null;
        }

        public async Task SendAsync(object message, CancellationToken cancellationToken = default)
        {
            var socket = _socket;
            if (socket != null && socket.State == WebSocketState.Open)
            {
                var json = JsonConvert.SerializeObject(message, Formatting.None, _jsonsettings);
                var bytes = Encoding.UTF8.GetBytes(json);
                await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
            }
        }

        #endregion

        #region Private Methods

        private async Task SocketListener(ClientWebSocket socket, CancellationToken cancellationToken = default)
        {
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var message = await ReadNextMessage(socket, cancellationToken);

                    string eventString = null;
                    long? channelId = null;

                    if (!string.IsNullOrEmpty(message))
                    {
                        var token = JToken.Parse(message);
                        switch (token)
                        {
                            case JObject _:
                                var messageObj = JObject.Parse(message);
                                eventString = (string)messageObj.GetValue("event");
                                break;

                            case JArray arrayToken:
                                // Data / private messages
                                if (long.TryParse(arrayToken.First.ToString(), out var localChannelId))
                                {
                                    channelId = localChannelId;
                                }

                                eventString = channelId != null ? "data" : "private";
                                break;
                        }

                        MessageReceived?.Invoke(this, new RawKrakenMessage(eventString, message, channelId));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                socket.Dispose();
                if (_socket == socket) _socket = null;
            }
        }

        private async Task<string> ReadNextMessage(ClientWebSocket socket, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[1024];
            var messageParts = new StringBuilder();

            WebSocketReceiveResult result;
            do
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                }
                else
                {
                    var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    messageParts.Append(str);
                }

            } while (!result.EndOfMessage);

            var message = messageParts.ToString();
            return message;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            _ = this.CloseAsync();
        }

        #endregion

    }
}
