using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Interfaces
{
    public interface IEventAction
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }
        string ActionPrefix { get; set; }
        string ActionDLLName { get; set; }
        string ActionDLLPath { get; set; }
        string ActionClass { get; set; }
        bool IsActive { get; set; }
    }
}
