using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.K2CEPListenerService.Data
{
    public class EventsData
    {
        private static System.Diagnostics.EventLog eventLog1 = Log.CEPLog.GetLog();

        public static async Task LogEventAsync(Model.EventListener el, string data, string datatype, string eventid, string processinstanceid)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var unit = new CEP.Data.ApplicationUnit())
                    {
                        Model.EventListenerLog ell = new Model.EventListenerLog();
                        ell.DisplayName = el.DisplayName;
                        ell.Name = el.Name;
                        ell.Action = el.Action;
                        ell.Origin = el.Origin;
                        ell.OriginChannel = el.OriginChannel;
                        ell.EventType = el.EventType;
                        ell.EventSource = el.EventSource;
                        ell.ProcessName = el.ProcessName;
                        ell.EventData = data;
                        ell.EventDataType = datatype;
                        ell.EventId = eventid;
                        ell.ProcessId = processinstanceid;
                        ell.EventDate = DateTime.Now;
                        ell.EventListenerId = el.Id;

                        unit.EventListenerLogs.Add(ell);
                        unit.SaveChanges();

                        Push.EventPush.PostEventNotificationToGroups(ell);
                    }
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry("Log Event failed: " + el.Origin + " - " + el.EventSource + " - " + el.EventType + " - " + eventid, EventLogEntryType.Error);
                }
            });
        }

        public static List<Model.EventListener> GetEvents()
        {
            List<Model.EventListener> events = new List<Model.EventListener>();
            try
            {
                // Get events to listen for
                using (CEP.Data.ApplicationUnit unit = new CEP.Data.ApplicationUnit())
                {
                    //events = unit.EventListeners.All(p => p.Origin.Equals("event store", StringComparison.CurrentCultureIgnoreCase)).ToList<Model.EventListener>();
                    events = unit.EventListeners.All().ToList();
                    eventLog1.WriteEntry("Events #: " + events.Count, EventLogEntryType.Information);
                }
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Error getting Events from database. \n\n " + ex.Message, EventLogEntryType.Error);
            }

            return events;
        }

    }
}
