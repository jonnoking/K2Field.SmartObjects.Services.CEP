using EventStore.ClientAPI;
using K2Field.SmartObjects.Services.CEP.ES;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Owin;
using SourceCode.Workflow.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.K2CEPListenerService
{
    public partial class K2CEPListenerService : ServiceBase
    {
        //private System.ComponentModel.IContainer components;
        private System.Diagnostics.EventLog eventLog1;


        public K2CEPListenerService()
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("K2CEPListener"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "K2CEPListener", "K2CEPListenerLog");
            }
            eventLog1.Source = "K2CEPListener";
            eventLog1.Log = "K2CEPListenerLog";

            this.AutoLog = false;
        }


        IEventStoreConnection _connection = null;        
        private List<Model.EventListener> events = new List<Model.EventListener>();

        IDisposable SignalRServer = null;

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Started...");
            GetEvents();
            ListenToEventStore();
            ListenToAzureAsync();


            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            string url = "http://localhost:8181";
            SignalRServer = WebApp.Start<Startup>(url);
            eventLog1.WriteEntry("CEP SignalR server running on " + url, EventLogEntryType.Information);
            //using (WebApp.Start(url))
            //{
                
            //}
        }



        private void GetEvents()
        {
            try
            {
                // Get events to listen for
                using (Data.ApplicationUnit unit = new Data.ApplicationUnit())
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
        }

        private async Task<bool> ConnectToEventStore()
        {
            bool success = false;
            try
            {
                // IP should be configurable
                _connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
                await _connection.ConnectAsync();
                _connection.Closed += _connection_Closed;
                _connection.Disconnected += _connection_Disconnected;

                success = true;
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Error connecting to Event Store. \n\n " + ex.Message, EventLogEntryType.Error);
                success = false;
            }
            return success;
        }

        public void ListenToEventStore()
        {
            ConnectToEventStore().Wait();
            //_connection.SubscribeToAllAsync(false, Appeared, Dropped, EventStoreCredentials.Default);
            foreach (Model.EventListener el in events.Where(p => p.Origin.Equals("event store", StringComparison.CurrentCultureIgnoreCase)).ToList<Model.EventListener>())
            {                
                _connection.SubscribeToStreamAsync(el.EventSource, true, Appeared, Dropped, EventStoreCredentials.Default);
                eventLog1.WriteEntry("Event Store listening to: " + el.EventSource, EventLogEntryType.Information);
            }
        }

        private void Appeared(EventStoreSubscription subscription, ResolvedEvent resolvedEvent)
        {
            Model.EventListener el = null;

            string json = string.Empty;

            el = events.Where(p => p.EventType.Equals(resolvedEvent.Event.EventType, StringComparison.CurrentCultureIgnoreCase) && p.Origin.Equals("event store", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (el != null)
            {
                eventLog1.WriteEntry("Event Store message received: " + resolvedEvent.Event.EventType + " - " + resolvedEvent.Event.EventId, EventLogEntryType.Information);
                // do something
                if (resolvedEvent.Event.IsJson)
                {
                    json = System.Text.Encoding.Default.GetString(resolvedEvent.Event.Data);
                }

                int pid = 0;
                if (el.Action.Equals("workflow", StringComparison.CurrentCultureIgnoreCase))
                {
                    pid = StartWorkflow(el, json, resolvedEvent.Event.EventId.ToString());
                }

                LogEventAsync(el, json, resolvedEvent.Event.EventId.ToString(), pid.ToString(), "success");
                   
           }
        }

        private void Dropped(EventStoreSubscription subscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
            ListenToEventStore();
        }

        public int StartWorkflow(Model.EventListener el, string data, string eventid)
        {
            int procid = 0;
            try
            {
                SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder connectionString =
                new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();
 
                connectionString.Authenticate = true;
                connectionString.Host = "localhost";
                connectionString.Integrated = true;
                connectionString.IsPrimaryLogin = true;
                connectionString.Port = 5252;
                connectionString.UserID = "administrator";
                connectionString.WindowsDomain = "denallix";
                connectionString.Password = "K2pass!";
                connectionString.SecurityLabelName = "K2"; //the default label

                SourceCode.Workflow.Client.Connection k2con = new SourceCode.Workflow.Client.Connection();
                                
                k2con.Open("localhost", connectionString.ToString());
 
                //create process instance
                ProcessInstance processInstance = k2con.CreateProcessInstance(el.ProcessName);

                try
                {
                    processInstance.DataFields["Event Origin"].Value = el.Origin;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Source"].Value = el.EventSource;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Type"].Value = el.EventType;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Id"].Value = eventid;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Data"].Value = data;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Type Id"].Value = el.Id;
                }
                catch { }
                
                //set process folio
                //processInstance.Folio = _processFolio + System.DateTime.Today.ToString();
 
                //start the process
                k2con.StartProcessInstance(processInstance, false);

                procid = processInstance.ID;
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Start Workflow failed: " + el.Origin + " - " + el.EventSource + " - " + el.EventType + " - " + eventid, EventLogEntryType.Error);
            }
            return procid;
        }

        public void PostEventNotification(Model.EventListenerLog ev)
        {
            GlobalHost.ConnectionManager.GetHubContext<CEPHub>().Clients.All.sendEvent(ev);
                
        }

        public async Task LogEventAsync(Model.EventListener el, string data, string eventid, string processinstanceid, string status)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var unit = new Data.ApplicationUnit())
                    {
                        Model.EventListenerLog ell = new Model.EventListenerLog();
                        ell.Action = el.Action;
                        ell.Origin = el.Origin;
                        ell.EventType = el.EventType;
                        ell.EventSource = el.EventSource;
                        ell.EventDisplayName = el.EventDisplayName;
                        ell.ProcessName = el.ProcessName;
                        ell.EventData = data;
                        ell.EventId = eventid;
                        ell.ProcessId = processinstanceid;
                        ell.Status = status;
                        ell.EventDate = DateTime.Now;
                        ell.EventListenerId = el.Id;

                        unit.EventListenerLogs.Add(ell);
                        unit.SaveChanges();

                        PostEventNotification(ell);
                    }
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry("Log Event failed: " + el.Origin + " - " + el.EventSource + " - " + el.EventType + " - " + eventid, EventLogEntryType.Error);
                }
            });            
        }

        public async Task ListenToAzureAsync()
        {
            await Task.Run(() =>
            {
                string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                QueueClient Client = QueueClient.CreateFromConnectionString(connectionString, "demoqueue1");
                eventLog1.WriteEntry("Azure Service Bus connected", EventLogEntryType.Information);
                while (true)
                {
                    BrokeredMessage message = Client.Receive();
                    if (message != null)
                    {
                        Model.EventListener el = events.Where(p => p.EventType.Equals(message.Label, StringComparison.CurrentCultureIgnoreCase) && p.Origin.Equals("azure", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        if (el != null)
                        {
                            string json = string.Empty;
                            try
                            {
                                json = message.GetBody<string>();
                            }
                            catch { }

                            int pid = 0;
                            if (el.Action.Equals("workflow", StringComparison.CurrentCultureIgnoreCase))
                            {
                                pid = StartWorkflow(el, json, message.MessageId);
                            }

                            LogEventAsync(el, json, message.MessageId, pid.ToString(), "success");
                            
                            message.Complete();
                        }
                    } //message.Abandon();
                }

                //return true;
            });
            //return result;
        }


        protected override void OnStop()
        {
            _connection.Close();
            _connection.Dispose();
            SignalRServer.Dispose();
        }



        void _connection_Disconnected(object sender, ClientConnectionEventArgs e)
        {
            eventLog1.WriteEntry("Event Store connection disconnected", EventLogEntryType.Warning);

        }

        void _connection_Closed(object sender, ClientClosedEventArgs e)
        {
            eventLog1.WriteEntry("Event Store connection closed", EventLogEntryType.Warning);

        }

    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
    public class CEPHub : Hub
    {
        public void Send(string name, string message, DateTime msgdatetime)
        {
            Clients.All.addMessage(name, message, msgdatetime);            
        }

        public void SendEvent(Model.EventListenerLog ev)
        {
            Clients.All.sendEvent(ev);
        }
    }
}
