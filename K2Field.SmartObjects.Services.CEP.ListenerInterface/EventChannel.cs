using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.ListenerInterface
{
    public class EventChannel
    {
        public string Origin { get; set; }
        public string OriginChannel { get; set; }
        public string EventSource { get; set; }
        public string EventType { get; set; }
        public string EventSubType { get; set; }

    }
}
