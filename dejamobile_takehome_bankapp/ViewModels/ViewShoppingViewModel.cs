using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using dejamobile_takehome_sdk;
using dejamobile_takehome_bankapp.Events;

namespace dejamobile_takehome_bankapp.ViewModels
{
    public class ViewShoppingViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        public IEventAggregator eventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }

        private List<string> _listOfAvaibleDisplayableCardNumber;
        public List<string> listOfAvaibleDisplayableCardNumber
        {
            get { return _listOfAvaibleDisplayableCardNumber; }
            set { SetProperty(ref _listOfAvaibleDisplayableCardNumber, value) ; }
        }

        private string _selectedDisplayableCardNumber ="";
        public string selectedDisplayableCardNumber
        {
            get { return _selectedDisplayableCardNumber; }
            set { SetProperty(ref _selectedDisplayableCardNumber, value) ; }
        }

        public DelegateCommand<string> onBtnClickBuy { get; set; }
        private List<dejamobile_takehome_sdk.Models.CardModel> availableCards { get; set; }
        private Dictionary<string, dejamobile_takehome_sdk.Models.CardModel> availableCardDictionnary { get; set; }

        public ViewShoppingViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            onBtnClickBuy = new DelegateCommand<string>(executeonBtnClickBuy, canExecuteonBtnClickBuy).ObservesProperty(() => selectedDisplayableCardNumber);
            availableCardDictionnary = new Dictionary<string, dejamobile_takehome_sdk.Models.CardModel>();
            listOfAvaibleDisplayableCardNumber = new List<string>();
            initSubscriptions();
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestArgs(Events.SdkCommandRequestArgs.CommandType.getCards));
        }

        private void buildDictionnary(List<dejamobile_takehome_sdk.Models.CardModel> cards)
        {
            availableCardDictionnary.Clear();
            foreach (dejamobile_takehome_sdk.Models.CardModel card in cards)
            {
                availableCardDictionnary.Add(reformatDisplayableCardNumber(card.cardNumber), card);
            }
        }

        private string reformatDisplayableCardNumber(string s)
        {
            string last4digits = s.Substring(s.Length - 6, 6);
            return "..." + last4digits;
        }

        private bool canExecuteonBtnClickBuy(string arg)
        {
            if (selectedDisplayableCardNumber.Count() == 9)
                return true;
            else
                return false;
        }

        private dejamobile_takehome_sdk.Models.CardModel getSelectedCardFromLast6Digits(string s)
        {
            dejamobile_takehome_sdk.Models.CardModel card;

            availableCardDictionnary.TryGetValue(s, out card);

            if(card != null)
            {
                return card;
            }
            else
            {
                eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.error, "Error while retrieving your payment method :("));
                return null;
            }
        }

        private void executeonBtnClickBuy(string obj)
        {
            dejamobile_takehome_sdk.Models.CardModel card = getSelectedCardFromLast6Digits(selectedDisplayableCardNumber);

            switch (obj)
            {
                case "groceries": //50€
                    eventAggregator.GetEvent<Events.MerchantOrderEvent>().Publish(new Events.MerchantOrderArgs(card, 1));
                    eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.information, "Your order has been placed..."));
                    break;
                case "gpu": //1999€
                    eventAggregator.GetEvent<Events.MerchantOrderEvent>().Publish(new Events.MerchantOrderArgs(card, 2));
                    eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.information, "Your order has been placed..."));
                    break;
                case "smartphone": //2399€
                    eventAggregator.GetEvent<Events.MerchantOrderEvent>().Publish(new Events.MerchantOrderArgs( card, 3));
                    eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.information, "Your order has been placed..."));
                    break;
                case "coffee": //9€
                    eventAggregator.GetEvent<Events.MerchantOrderEvent>().Publish(new Events.MerchantOrderArgs( card, 4));
                    eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.information, "Your order has been placed..."));
                    break;
                default:
                    break;
            }
        }

        private void initSubscriptions()
        {
            eventAggregator.GetEvent<Events.NavigateToEvent>().Subscribe(onNavigateEvents);
            eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Subscribe(onSdkCommandResultEvents);
            eventAggregator.GetEvent<Events.MerchantOrderEvent>().Subscribe(onMerchantOrderEvents);
        }

        private void onMerchantOrderEvents(MerchantOrderArgs obj)
        {
            switch (obj.orderStatus)
            {
                case MerchantOrderArgs.OrderStatus.approved:
                    //send info to user
                    eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new NotificationArgs(NotificationArgs.notificationTypeEnum.success, "Transaction has been successfully processed !"));
                    break;
                case MerchantOrderArgs.OrderStatus.refused:
                    //send info to user
                    eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new NotificationArgs(NotificationArgs.notificationTypeEnum.success, "Transaction refused :("));
                    break;
                default:
                    break;
            }
        }

        private void onCardsRefresh()
        {
            listOfAvaibleDisplayableCardNumber.Clear();
            buildDictionnary(availableCards);

            foreach(KeyValuePair<string, dejamobile_takehome_sdk.Models.CardModel> kvp in availableCardDictionnary)
            {
                listOfAvaibleDisplayableCardNumber.Add(kvp.Key);
            }
        }

        private void onSdkCommandResultEvents(TaskResult obj)
        {
            switch (obj.name)
            {
                case TaskResult.TaskName.getCards:
                    if (obj.result)
                    {
                        availableCards = (List<dejamobile_takehome_sdk.Models.CardModel>) obj.payload;
                        onCardsRefresh();
                    }
                    break;
                default:
                    break;
            }
        }

        private void onNavigateEvents(string obj)
        {
            //refresh cards on navigate to this userControl
            switch (obj)
            {
                case "ViewShopping":
                    eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestArgs(Events.SdkCommandRequestArgs.CommandType.getCards));
                    break;
                default:
                    break;
            }
        }
    }
}
