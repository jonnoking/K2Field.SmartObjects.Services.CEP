using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;

namespace K2Field.SmartObjects.Services.CEP.K2CEPListenerService.API
{
    public class EventController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
        public IHttpActionResult Get([FromUri]string origin, [FromUri]string originchannel, [FromUri]string eventsource, [FromUri]string eventtype, [FromUri]string eventdata, [FromUri]string eventdatatype="application/json")
        {
            Model.EventListenerLog ell = new Model.EventListenerLog();
            try
            {
                // find event
                Model.EventListener el = null;
                List<Model.EventListener> lel = Data.EventsData.GetEvents().ToList<Model.EventListener>();

                //el = lel.Where(p => p.Origin.Equals(origin, StringComparison.CurrentCultureIgnoreCase)
                //    && p.OriginChannel.Equals(originchannel, StringComparison.CurrentCultureIgnoreCase)
                //    && p.EventSource.Equals(eventsource, StringComparison.CurrentCultureIgnoreCase)
                //    && p.EventType.Equals(eventtype, StringComparison.CurrentCultureIgnoreCase)
                //    ).FirstOrDefault();

                el = lel.Where(p => p.EventType.Equals(eventtype, StringComparison.CurrentCultureIgnoreCase)
                    ).FirstOrDefault();
                if (el != null)
                {
                    int pid = 0;
                    if (el.Action.Equals("k2 process", StringComparison.CurrentCultureIgnoreCase))
                    {
                        pid = K2.K2Utils.StartWorkflow(el, eventdata, "");
                    }

                    ell = new Model.EventListenerLog()
                    {
                        Origin = el.Origin,
                        OriginChannel = el.OriginChannel,
                        Action = el.Action,
                        EventSource = el.EventSource,
                        EventType = el.EventType,
                        ProcessId = pid.ToString(),
                        ProcessName = el.ProcessName,
                        EventData = eventdata,
                        EventDataType = eventdatatype,
                        EventDate = DateTime.Now,
                        EventListenerId = el.Id,
                    };

                    using (CEP.Data.ApplicationUnit unit = new CEP.Data.ApplicationUnit())
                    {
                        unit.EventListenerLogs.Add(ell);
                        unit.SaveChanges();
                    }
                    //GlobalHost.ConnectionManager.GetHubContext<Hubs.CEPHub>().Clients.Group(eventtype.ToLower()).sendEvent(ell);
                    Push.EventPush.PostEventNotificationToGroups(ell);
                    return (Ok(ell));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
    }
}
