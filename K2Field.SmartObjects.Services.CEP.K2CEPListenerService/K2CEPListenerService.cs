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
using System.Web.Http;

namespace K2Field.SmartObjects.Services.CEP.K2CEPListenerService
{
    public partial class K2CEPListenerService : ServiceBase
    {
        //private System.ComponentModel.IContainer components;
        private System.Diagnostics.EventLog eventLog1;


        public K2CEPListenerService()
        {
            InitializeComponent();
            //eventLog1 = new System.Diagnostics.EventLog();
            //if (!System.Diagnostics.EventLog.SourceExists("K2CEPListener"))
            //{
            //    System.Diagnostics.EventLog.CreateEventSource(
            //        "K2CEPListener", "K2CEPListenerLog");
            //}
            //eventLog1.Source = "K2CEPListener";
            //eventLog1.Log = "K2CEPListenerLog";

            eventLog1 = Log.CEPLog.GetLog();

            this.AutoLog = false;
        }


        IEventStoreConnection _connection = null;        
        private List<Model.EventListener> events = new List<Model.EventListener>();

        IDisposable SignalRServer = null;
        IDisposable WebApiServer = null;

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Started...");
            events = Data.EventsData.GetEvents();
            ListenToEventStore();
            ListenToAzureAsync();


            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            string url = "http://localhost:8181";
            SignalRServer = WebApp.Start<Start.Startup>(url);
            eventLog1.WriteEntry("CEP SignalR server running on " + url, EventLogEntryType.Information);
            //using (WebApp.Start(url))
            //{
                
            //}

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
                if (el.Action.Equals("k2 process", StringComparison.CurrentCultureIgnoreCase))
                {
                    pid = K2.K2Utils.StartWorkflow(el, json, resolvedEvent.Event.EventId.ToString());
                }

                Data.EventsData.LogEventAsync(el, json, resolvedEvent.Event.EventId.ToString(), "application/json", pid.ToString());
                   
           }
        }

        private void Dropped(EventStoreSubscription subscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
            ListenToEventStore();
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
                        Model.EventListener el = events.Where(p => p.EventType.Equals(message.Label, StringComparison.CurrentCultureIgnoreCase) && p.Origin.Equals("azure service bus", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        if (el != null)
                        {
                            string json = string.Empty;
                            try
                            {
                                json = message.GetBody<string>();
                            }
                            catch { }

                            int pid = 0;
                            if (el.Action.Equals("k2 process", StringComparison.CurrentCultureIgnoreCase))
                            {
                                pid = K2.K2Utils.StartWorkflow(el, json, message.MessageId);
                            }

                            string ct = string.Empty;
                            if (!string.IsNullOrWhiteSpace(message.ContentType))
                            {
                                ct = message.ContentType;
                            }
                            else
                            {
                                ct = "application/json";
                            }

                            Data.EventsData.LogEventAsync(el, json, message.MessageId, ct, pid.ToString());
                            
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

    //class Startup
    //{
    //    public void Configuration(IAppBuilder app)
    //    {
    //        app.UseCors(CorsOptions.AllowAll);
    //        app.MapSignalR();

    //        HttpConfiguration config = new HttpConfiguration();
    //        config.Routes.MapHttpRoute(
    //            name: "DefaultApi",
    //            routeTemplate: "api/{controller}/{id}",
    //            defaults: new { id = RouteParameter.Optional }
    //        );

    //        app.UseWebApi(config); 
    //    }
    //}


    // To Do
    /* 
     *** CORE ***
     * Modular inferface driven services & actions e.g. start wf, log to db
     * Externalized connections - event store, azure, db, signalr, k2
     * Refresh event read from DB - on signalr event? on timer?
     * Log event read refresh datetime
     * Add webapi to services - GET interface to post an event - generic? per queue
     * 
     *** Azure ***
     * listener per registered event
     * listeners for queue, topic, subscription
     * 
     *** Event Store ***
     * 
     * 
     *** Splunk ***
     * Implement
     * 
     *** Virtual Event ***
     * Signalr based event fired from SF or WF
     * 
     * 
     */
}
