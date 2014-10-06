using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Interfaces
{
    public interface IEventChannel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }
        string ListenerPrefix { get; set; }
        string ListenerDLLName { get; set; }
        string ListenerDLLPath { get; set; }
        string ListenerClass { get; set; }
        bool IsActive { get; set; }
    }
}
