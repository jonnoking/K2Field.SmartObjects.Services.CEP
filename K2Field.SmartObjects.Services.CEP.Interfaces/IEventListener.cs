using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Interfaces
{
    public interface IEventListener : IDisposable
    {
        Task Start();

        bool IsRunning { get; set; }
        event EventHandler MessageReceived;
        //event EventHandler ListenerError;
        IEventChannelInstance EventChannelInstance { get; set; }
        IEvent Event { get; set; }
    }
}
