using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Interfaces
{
    public interface IEventChannelInstance
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }
        string Source { get; set; } // stream, queue, topic:subscription, http url, email address ?
        string ConnectionString { get; set; }
        string ClientKey { get; set; }
        string ClientSecret { get; set; }
        string IsActive { get; set; }
        IEventChannel EventChannel { get; set; } // Azure Service Bus, Event Store, Splunk, HTTP, Email
    }
}
