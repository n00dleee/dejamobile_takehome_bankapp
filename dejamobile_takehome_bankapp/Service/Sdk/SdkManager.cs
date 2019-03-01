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

        private async void onSdkCommandRequestEvents(SdkCommandRequestArgs arg)
        {
            try
            {
                TaskResult taskResult;
                switch (arg.commandType)
                {
                    case SdkCommandRequestArgs.CommandType.createUser:
                        taskResult = await sdk.CreateUser((dejamobile_takehome_sdk.Models.UserModel)arg.payload);
                        eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Publish(taskResult);
                        break;
                    case SdkCommandRequestArgs.CommandType.login:
                        taskResult = await sdk.ConnectUser((dejamobile_takehome_sdk.Models.UserModel)arg.payload);
                        eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Publish(taskResult);
                        break;
                    case SdkCommandRequestArgs.CommandType.addCard:
                        taskResult = await sdk.AddCard((dejamobile_takehome_sdk.Models.CardModel)arg.payload);
                        eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Publish(taskResult);
                        break;
                    case SdkCommandRequestArgs.CommandType.getCards:
                        taskResult = sdk.getDigitizedCardsList();
                        eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Publish(taskResult);
                        break;
                    case SdkCommandRequestArgs.CommandType.deleteCard:
                        taskResult = sdk.deleteDigitizedCard((string)arg.payload);
                        eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Publish(taskResult);
                        break;
                    case SdkCommandRequestArgs.CommandType.getStats:
                        break;
                    default:
                        break;
                }
            }catch
            {
                eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new NotificationArgs(NotificationArgs.notificationTypeEnum.error, "Unexpected error in SDK manager service... be sure backend is running and reachable"));
            }
        }

        public override void unsubscribe()
        {
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Unsubscribe(onSdkCommandRequestEvents);
        }
    }
}
