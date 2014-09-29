using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.K2CEPListenerService.Hubs
{
    public class CEPHub : Hub
    {
        public void Send(string name, string message, DateTime msgdatetime)
        {
            Clients.All.addMessage(name, message, msgdatetime);
        }

        // may need to change for groups
        public void SendEvent(Model.EventListenerLog ev)
        {
            Clients.All.sendEvent(ev);
        }
        public Task JoinGroup(string eventtype)
        {
            string[] g;
            if (eventtype.Contains("|"))
            {
                g = eventtype.Split('|');
                for(int i=0; i<g.Length; i++) {
                    g[i] = g[i].Trim();

                    Groups.Add(Context.ConnectionId, g[i].ToLower());
                }
            }

            return Groups.Add(Context.ConnectionId, eventtype.ToLower());
        }
    }

}
