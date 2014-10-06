using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using K2Field.SmartObjects.Services.CEP.Interfaces;

namespace K2Field.SmartObjects.Services.CEP.Model
{
    public class Event : IEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string EventName { get; set; }
        public string EventDataType { get; set; }
        public string EventData { get; set; }
        public string EventFilter { get; set; } // for future...?
        public ObservableCollection<IEventChannelInstance> EventChannels { get; set; }
        public ObservableCollection<IEventActionInstance> EventActions { get; set; }
    }
}
