using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dejamobile_takehome_bankapp.Events;

namespace dejamobile_takehome_bankapp.Service.Sdk
{
    public class SdkManager : ServiceTemplate
    {

        public SdkManager(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public override void subscribe()
        {
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Subscribe(onSdkCommandRequestEvents);
        }

        private void onSdkCommandRequestEvents(SdkCommandRequestEventArgs arg)
        {
            switch (arg.commandType)
            {
                case SdkCommandRequestEventArgs.CommandType.createUser:
                    break;
                case SdkCommandRequestEventArgs.CommandType.login:

                    break;
                case SdkCommandRequestEventArgs.CommandType.addCard:
                    break;
                case SdkCommandRequestEventArgs.CommandType.getCards:
                    break;
                case SdkCommandRequestEventArgs.CommandType.deleteCard:
                    break;
                case SdkCommandRequestEventArgs.CommandType.getStats:
                    break;
                default:
                    break;
            }
        }

        public override void unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
