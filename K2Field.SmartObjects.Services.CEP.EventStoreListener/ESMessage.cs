using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K2Field.SmartObjects.Services.CEP.ListenerInterface;
using EventStore.ClientAPI;

namespace K2Field.SmartObjects.Services.CEP.EventStoreListener
{
    public class ESMessage: IEventMessage
    {
        public string MessageId { get; set; }
        public string Body { get; set; }
        public string ContentType { get; set; }
        public DateTime MessageDateTime { get; set; }
        public string Label { get; set; }
        public long Size { get; set; }
        public string ReplyTo { get; set; }
        public string SessionId { get; set; }
        public string To { get; set; }
        public RecordedEvent ESEvent { get; set; }

    }
}
