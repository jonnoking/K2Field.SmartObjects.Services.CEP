using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Sandbox
{
    public class SplunkListener: IListener
    {
        public bool IsRunning { get; set; }
        System.Timers.Timer aTimer = new System.Timers.Timer();
        //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

        public async Task Start()
        {
            aTimer.Interval = 7000;
            aTimer.Enabled = true;

            await Task.Run(() =>
            {
                aTimer.Elapsed += aTimer_Elapsed;
            });
        }

        void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MessageReceivedArgs args = new MessageReceivedArgs();
            args.Message = "splunk";
            OnMessageReceived(args);
        }

        protected virtual void OnMessageReceived(MessageReceivedArgs e)
        {
            EventHandler handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler MessageReceived;
    }
}
