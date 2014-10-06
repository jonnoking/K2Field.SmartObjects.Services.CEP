using K2Field.SmartObjects.Services.CEP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Common
{
    public class MessageReceivedArgs : EventArgs
    {
        public IEventChannelInstance EventChannelInstance { get; set; }
        public IEventMessage EventMessage { get; set; }
        public DateTime EventTimestamp { get; set; }
    }
}
