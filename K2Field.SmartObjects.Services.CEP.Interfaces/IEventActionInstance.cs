using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Interfaces
{
    public interface IEventActionInstance
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }


        IEvent Event { get; set; }
        IEventMessage Message { get; set; }
        
        
        bool IsActive { get; set; }
    }
}
