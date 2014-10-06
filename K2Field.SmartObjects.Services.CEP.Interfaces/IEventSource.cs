using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Interfaces
{
    // A system that we will listen to
    // 
    public interface IEventSource
    {
        string Name { get; set; }
    }
}
