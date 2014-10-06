using K2Field.SmartObjects.Services.CEP.Common;
using K2Field.SmartObjects.Services.CEP.Interfaces;
using K2Field.SmartObjects.Services.CEP.Model;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.AzureServiceBusListener
{
    public class ASBQueueListener : IEventListener
    {
        public bool IsRunning { get; set; }

        public IEventChannelInstance EventChannelInstance { get; set; }
        public IEvent Event { get; set; }

        private System.Diagnostics.EventLog eventLog;


        public ASBQueueListener()
        {
            eventLog = Common.Log.EventLog.GetLog();

        }

        public async Task Start()
        {
            await Task.Run(() =>
                {
                    ListenToAzureQueueAsync();
                });
        }

        public async Task ListenToAzureQueueAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    //string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                    var namespaceManager = NamespaceManager.CreateFromConnectionString(EventChannelInstance.ConnectionString);
                    QueueClient Client = QueueClient.CreateFromConnectionString(EventChannelInstance.ConnectionString, EventChannelInstance.Source);
                    eventLog.WriteEntry("Azure Service Bus Queue connected", EventLogEntryType.Information);
                    while (true)
                    {
                        BrokeredMessage message = Client.Receive();
                        if (message != null)
                        {
                            ProcessMessage(message);
                        }
                    } //message.Abandon();
                }
                catch(Exception ex)
                {
                    eventLog.WriteEntry("Azure Service Bus failed to connect " + ex.Message, EventLogEntryType.Error);
                }
            });
            //return true;
        }

        void ProcessMessage(BrokeredMessage message)
        {
            try
            {
                ASBMessage msg = new ASBMessage()
                {
                    MessageId = message.MessageId,
                    ContentType = message.ContentType != null ? message.ContentType : "",
                    Label = message.Label != null ? message.Label : "",
                    MessageDateTime = message.EnqueuedTimeUtc != null ? message.EnqueuedTimeUtc : DateTime.Now,
                    ReplyTo = message.ReplyTo != null ? message.ReplyTo : "",
                    SessionId = message.SessionId != null ? message.SessionId : "",
                    Size = message.Size != null ? message.Size : 0,
                    To = message.To != null ? message.To : "",
                    CorrelationId = message.CorrelationId != null ? message.CorrelationId : "",
                };
                bool error = false;
                try
                {
                    switch (msg.ContentType)
                    {
                        case "":
                        case "json":
                        case "application/json":
                        case "text":
                        case "text/xml":
                        case "application/xml":
                            msg.Body = message.GetBody<string>();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    error = true;
                    eventLog.WriteEntry("Azure Service Bus failed at deserialize message " + ex.Message, EventLogEntryType.Error);
                    msg.Body = "FAILED to deserialize content";
                }

                MessageReceivedArgs args = new MessageReceivedArgs()
                {
                    EventChannelInstance = EventChannelInstance,
                    EventMessage = msg,
                    EventTimestamp = DateTime.Now
                };
                OnMessageReceived(args);

                message.Complete();
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Azure Service Bus failed at Process Message " + ex.Message, EventLogEntryType.Error);
            }
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
        }
    }
}
