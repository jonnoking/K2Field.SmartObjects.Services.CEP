using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.ListenerInterface
{
    public class MessageReceivedArgs : EventArgs
    {
        public EventChannel EventChannel { get; set; }
        public IEventMessage EventMessage { get; set; }
        public DateTime EventTimestamp { get; set; }
    }
}
