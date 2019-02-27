using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dejamobile_takehome_bankapp.Events;
using dejamobile_takehome_sdk;

namespace dejamobile_takehome_bankapp.Service.Sdk
{
    public class SdkManager : ServiceTemplate
    {
        dejamobile_takehome_sdk.Sdk sdk;

        public SdkManager(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            sdk = new dejamobile_takehome_sdk.Sdk(true);
        }

        public override void subscribe()
        {
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Subscribe(onSdkCommandRequestEvents);
        }

        private async void onSdkCommandRequestEvents(SdkCommandRequestEventArgs arg)
        {
            TaskResult taskResult;
            switch (arg.commandType)
            {
                case SdkCommandRequestEventArgs.CommandType.createUser:
                    taskResult = await sdk.CreateUser((dejamobile_takehome_sdk.Models.UserModel)arg.payload);
                    eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Publish(taskResult);
                    break;
                case SdkCommandRequestEventArgs.CommandType.login:
                    taskResult = await sdk.ConnectUser((dejamobile_takehome_sdk.Models.UserModel)arg.payload);
                    eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Publish(taskResult);
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
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Unsubscribe(onSdkCommandRequestEvents);
        }
    }
}
