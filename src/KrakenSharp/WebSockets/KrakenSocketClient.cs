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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KrakenSharp.WebSockets;
using KrakenSharp.Messages;

namespace KrakenSharp.WebSockets
{

    public abstract class KrakenSocketClient : IDisposable
    {

        public const int DEFAULT_AWAIT_TIMEOUT = 15000;

        public event EventHandler<RawKrakenMessage> MessageReceived;

        public event EventHandler<Heartbeat> HeartbeatReceived;
        public event EventHandler<SystemStatusResponse> SystemStatusChanged;
        public event EventHandler<SubscriptionStatusResponse> SubscriptionStatusChanged;
        public event EventHandler<PongMessage> PongReceived;

        #region Fields

        private int _requestIdCounter;
        KrakenSocketConnection _socket;
        private Dictionary<long, SubscriptionStatusResponse> _subscriptions = new Dictionary<long, SubscriptionStatusResponse>();
        private System.Collections.ObjectModel.ReadOnlyDictionary<long, SubscriptionStatusResponse> _readonlySubscriptions;

        #endregion

        #region CONSTRUCTOR

        public KrakenSocketClient(string url)
            : this(new KrakenSocketConnection(url))
        {
            this.ClientId = (ushort)(DateTime.UtcNow.Ticks & 65535);
        }

        internal KrakenSocketClient(KrakenSocketConnection socket)
        {
            this.ClientId = (ushort)(DateTime.UtcNow.Ticks & 65535);
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _socket.MessageReceived += _socket_MessageReceived;
            _readonlySubscriptions = new System.Collections.ObjectModel.ReadOnlyDictionary<long, SubscriptionStatusResponse>(_subscriptions);
        }

        #endregion

        #region Properties

        /// <summary>
        /// A value representing this client. This will be used for generating requestId's in the format:
        /// leading 0 - requestIds must be positive
        /// 16 bits clientId
        /// 15 bits counter
        /// </summary>
        public ushort ClientId { get; set; }

        internal KrakenSocketConnection Socket { get { return _socket; } }

        public ILogger Logger { get; set; }

        public int DefaultAwaitTimeout { get; set; } = DEFAULT_AWAIT_TIMEOUT;



        public SystemStatusResponse SystemStatue { get; private set; }

        public IDictionary<long, SubscriptionStatusResponse> Subscriptions { get { return _readonlySubscriptions; } }

        #endregion

        #region Methods

        public Task Connect(CancellationToken token = default(CancellationToken))
        {
            try
            {
                return _socket.ConnectAsync(token);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to Open Connection.");
                throw ex;
            }
        }

        public Task Close(CancellationToken token = default(CancellationToken))
        {
            try
            { 
                return _socket.CloseAsync(token);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to Close Connection.");
                throw ex;
            }
        }

        public Task Subscribe(SubscribeCommand subscription, CancellationToken token = default(CancellationToken))
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            try
            {
                return _socket.SendAsync(subscription, token);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to Subscribe for {@subscription}", subscription);
                throw ex;
            }
        }

        /// <summary>
        /// Submits a subscription and waits for the status's to come back. The result will contains all statuses captured. 
        /// If those responses do not come before 'timeout' passes than only those captured in that time are returned. 
        /// This means that for N pairs, you may get 0->N responses in the list.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<SubscriptionStatusResponse>> SubscribeAndWait(SubscribeCommand subscription, int timeout = -1, CancellationToken token = default(CancellationToken))
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            timeout = GetTimeout(timeout);
            var hash = new HashSet<string>(subscription.Pairs);
            if (subscription.RequestId == null) subscription.RequestId = GetNextRequestId();

            var results = new List<SubscriptionStatusResponse>(hash.Count);
            var tcs = new TaskCompletionSource<bool>();
            EventHandler<SubscriptionStatusResponse> handler = (s, e) =>
            {
                if(e.RequestId == subscription.RequestId && 
                   string.Equals(e.Subscription.Name, subscription.Options.Name, StringComparison.OrdinalIgnoreCase) && 
                   hash.Contains(e.Pair))
                {
                    results.Add(e);
                    if(results.Count == hash.Count)
                    {
                        tcs.SetResult(true);
                    }
                }
            };

