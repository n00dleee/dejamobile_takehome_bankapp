using dejamobile_takehome_bankapp.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dejamobile_takehome_bankapp.Service.Logs
{
    class LogManager : ServiceTemplate
    {
        public LogManager(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public override void subscribe()
        {
            eventAggregator.GetEvent<Events.LogInfoEvent>().Subscribe(onLogInfoEvents);
        }

        public override void unsubscribe()
        {
            eventAggregator.GetEvent<Events.LogInfoEvent>().Unsubscribe(onLogInfoEvents);
        }

        private void onLogInfoEvents(LogInfoArg obj)
        {
            System.Console.WriteLine("-------------------------------------------\r\nTime : " + DateTime.Now.ToString() + "\r\nLog type : " +
                obj.type.ToString() + "\r\nInfo : " + obj.dataTolog +
                "\r\n-------------------------------------------");

            //we later may handle specific logs differently
            switch (obj.type)
            {
                case LogInfoArg.Type.appInfo:
                    break;
                case LogInfoArg.Type.error:
                    break;
                case LogInfoArg.Type.debug:
                    break;
                default:
                    break;
            }
        }
    }
}
