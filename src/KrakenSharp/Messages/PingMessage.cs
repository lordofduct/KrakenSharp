using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrakenSharp.Messages
{

    public class PingMessage : KrakenMessage
    {
        public const string EventName = "ping";

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <remarks>
        /// client originated requestID sent as acknowledgment in the message response
        /// </remarks>
        /// <value>
        /// The request identifier.
        /// </value>
        [JsonProperty("reqid")]
        public int? RequestId { get; set; }

        public PingMessage() : base(EventName) { }

    }

    public class PongMessage : KrakenMessage
    {
        /// <summary>
        /// The unique event name.
        /// </summary>
        public const string EventName = "pong";


        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <remarks>
        /// client originated requestID sent as acknowledgment in the message response
        /// </remarks>
        /// <value>
        /// The request identifier.
        /// </value>
        [JsonProperty("reqid")]
        public int? RequestId { get; set; }

        public PongMessage() : base(EventName) { }

    }

}