            try
            {
                this.SubscriptionStatusChanged += handler;
                await _socket.SendAsync(subscription, token);
                var complete = await Task.WhenAny(tcs.Task, Task.Delay(timeout, token));
                return results;
            }
            catch (OperationCanceledException cex)
            {
                throw cex;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to Subscribe for {@subscription}", subscription);
                throw ex;
            }
            finally
            {
                this.SubscriptionStatusChanged -= handler;
            }
        }

        public Task Unsubscribe(long channelId, CancellationToken token = default(CancellationToken))
        {
            try
            {
                Logger?.LogTrace("Unsubscribe from subscription with channelId '{channelId}'", channelId);
                return _socket.SendAsync(new Unsubscribe(channelId), token);
            }
            catch(Exception ex)
            {
                Logger?.LogError(ex, "Failed to Unsubscribe {channelId}.", channelId);
                throw ex;
            }
        }

        public Task Ping(CancellationToken token = default(CancellationToken))
        {
            try
            {
                return _socket.SendAsync(new PingMessage() { RequestId = GetNextRequestId() }, token);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to Ping server.");
                throw ex;
            }
        }

        public async Task<bool> PingAndWait(int timeout = -1, CancellationToken token = default(CancellationToken))
        {
            timeout = GetTimeout(timeout);
            var cmnd = new PingMessage() { RequestId = GetNextRequestId() };
            var tcs = new TaskCompletionSource<PongMessage>();
            EventHandler<PongMessage> handler = (s, e) =>
            {
                if(e.RequestId == cmnd.RequestId)
                {
                    tcs.SetResult(e);
                }
            };
            try
            {
                this.PongReceived += handler;
                await _socket.SendAsync(cmnd, token);
                var complete = await Task.WhenAny(tcs.Task, Task.Delay(timeout, token));
                return complete == tcs.Task;
            }
            catch (OperationCanceledException cex)
            {
                throw cex;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to Ping server.");
                throw ex;
            }
            finally
            {
                this.PongReceived -= handler;
            }
        }

        public int GetNextRequestId()
        {
            int counter = Interlocked.Increment(ref _requestIdCounter);
            return (this.ClientId << 15) | (counter & 32767);
        }

        #endregion

        #region Private Methods

        private void SyncSubscriptionStatus(SubscriptionStatusResponse status)
        {
            if (status == null) return;

            long channelid = status.ChannelId;
            switch(status.Status)
            {
                case SubscriptionStatus.Unsubscribed:
                    _subscriptions.Remove(channelid);
                    break;
                default:
                    _subscriptions[channelid] = status;
                    break;
            }
        }

        protected T UnwrapReceivedMessage<T>(RawKrakenMessage e)
        {
            try
            {
                T result = JsonConvert.DeserializeObject<T>(e.RawContent);
                Logger?.LogTrace("Event '{eventType}' received: {@event}", e.Event, result);
                return result;
            }
            catch(Exception ex)
            {
                Logger?.LogError(ex, "Failed to deserialize {event}: {message}", e.Event, e.RawContent);
            }

            return default(T);
        }

        protected virtual bool HandleSocketMessage(RawKrakenMessage e)
        {
            switch (e.Event)
            {
                case Heartbeat.EventName:
                    e.UnwrappedObject = Heartbeat.Empty;
                    this.HeartbeatReceived?.Invoke(this, Heartbeat.Empty);
                    return true;
                case SystemStatusResponse.EventName:
                    {
                        var status = UnwrapReceivedMessage<SystemStatusResponse>(e);
                        e.UnwrappedObject = status;
                        if (status != null)
                        {
                            this.SystemStatue = status;
                            this.SystemStatusChanged?.Invoke(this, status);
                        }
                    }
                    return true;
                case SubscriptionStatusResponse.EventName:
                    {
                        var status = UnwrapReceivedMessage<SubscriptionStatusResponse>(e);
                        e.UnwrappedObject = status;
                        if (status != null)
                        {
                            SyncSubscriptionStatus(status);
                            this.SubscriptionStatusChanged?.Invoke(this, status);
                        }
                    }
                    return true;
                case PongMessage.EventName:
                    {
                        var status = UnwrapReceivedMessage<PongMessage>(e);
                        e.UnwrappedObject = status;
                        if (status != null) this.PongReceived?.Invoke(this, status);
                    }
                    return true;
            }

            return false;
        }

        public int GetTimeout(int timeout)
        {
            if (timeout >= 0) return timeout;
            if (this.DefaultAwaitTimeout >= 0) return this.DefaultAwaitTimeout;
            return DEFAULT_AWAIT_TIMEOUT;
        }

        #endregion

        #region Event Handlers

        private void _socket_MessageReceived(object sender, RawKrakenMessage e)
        {
            this.HandleSocketMessage(e);
            this.MessageReceived?.Invoke(this, e);
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            if(_socket != null)
            {
                _socket.Dispose();
            }
        }

        #endregion

    }

    public class PublicKrakenSocketClient : KrakenSocketClient
    {

        public event EventHandler<TickerMessage> TickerReceived;
        public event EventHandler<OhlcMessage> OhlcReceived;
        public event EventHandler<TradeMessage> TradeReceived;
        public event EventHandler<SpreadMessage> SpreadReceived;
        public event EventHandler<BookMessage> BookSnapshotReceived;
        public event EventHandler<BookMessage> BookUpdateReceived;

        #region CONSTRUCTOR

        public PublicKrakenSocketClient(string url)
            : this(new KrakenSocketConnection(url))
        {

        }

        internal PublicKrakenSocketClient(KrakenSocketConnection socket) : base(socket)
        {

        }

        #endregion

        #region Overrides

        protected override bool HandleSocketMessage(RawKrakenMessage e)
        {
            if (base.HandleSocketMessage(e)) return true;

            switch (e.Event)
            {
                case "data":
                    {
                        SubscriptionStatusResponse subscription;
                        if (this.Subscriptions.TryGetValue(e.ChannelId.Value, out subscription))
                        {
                            HandleData(e, subscription);
                        }
                    }
                    return true;
            }

            return false;
        }

        private void HandleData(RawKrakenMessage e, SubscriptionStatusResponse subscription)
        {
            switch (subscription.Subscription.Name)
            {
                case SubscribeOptionNames.Ticker:
                    {
                        var msg = TickerMessage.Parse(e.RawContent, subscription);
                        e.UnwrappedObject = msg;
                        TickerReceived?.Invoke(this, msg);
                    }
                    break;
                case SubscribeOptionNames.OHLC:
                    {
                        var msg = OhlcMessage.Parse(e.RawContent, subscription);
                        e.UnwrappedObject = msg;
                        OhlcReceived?.Invoke(this, msg);
                    }
                    break;
                case SubscribeOptionNames.Trade:
                    {
                        var msg = TradeMessage.Parse(e.RawContent, subscription);
                        e.UnwrappedObject = msg;
                        TradeReceived?.Invoke(this, msg);
                    }
                    break;
                case SubscribeOptionNames.Spread:
                    {
                        var msg = SpreadMessage.Parse(e.RawContent, subscription);
                        e.UnwrappedObject = msg;
                        SpreadReceived?.Invoke(this, msg);
                    }
                    break;
                case SubscribeOptionNames.Book:
                    {
                        //REFACTOR - we do not set the unwrapped object for book, need to determine if this should be an else/if or if we can receive both a snapshot AND a update in the same message
                        if (e.RawContent.Contains(@"""as"":") && e.RawContent.Contains(@"""bs"":"))
                        {
                            BookSnapshotReceived?.Invoke(this, BookMessage.ParseSnapshot(e.RawContent, subscription));
                        }
                        if (e.RawContent.Contains(@"""a"":") || e.RawContent.Contains(@"""b"":"))
                        {
                            BookUpdateReceived?.Invoke(this, BookMessage.ParseUpdate(e.RawContent, subscription));
                        }
                    }
                    break;
            }
        }

        #endregion

    }

    public class PrivateKrakenSocketClient : KrakenSocketClient
    {

        public event EventHandler<AddOrderStatusResponse> AddOrderStatusReceived;
        public event EventHandler<CancelOrderStatusResponse> CancelOrderStatusReceived;
        public event EventHandler<CancelAllStatusResponse> CancelAllStatusReceived;
        public event EventHandler<CancelAllOrdersAfterStatusResponse> CancelAllOrdersAfterStatusReceived;

        public event EventHandler<OwnTradesMessage> OwnTradesReceived;
        public event EventHandler<OpenOrdersMessage> OpenOrdersReceived;
        public event EventHandler<OpenOrdersStatusChangeMessage> OpenOrdersStatusChangedReceived;

        #region CONSTRUCTOR

        public PrivateKrakenSocketClient(string url)
            : this(new KrakenSocketConnection(url))
        {

        }

        internal PrivateKrakenSocketClient(KrakenSocketConnection socket) : base(socket)
        {

        }

        #endregion

        #region Methods

        public Task AddOrder(AddOrderCommand cmnd, CancellationToken token = default(CancellationToken))
        {
            if (cmnd == null) throw new ArgumentNullException(nameof(cmnd));

            try
            {
                Logger?.LogTrace("Adding new order:{@cmnd}", cmnd);
                return this.Socket.SendAsync(cmnd, token);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Fatal Error attempting to add order: {@cmnd}.", cmnd);
                throw ex;
            }
        }

        public async Task<AddOrderStatusResponse> AddOrderAndWait(AddOrderCommand cmnd, int timeout = -1, CancellationToken token = default(CancellationToken))
        {
            if (cmnd == null) throw new ArgumentNullException(nameof(cmnd));

            timeout = GetTimeout(timeout);
            if (cmnd.RequestId == null) cmnd.RequestId = GetNextRequestId();

            var tcs = new TaskCompletionSource<AddOrderStatusResponse>();
            EventHandler<AddOrderStatusResponse> handler = (s, e) =>
            {
                if (e.RequestId == cmnd.RequestId)
                {
                    tcs.SetResult(e);
                }
            };

            try
            {
                this.AddOrderStatusReceived += handler;
                await this.Socket.SendAsync(cmnd, token);
                var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout, token));
                return completed == tcs.Task ? tcs.Task.Result : AddOrderStatusResponse.Timeout(cmnd.RequestId);
            }
            catch(OperationCanceledException cex)
            {
                throw cex;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Fatal Error attempting to add order: {@cmnd}.", cmnd);
                throw ex;
            }
            finally
            {
                this.AddOrderStatusReceived -= handler;
            }
        }

        public Task CancelOrder(CancelOrderCommand cmnd, CancellationToken token = default(CancellationToken))
        {
            if (cmnd == null) throw new ArgumentNullException(nameof(cmnd));

            try
            {
                Logger?.LogTrace("Cancelling existing order: {@cmnd}", cmnd);
                return this.Socket.SendAsync(cmnd, token);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Fatal Error attempting to cancel order: {@cmnd}.", cmnd);
                throw ex;
            }
        }

        public async Task<CancelOrderStatusResponse> CancelOrderAndWait(CancelOrderCommand cmnd, int timeout = -1, CancellationToken token = default(CancellationToken))
        {
            if (cmnd == null) throw new ArgumentNullException(nameof(cmnd));

            timeout = GetTimeout(timeout);
            if (cmnd.RequestId == null) cmnd.RequestId = GetNextRequestId();

            var tcs = new TaskCompletionSource<CancelOrderStatusResponse>();
            EventHandler<CancelOrderStatusResponse> handler = (s, e) =>
            {
                if (e.RequestId == cmnd.RequestId)
                {
                    tcs.SetResult(e);
                }
            };

            try
            {
                this.CancelOrderStatusReceived += handler;
                await this.Socket.SendAsync(cmnd, token);
                var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout, token));
                return completed == tcs.Task ? tcs.Task.Result : CancelOrderStatusResponse.Timeout(cmnd.RequestId);
            }
            catch (OperationCanceledException cex)
            {
                throw cex;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Fatal Error attempting to cancel order: {@cmnd}.", cmnd);
                throw ex;
            }
            finally
            {
                this.CancelOrderStatusReceived -= handler;
            }
        }

        public Task CancelAll(CancelAllCommand cmnd, CancellationToken token = default(CancellationToken))
        {
            if (cmnd == null) throw new ArgumentNullException(nameof(cmnd));
            return this.Socket.SendAsync(cmnd, token);
        }

        public Task CancelAll(string authToken, CancellationToken token = default(CancellationToken))
        {
            return this.CancelAll(new CancelAllCommand(authToken) { }, token);
        }

        public async Task<CancelAllStatusResponse> CancelAllAndWait(string authToken, int timeout = -1, CancellationToken token = default(CancellationToken))
        {
            timeout = GetTimeout(timeout);
            var cmnd = new CancelAllCommand(authToken) { RequestId = GetNextRequestId() };

            var tcs = new TaskCompletionSource<CancelAllStatusResponse>();
            EventHandler<CancelAllStatusResponse> handler = (s, e) =>
            {
                if (e.RequestId == cmnd.RequestId)
                {
                    tcs.SetResult(e);
                }
            };

            try
            {
                this.CancelAllStatusReceived += handler;
                await this.Socket.SendAsync(cmnd, token);
                var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout, token));
                return completed == tcs.Task ? tcs.Task.Result : CancelAllStatusResponse.Timeout(cmnd.RequestId);
            }
            catch (OperationCanceledException cex)
            {
                throw cex;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Fatal Error attempting to cancel all orders: {@cmnd}.", cmnd);
                throw ex;
            }
            finally
            {
                this.CancelAllStatusReceived -= handler;
            }
        }

        public Task CancelAllOrdersAfter(CancelAllOrdersAfterCommand cmnd, CancellationToken token = default(CancellationToken))
        {
            if (cmnd == null) throw new ArgumentNullException(nameof(cmnd));
            return this.Socket.SendAsync(cmnd, token);
        }

        public Task CancelAllOrdersAfter(string authToken, int seconds, CancellationToken token = default(CancellationToken))
        {
            return this.CancelAllOrdersAfter(new CancelAllOrdersAfterCommand(authToken, seconds), token);
        }

        public async Task<CancelAllOrdersAfterStatusResponse> CancelAllOrdersAfterAndWait(string authToken, int seconds, int timeout = -1, CancellationToken token = default(CancellationToken))
        {
            timeout = GetTimeout(timeout);
            var cmnd = new CancelAllOrdersAfterCommand(authToken, seconds) { RequestId = GetNextRequestId() };

            var tcs = new TaskCompletionSource<CancelAllOrdersAfterStatusResponse>();
            EventHandler<CancelAllOrdersAfterStatusResponse> handler = (s, e) =>
            {
                if (e.RequestId == cmnd.RequestId)
                {
                    tcs.SetResult(e);
                }
            };

            try
            {
                this.CancelAllOrdersAfterStatusReceived += handler;
                await this.Socket.SendAsync(cmnd, token);
                var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout, token));
                return completed == tcs.Task ? tcs.Task.Result : CancelAllOrdersAfterStatusResponse.Timeout(cmnd.RequestId);
            }
            catch (OperationCanceledException cex)
            {
                throw cex;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Fatal Error attempting to cancel all orders: {@cmnd}.", cmnd);
                throw ex;
            }
            finally
            {
                this.CancelAllOrdersAfterStatusReceived -= handler;
            }
        }

        #endregion

        #region Overrides

        protected override bool HandleSocketMessage(RawKrakenMessage e)
        {
            if (base.HandleSocketMessage(e)) return true;

            switch(e.Event)
            {
                case AddOrderStatusResponse.EventName:
                    {
                        var status = UnwrapReceivedMessage<AddOrderStatusResponse>(e);
                        e.UnwrappedObject = status;
                        if (status != null)
                        {
                            this.AddOrderStatusReceived?.Invoke(this, status);
                        }
                    }
                    return true;
                case CancelOrderStatusResponse.EventName:
                    {
                        var status = UnwrapReceivedMessage<CancelOrderStatusResponse>(e);
                        e.UnwrappedObject = status;
                        if (status != null)
                        {
                            this.CancelOrderStatusReceived?.Invoke(this, status);
                        }
                    }
                    return true;
                case CancelAllStatusResponse.EventName:
                    {
                        var status = UnwrapReceivedMessage<CancelAllStatusResponse>(e);
                        e.UnwrappedObject = status;
                        if (status != null)
                        {
                            this.CancelAllStatusReceived?.Invoke(this, status);
                        }
                    }
                    return true;
                case CancelAllOrdersAfterStatusResponse.EventName:
                    {
                        var status = UnwrapReceivedMessage<CancelAllOrdersAfterStatusResponse>(e);
                        e.UnwrappedObject = status;
                        if (status != null)
                        {
                            this.CancelAllOrdersAfterStatusReceived?.Invoke(this, status);
                        }
                    }
                    return true;
                case "private":
                    HandlePrivateData(e);
                    return true;
            }

            return false;
        }

        private void HandlePrivateData(RawKrakenMessage e)
        {
            var message = KrakenDataMessageHelper.EnsureRawMessageIsJArray(e.RawContent);
            
            switch((string)message[1])
            {
                case "ownTrades":
                    {
                        try
                        {
                            var result = OwnTradesMessage.CreateFromJArray(message);
                            Logger?.LogTrace("Event '{eventType}' received: {@event}", "ownTrades", result);
                            OwnTradesReceived?.Invoke(this, result);
                        }
                        catch (Exception ex)
                        {
                            Logger?.LogError(ex, "Failed to deserialize {event}: {message}", "ownTrades", e.RawContent);
                        }
                    }
                    break;
                case "openOrders":
                    {
                        try
                        {
                            var jar = (JArray)message[0];
                            if(jar.Count > 0 && (jar[0] as JObject)?.Count == 1)
                            {
                                //if it's only got one entry, that's the status update
                                var result = OpenOrdersStatusChangeMessage.CreateFromJArray(message);
                                Logger?.LogTrace("Event '{eventType}' received: {@event}", "openOrders", result);
                                OpenOrdersStatusChangedReceived?.Invoke(this, result);
                            }
                            else
                            {
                                var result = OpenOrdersMessage.CreateFromJArray(message);
                                Logger?.LogTrace("Event '{eventType}' received: {@event}", "openOrders", result);
                                OpenOrdersReceived?.Invoke(this, result);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger?.LogError(ex, "Failed to deserialize {event}: {message}", "openOrders", e.RawContent);
                        }
                    }
                    break;
            }
        }

        #endregion

    }

}
