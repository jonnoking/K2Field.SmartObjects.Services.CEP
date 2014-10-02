using SourceCode.Workflow.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.K2CEPListenerService.K2
{
    public class K2Utils
    {
        private static System.Diagnostics.EventLog eventLog1 = Log.CEPLog.GetLog();

        public static int StartWorkflow(Model.EventListener el, string data, string eventid)
        {
            int procid = 0;
            try
            {
                SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder connectionString =
                new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();

                connectionString.Authenticate = true;
                connectionString.Host = "localhost";
                connectionString.Integrated = true;
                connectionString.IsPrimaryLogin = true;
                connectionString.Port = 5252;
                connectionString.UserID = "administrator";
                connectionString.WindowsDomain = "denallix";
                connectionString.Password = "K2pass!";
                connectionString.SecurityLabelName = "K2"; //the default label

                SourceCode.Workflow.Client.Connection k2con = new SourceCode.Workflow.Client.Connection();

                k2con.Open("localhost", connectionString.ToString());

                //create process instance
                ProcessInstance processInstance = k2con.CreateProcessInstance(el.ProcessName);

                try
                {
                    processInstance.DataFields["Event Origin"].Value = el.Origin;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Source"].Value = el.EventSource;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Type"].Value = el.EventType;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Id"].Value = eventid;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Data"].Value = data;
                }
                catch { }

                try
                {
                    processInstance.DataFields["Event Type Id"].Value = el.Id;
                }
                catch { }

                //set process folio
                //processInstance.Folio = _processFolio + System.DateTime.Today.ToString();

                //start the process
                //k2con.StartProcessInstance(processInstance, false);

                //procid = processInstance.ID;
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Start Workflow failed: " + el.Origin + " - " + el.EventSource + " - " + el.EventType + " - " + eventid, EventLogEntryType.Error);
            }
            return procid;
        }
    }
}
