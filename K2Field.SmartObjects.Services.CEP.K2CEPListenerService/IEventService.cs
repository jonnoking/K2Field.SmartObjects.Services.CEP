using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.K2CEPListenerService
{
    interface IEventService
    {
        Task Run();
        Task Refresh(Model.EventListener events);
        
        
    }
}
