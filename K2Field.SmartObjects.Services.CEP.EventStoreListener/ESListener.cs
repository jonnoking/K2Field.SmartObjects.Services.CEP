using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K2Field.SmartObjects.Services.CEP.ListenerInterface;
using EventStore.ClientAPI;
using System.Net;
using System.Diagnostics;
using K2Field.SmartObjects.Services.CEP.ES;

namespace K2Field.SmartObjects.Services.CEP.EventStoreListener
{
    public class ESListener : IEventListener
    {
        public string ConnectionString { get; set; }
        public string ClientKey { get; set; }
        public string ClientSecret { get; set; }

        public bool IsRunning { get; set; }

        public EventChannel EventChannel { get; set; }

        private System.Diagnostics.EventLog eventLog;
        IEventStoreConnection _connection = null;

        string ESIP = string.Empty;
        string ESPort = string.Empty;

        public ESListener()
        {
            eventLog = ListenerInterface.Log.EventLog.GetLog();
        }

        public async Task Start()
        {
            await Task.Run(() =>
                {
                    string[] ESCon = ConnectionString.Split(':');
                    if (ESCon != null && ESCon.Length == 2)
                    {
                        ESIP = ESCon[0];
                        ESPort = ESCon[1];
                    }
                    ListenToEventStore();
                });
        }

        private async Task<bool> ConnectToEventStore()
        {
            bool success = false;
            try
            {
                // IP should be configurable
                _connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Parse(ESIP), int.Parse(ESPort)));
                await _connection.ConnectAsync();
                _connection.Closed += _connection_Closed;
                _connection.Disconnected += _connection_Disconnected;

                success = true;
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Error connecting to Event Store. \n\n " + ex.Message, EventLogEntryType.Error);
                success = false;
            }
            return success;
        }

        public void ListenToEventStore()
        {
            try
            {
                ConnectToEventStore().Wait();
                //_connection.SubscribeToAllAsync(false, Appeared, Dropped, EventStoreCredentials.Default);

                _connection.SubscribeToStreamAsync(EventChannel.EventSource, true, Appeared, Dropped, EventStoreCredentials.Default);

                eventLog.WriteEntry("Event Store listening to: " + EventChannel.EventSource, EventLogEntryType.Information);
            }
            catch(Exception ex)
            {
                eventLog.WriteEntry("Event Store failed at ListenToEventStore() " + ex.Message, EventLogEntryType.Error);
            }
        }

        private void Appeared(EventStoreSubscription subscription, ResolvedEvent resolvedEvent)
        {
            try
            {
                ListenerInterface.MessageReceivedArgs args = new MessageReceivedArgs();
                args.EventChannel = this.EventChannel;

                ESMessage esm = new ESMessage()
                {
                    MessageId = resolvedEvent.Event.EventId.ToString(),
                    MessageDateTime = resolvedEvent.Event.Created,
                    ContentType = resolvedEvent.Event.IsJson ? "application/json" : "text",
                    Body = resolvedEvent.Event.IsJson ? System.Text.Encoding.Default.GetString(resolvedEvent.Event.Data) : "SOME BYTE ARRAY - NEED TO IMPLEMENTED",
                    ESEvent = resolvedEvent.Event
                };

                args.EventMessage = esm as IEventMessage;
                args.EventTimestamp = DateTime.Now;
                OnMessageReceived(args);
            }
            catch(Exception ex)
            {
                eventLog.WriteEntry("Event Store failed at Appeared() " + ex.Message, EventLogEntryType.Error);
            }
            
        }

        private void Dropped(EventStoreSubscription subscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
            ListenToEventStore();
        }


        void _connection_Disconnected(object sender, ClientConnectionEventArgs e)
        {
            eventLog.WriteEntry("Event Store connection disconnected", EventLogEntryType.Warning);

        }

        void _connection_Closed(object sender, ClientClosedEventArgs e)
        {
            eventLog.WriteEntry("Event Store connection closed", EventLogEntryType.Warning);

        }

        protected virtual void OnMessageReceived(MessageReceivedArgs e)
        {
            EventHandler handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler MessageReceived;

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
