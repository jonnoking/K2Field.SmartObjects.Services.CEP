using EventStore.ClientAPI;
using K2Field.SmartObjects.Services.CEP.Data;
using K2Field.SmartObjects.Services.CEP.ES;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace K2Field.SmartObjects.Services.CEP.Sandbox
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            //// Set the Interval to 2 seconds (2000 milliseconds).
            //aTimer.Interval = 10000;
            //aTimer.Enabled = true;


        }


        IEventStoreConnection _connection = null;
        private List<Model.EventListener> events = new List<Model.EventListener>();

        // Create a timer with a ten second interval.
        System.Timers.Timer aTimer = new System.Timers.Timer();

        // Hook up the Elapsed event for the timer.

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);

            using (Data.ApplicationUnit unit = new ApplicationUnit())
            {
                events = unit.EventListeners.All(p => p.Origin.Equals("event store", StringComparison.CurrentCultureIgnoreCase)).ToList<Model.EventListener>();
            }
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            // will reset database
            Database.SetInitializer(new DropCreateDatabaseAlways<Data.CEPListenerContext>());

            CreateDatabase();

            //_connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            //_connection.ConnectAsync();

            ////List<Model.EventListener> events = new List<Model.EventListener>();

            //using(Data.ApplicationUnit unit = new ApplicationUnit())
            //{
            //    events = unit.EventListeners.All(p => p.Origin.Equals("event store", StringComparison.CurrentCultureIgnoreCase)).ToList<Model.EventListener>();
            //}

            //StartReading();
            //ListenToAzureAsync();
            //btnInit.Text = "Running";
        }

        public void StartReading()
        {
            _connection.SubscribeToAllAsync(false, Appeared, Dropped, EventStoreCredentials.Default);
            //foreach(Model.EventListener el in events)
            //{
            //    _connection.SubscribeToStreamAsync(el.EventSource, true, Appeared, Dropped, EventStoreCredentials.Default);
            //}
        }


        private void ListenToEventStore(List<Model.EventListener> events)
        {
            StartReading();
        }

        private void Appeared(EventStoreSubscription subscription, ResolvedEvent resolvedEvent)
        {
            Model.EventListener el = null;

            string json = string.Empty;

            el = events.Where(p => p.EventType.Equals(resolvedEvent.Event.EventType, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (el != null)
            {
                    
                // do something
                if(resolvedEvent.Event.IsJson)
                {
                    json = System.Text.Encoding.Default.GetString(resolvedEvent.Event.Data);
                }

                Console.WriteLine("EVENT: " + resolvedEvent.Event.EventType + " - " + resolvedEvent.Event.EventId);


                //SourceCode.Workflow.Client.Connection k2con = new SourceCode.Workflow.Client.Connection();
                //k2con.Open("localhost", "administrator,K2pass!,denallix");

                //SourceCode.Workflow.Client.ProcessInstance pi = k2con.CreateProcessInstance(el.ProcessName);

                //k2con.StartProcessInstance(pi);
                    

            }


            //Console.WriteLine("WEB: " + resolvedEvent.Event.EventType);
            // do something with the events here
            // var @event = resolvedEvent.ParseJson();
        }

        private void Dropped(EventStoreSubscription subscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
            // is called when the tcp connection is dropped, we could
            // implement recovery here
            //_connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            StartReading();
        }


        //azure service bus listener
        public async Task ListenToAzureAsync()
        {
            await Task.Run(() =>
            {
                string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                //string connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
                //string connectionString = "Endpoint=sb://k2field.servicebus.windows.net/;SharedSecretIssuer=owner;SharedSecretValue=llu264DIUCEy8W1h56oMlCQobjQUYM7oLL2BLNDvQuw=";
                var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                QueueClient Client = QueueClient.CreateFromConnectionString(connectionString, "demoqueue1");


                // Continuously process messages sent to the "TestQueue" 
                while (true)
                {
                    BrokeredMessage message = Client.Receive();
                    PrintAsync(message);
                    //Client.ReceiveAsync().ContinueWith(t => PrintAsync(t.Result));
                    //Client.ReceiveAsync().ContinueWith<Task<bool>>(t => PrintAsync(t.Result));



                    //PrintAsync(message);
                    //if (message != null)
                    //{
                    //try
                    //{
                    //    Console.WriteLine("Body: " + message.GetBody<string>());
                    //    Console.WriteLine("MessageID: " + message.MessageId);
                    //    //Console.WriteLine("Test Property: " + message.GetBody<string>());

                    //    // Remove message from queue
                    //    message.Complete();
                    //}
                    //catch (Exception)
                    //{
                    //    // Indicate a problem, unlock message in queue
                    //    message.Abandon();
                    //}
                    //}
                }

                //return true;
            });
            //return result;
        }

        public async Task<bool> PrintAsync(BrokeredMessage message)
        {
            var result = await Task.Run(() =>
            {
                //BrokeredMessage message = msg.Result;
                bool res = false;
                if (message != null)
                {

                    try
                    {
                        using (var unit = new ApplicationUnit())
                        {
                            Model.EventListenerLog el = new Model.EventListenerLog();
                            el.Action = "K2 Process";
                            el.Origin = "Azure Service Bus";
                            el.EventType = "TestEvent";
                            el.EventSource = "jonnoStream";
                            
                            el.ProcessName = @"CEP\CustomerReview";
                            el.EventData = message.MessageId + " - " + message.GetBody<string>();
                            el.EventDate = DateTime.Now;

                            unit.EventListenerLogs.Add(el);
                            unit.SaveChanges();
                        }


                        //Console.WriteLine("Body: " + message.GetBody<string>());
                        //Console.WriteLine("MessageID: " + message.MessageId);
                        //Console.WriteLine("Test Property: " + message.GetBody<string>());

                        // Remove message from queue
                        message.Complete();
                        res = true;
                    }
                    catch (Exception)
                    {
                        // Indicate a problem, unlock message in queue
                        message.Abandon();
                        res = false;
                    }
                }

                //Thread.Sleep(3000);
                return res;
            });
            return result;
        }


        void CreateDatabase()
        {
            using (var unit = new ApplicationUnit())
            {
                Model.EventListener el = new Model.EventListener();
                el.DisplayName = "ES Test Event";
                el.Name = "estestevent";
                el.Action = "K2 Process";
                el.Origin = "Event Store";
                el.OriginChannel = "stream";
                el.EventSource = "jonnoStream";
                el.EventType = "TestEvent";
                el.ProcessName = @"CEP\CustomerReview";
                el.IsActive = true;

                Model.EventListener el1 = new Model.EventListener();
                el1.DisplayName = "ES Customer Event";
                el1.Name = "escustomerevent";
                el1.Action = "K2 Process";
                el1.Origin = "Event Store";
                el1.OriginChannel = "stream";
                el1.EventSource = "customerStream";
                el1.EventType = "customerevent";
                el1.ProcessName = @"CEP\CustomerReview";
                el1.IsActive = true;

                Model.EventListener el2 = new Model.EventListener();
                el2.DisplayName = "ASB Customer Event";
                el2.Name = "asbcustomerevent";
                el2.Action = "K2 Process";
                el2.Origin = "Azure Service Bus";
                el2.OriginChannel = "queue";
                el2.EventSource = "demoqueue1";
                el2.EventType = "customerevent";
                el2.ProcessName = @"CEP\CustomerReview";
                el2.IsActive = true;

                unit.EventListeners.Add(el);
                unit.EventListeners.Add(el1);
                unit.EventListeners.Add(el2);
                unit.SaveChanges();

            }

        }
    }
}
