using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K2Field.SmartObjects.Services.CEP.Interfaces;

namespace K2Field.SmartObjects.Services.CEP.Model
{
    public class EventChannel : IEventChannel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ListenerPrefix { get; set; }
        public string ListenerDLLName { get; set; }
        public string ListenerDLLPath { get; set; }
        public string ListenerClass { get; set; }
        public bool IsActive { get; set; }
    }
}
