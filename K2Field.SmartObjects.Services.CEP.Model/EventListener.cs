using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2Field.SmartObjects.Services.CEP.Model
{
    public class EventListener
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Origin { get; set; }
        public string OriginChannel { get; set; }
        public string EventType { get; set; }
        public string EventSource { get; set; }
        public string Action { get; set; }
        public string ProcessName { get; set; }
        public bool IsActive { get; set; }


    }
}
