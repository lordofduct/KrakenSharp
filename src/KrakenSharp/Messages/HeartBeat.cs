using System;
using System.Collections.Generic;
using System.Text;

namespace KrakenSharp.Messages
{
    public sealed class Heartbeat : KrakenMessage
    {
        public const string EventName = "heartbeat";

        public new static readonly Heartbeat Empty = new Heartbeat();

        public Heartbeat() : base(EventName)
        { }
    }
}
