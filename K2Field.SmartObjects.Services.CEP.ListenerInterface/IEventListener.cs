using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.ListenerInterface
{
    public interface IEventListener : IDisposable
    {
        Task Start();

        bool IsRunning { get; set; }
        event EventHandler MessageReceived;
        //event EventHandler ListenerError;


        string ConnectionString { get; set; }
        string ClientKey { get; set; }
        string ClientSecret { get; set; }

        EventChannel EventChannel { get; set; }
    }
}
