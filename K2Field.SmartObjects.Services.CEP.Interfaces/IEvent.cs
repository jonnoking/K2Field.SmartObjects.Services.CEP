using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Interfaces
{
    public interface IEvent
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }
        string EventName { get; set; }
        string EventDataType { get; set; }
        string EventData { get; set; }
        string EventFilter { get; set; } 
        ObservableCollection<IEventChannelInstance> EventChannels { get; set; }
        ObservableCollection<IEventActionInstance> EventActions { get; set; }
    }
}
