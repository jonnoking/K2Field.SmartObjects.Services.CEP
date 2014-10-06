using K2Field.SmartObjects.Services.CEP.Interfaces;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.AzureServiceBusListener
{
    class ASBMessage : IEventMessage
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
        public string CorrelationId { get; set; }
        public object RaisedEvent { get; set; }

    }
}
