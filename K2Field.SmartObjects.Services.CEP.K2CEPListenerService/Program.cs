using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.K2CEPListenerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new K2CEPListenerService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
