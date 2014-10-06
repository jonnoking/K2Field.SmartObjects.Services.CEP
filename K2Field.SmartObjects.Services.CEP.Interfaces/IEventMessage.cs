using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Interfaces
{
    public interface IEventMessage
    {
        string MessageId { get; set; }
        string Body { get; set; }
        string ContentType { get; set; }
        DateTime MessageDateTime { get; set; }
        string Label { get; set; }
        long Size { get; set; }
        string ReplyTo { get; set; }
        string SessionId { get; set; }
        string To { get; set; }
        object RaisedEvent { get; set; }
    }
}
