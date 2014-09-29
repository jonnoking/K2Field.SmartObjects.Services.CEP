using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.K2CEPListenerService.Push
{
    public class EventPush
    {
        private static System.Diagnostics.EventLog eventLog1 = Log.CEPLog.GetLog();

        public static void PostEventNotificationToGroups(Model.EventListenerLog ev)
        {
            GlobalHost.ConnectionManager.GetHubContext<Hubs.CEPHub>().Clients.Group(ev.EventType.ToLower()).sendEvent(ev);
        }
        public static void PostEventNotificationToAll(Model.EventListenerLog ev)
        {
            GlobalHost.ConnectionManager.GetHubContext<Hubs.CEPHub>().Clients.All.sendEvent(ev);
        }

    }
}
