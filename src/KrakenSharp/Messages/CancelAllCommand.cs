using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrakenSharp.Messages
{

    public class CancelAllCommand : PrivateKrakenMessage
    {
        public const string EventName = "cancelAll";

        [JsonProperty("reqid")]
        public int? RequestId { get; set; }

        public CancelAllCommand(string token) : base(EventName, token) { }

    }

    public class CancelAllStatusResponse : KrakenMessage
    {
        public const string EventName = "cancelAllStatus";

        [JsonProperty("reqid")]
        public int? RequestId { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StatusConverter))]
        public Status Status { get; private set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        public CancelAllStatusResponse() : base(EventName) { }

        public static CancelAllStatusResponse Timeout(int? requestId = null)
        {
            return new CancelAllStatusResponse()
            {
                RequestId = requestId,
                Count = 0,
                Status = Status.Error,
                ErrorMessage = "Timeout",
            };
        }

    }

    public class CancelAllOrdersAfterCommand : PrivateKrakenMessage
    {
        public const string EventName = "cancelAllOrdersAfter";


        [JsonProperty("timeout")]
        public int Timeout { get; set; }

        [JsonProperty("reqid")]
        public int? RequestId { get; set; }

        public CancelAllOrdersAfterCommand(string token, int seconds) : base(EventName, token)
        {
            this.Timeout = seconds;
        }

    }

    public class CancelAllOrdersAfterStatusResponse : KrakenMessage
    {
        public const string EventName = "cancelAllOrdersAfterStatus";

        [JsonProperty("reqid")]
        public int? RequestId { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StatusConverter))]
        public Status Status { get; private set; }

        /// <summary>
        /// Timestamp (RFC3339) reflecting when the request has been handled (second precision, rounded up)
        /// </summary>
        [JsonProperty("currentTime")]
        public string CurrentTime { get; set; }

        /// <summary>
        /// Timestamp (RFC3339) reflecting the time at which all open orders will be cancelled, unless the timer is extended or disabled (second precision, rounded up)
        /// </summary>
        [JsonProperty("triggerTime")]
        public string TriggerTime { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        public CancelAllOrdersAfterStatusResponse() : base(EventName) { }

        public static CancelAllOrdersAfterStatusResponse Timeout(int? requestId = null)
        {
            return new CancelAllOrdersAfterStatusResponse()
            {
                RequestId = requestId,
                Status = Status.Error,
                CurrentTime = string.Empty,
                TriggerTime = string.Empty,
                ErrorMessage = "Timeout",
            };
        }

    }

}
