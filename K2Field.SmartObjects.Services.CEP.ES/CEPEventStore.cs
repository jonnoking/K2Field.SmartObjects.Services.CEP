using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.ES
{

    public class CEPEventStoreListener
    {
        private IEventStoreConnection _connection = null;

        public CEPEventStoreListener()
        {
            _connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
        }

        private void ListenToEventStore()
        {
            StartReading();
        }

        public void StartReading()
        {
            //_connection.SubscribeToAllFrom(true, Appeared, Dropped, EventStoreCredentials.Default);
            _connection.SubscribeToStreamAsync("jonnoStream", true, Appeared, Dropped, EventStoreCredentials.Default);

            //_connection.SubscribeToAllAsync(true, Appeared, Dropped, EventStoreCredentials.Default);
        }

        private void Appeared(EventStoreSubscription subscription, ResolvedEvent resolvedEvent)
        {
            Console.WriteLine("WEB: " + resolvedEvent.Event.EventType);
            // do something with the events here
            // var @event = resolvedEvent.ParseJson();
        }

        private void Dropped(EventStoreSubscription subscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
            // is called when the tcp connection is dropped, we could
            // implement recovery here
            //_connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            //StartReading();
        }


    }


    public class CEPEventStore
    {
        private static EventData ToEventData(Guid eventId, object @event, string eventName = null, IDictionary<string, object> headers = null)
        {
            if (headers == null) headers = new Dictionary<string, object>();
            var serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };

            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, serializerSettings));

            var eventHeaders = new Dictionary<string, object>(headers)
                    {
                        {
                            "EventClrTypeName", @event.GetType().AssemblyQualifiedName
                        }
                    };
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, serializerSettings));

            string typeName = string.Empty;
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                typeName = eventName;
            }
            else
            {
                typeName = @event.GetType().Name;
            }


            return new EventData(eventId, typeName, true, data, metadata);
        }

    }

    internal class IPEndPointFactory
    {
        public static IPEndPoint DefaultTcp()
        {
            return CreateIPEndPoint(1113);
        }
        public static IPEndPoint DefaultHttp()
        {
            return CreateIPEndPoint(2113);
        }
        private static IPEndPoint CreateIPEndPoint(int port)
        {
            var address = IPAddress.Parse("127.0.0.1");
            return new IPEndPoint(address, port);
        }
    }

    public static class EventStoreConnectionFactory
    {
        public static IEventStoreConnection Default()
        {
            var connection = EventStoreConnection.Create(IPEndPointFactory.DefaultTcp());
            connection.ConnectAsync();
            return connection;
        }
    }

    public class EventStoreCredentials
    {
        private static readonly UserCredentials _credentials =
            new UserCredentials("admin", "changeit");

        public static UserCredentials Default { get { return _credentials; } }
    }

    public class ProjectionList
    {
        public List<Projection> Projections { get; set; }
    }

    public class Projection
    {
        public int coreProcessingTime { get; set; }
        public int version { get; set; }
        public int epoch { get; set; }
        public string effectiveName { get; set; }
        public int writesInProgress { get; set; }
        public int readsInProgress { get; set; }
        public int partitionsCached { get; set; }
        public string status { get; set; }
        public string stateReason { get; set; }
        public string name { get; set; }
        public string mode { get; set; }
        public string position { get; set; }
        public float progress { get; set; }
        public string lastCheckpoint { get; set; }
        public int eventsProcessedAfterRestart { get; set; }
        public string statusUrl { get; set; }
        public string stateUrl { get; set; }
        public string resultUrl { get; set; }
        public string queryUrl { get; set; }
        public string enableCommandUrl { get; set; }
        public string disableCommandUrl { get; set; }
        public string checkpointStatus { get; set; }
        public int bufferedEvents { get; set; }
        public int writePendingEventsBeforeCheckpoint { get; set; }
        public int writePendingEventsAfterCheckpoint { get; set; }
    }

}
