using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Model
{
    public class EventListenerLog
    {
        public Guid Id { get; set; }
        public string Origin { get; set; }
        public string OriginChannel { get; set; }
        public string EventSource { get; set; }
        public string EventType { get; set; }
        public string Action { get; set; }
        public string ProcessName { get; set; }
        public string ProcessId { get; set; }
        public string EventData { get; set; }
        public string EventDataType { get; set; }
        public DateTime EventDate { get; set; }
        public string EventId { get; set; }
        public Guid EventListenerId { get; set; }
    }
}
