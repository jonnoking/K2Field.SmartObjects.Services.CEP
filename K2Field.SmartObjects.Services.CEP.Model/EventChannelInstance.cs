using K2Field.SmartObjects.Services.CEP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Model
{
    public class EventChannelInstance : IEventChannelInstance
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Source { get; set; } // stream, queue, topic:subscription, http url, email address ?
        public string ConnectionString { get; set; }
        public string ClientKey { get; set; }
        public string ClientSecret { get; set; }
        public string IsActive { get; set; }
        public IEventChannel EventChannel { get; set; } // Azure Service Bus, Event Store, Splunk, HTTP, Email
    }
}
