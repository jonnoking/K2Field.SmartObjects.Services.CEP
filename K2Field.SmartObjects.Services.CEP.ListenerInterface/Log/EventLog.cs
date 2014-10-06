using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Common.Log
{
    public class EventLog
    {
        public static System.Diagnostics.EventLog GetLog()
        {
            System.Diagnostics.EventLog eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("K2CEPListener"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "K2CEPListener", "K2CEPListenerLog");
            }
            eventLog1.Source = "K2CEPListener";
            eventLog1.Log = "K2CEPListenerLog";

            return eventLog1;
        }
    }
}
