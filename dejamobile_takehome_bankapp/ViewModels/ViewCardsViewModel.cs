using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using dejamobile_takehome_sdk;
using System.Windows;

namespace dejamobile_takehome_bankapp.ViewModels
{
    public class ViewCardsViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        public IEventAggregator eventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }

        List<dejamobile_takehome_sdk.Models.CardModel> cardList { get; set; }

        public DelegateCommand<string> onBtnClickAddCard { get; set; }
        public DelegateCommand<string> onBtnClickValidateCardCreation { set; get; }
        public DelegateCommand<string> onBtnClickCancelCardCreation { set; get; }
        public DelegateCommand<string> onBtnClickCardNumberAutoFill { get; set; }

        private string _cardCreationOwnerName= "";
        public string cardCreationOwnerName
        {
            get { return _cardCreationOwnerName; }
            set { SetProperty(ref _cardCreationOwnerName, value); }
        }

        private string _cardCreationCardNumber = "";
        public string cardCreationCardNumber
        {
            get { return _cardCreationCardNumber; }
            set { SetProperty(ref _cardCreationCardNumber, value); }
        }

        private string _cardCreationExpDate = "";
        public string cardCreationExpDate
        {
            get { return _cardCreationExpDate; }
            set { SetProperty(ref _cardCreationExpDate, value); }
        }

        private string _cardCreationCrypto = "";
        public string cardCreationCrypto
        {
            get { return _cardCreationCrypto; }
            set { SetProperty(ref _cardCreationCrypto, value); }
        }

        //Visibilities
        private Visibility _stackPanelAddCardButtonVisibility = Visibility.Visible;
        public Visibility stackPanelAddCardButtonVisibility
        {
            get { return _stackPanelAddCardButtonVisibility; }
            set { SetProperty(ref _stackPanelAddCardButtonVisibility, value); }
        }

        private Visibility _stackPanelCardCreationVisibility = Visibility.Collapsed;
        public Visibility stackPanelCardCreationVisibility
        {
            get { return _stackPanelCardCreationVisibility; }
            set { SetProperty(ref _stackPanelCardCreationVisibility, value); }
        }

        private Visibility _stackPanelCardsDisplayVisibility = Visibility.Collapsed;
        public Visibility stackPanelCardsDisplayVisibility
        {
            get { return _stackPanelCardsDisplayVisibility; }
            set { SetProperty(ref _stackPanelCardsDisplayVisibility, value); }
        }

        public enum mode
        {
            display, creation, helper,
        }

        private mode _currentMode;
        public mode currentMode
        {
            get { return _currentMode; }
            set { _currentMode = value; onModeChanged(value); }
        }

        private void onModeChanged(mode value)
        {
            switch (value)
            {
                case mode.display:
                default:
                    stackPanelCardsDisplayVisibility = Visibility.Visible;
                    stackPanelCardCreationVisibility = Visibility.Collapsed;
                    stackPanelAddCardButtonVisibility = Visibility.Visible;
                    break;
                case mode.creation:
                    stackPanelCardsDisplayVisibility = Visibility.Collapsed;
                    stackPanelCardCreationVisibility = Visibility.Visible;
                    stackPanelAddCardButtonVisibility = Visibility.Collapsed;
                    break;
            }
        }

        public ViewCardsViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            initSubscriptions();

            onBtnClickAddCard = new DelegateCommand<string>(executeonBtnClickAddCard, canExecuteonBtnClickAddCard);
            onBtnClickValidateCardCreation = new DelegateCommand<string>(executeonBtnClickValidateCardCreation, canExecuteonBtnClickValidateCardCreation).
                ObservesProperty(() => cardCreationOwnerName).
                ObservesProperty(() => cardCreationCardNumber).
                ObservesProperty(() => cardCreationExpDate).
                ObservesProperty(() => cardCreationCrypto);
            onBtnClickCancelCardCreation = new DelegateCommand<string>(executeonBtnClickCancelCardCreation, canExecuteonBtnClickCancelCardCreation);
            onBtnClickCardNumberAutoFill = new DelegateCommand<string>(executeonBtnClickCardNumberAutoFill, canExecuteonBtnClickCardNumberAutoFill);

            //refresh cards
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestEventArgs(Events.SdkCommandRequestEventArgs.CommandType.getCards));
        }

        private bool canExecuteonBtnClickCardNumberAutoFill(string arg)
        {
            return true;
        }

        private void executeonBtnClickCardNumberAutoFill(string obj)
        {
            cardCreationCardNumber = "4556848510974654";
        }

        private bool canExecuteonBtnClickCancelCardCreation(string arg)
        {
            return true;
        }

        private void executeonBtnClickCancelCardCreation(string obj)
        {
            currentMode = mode.display;
        }

        private bool canExecuteonBtnClickValidateCardCreation(string arg)
        {
            if (cardCreationOwnerName.Count() > 0 && cardCreationCardNumber.Count() == 16 && cardCreationExpDate.Count() == 5 && cardCreationCrypto.Count() == 3)
            {
                return true;
            }
            else
                return false;
        }

        private void executeonBtnClickValidateCardCreation(string obj)
        {
            dejamobile_takehome_sdk.Models.CardModel card = new dejamobile_takehome_sdk.Models.CardModel(cardCreationOwnerName, cardCreationCardNumber, cardCreationExpDate, cardCreationCrypto);
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestEventArgs(Events.SdkCommandRequestEventArgs.CommandType.addCard, card));
        }

        private bool canExecuteonBtnClickAddCard(string arg)
        {
            return true;
        }

        private void executeonBtnClickAddCard(string obj)
        {
            currentMode = mode.creation;
        }

        private void initSubscriptions()
        {
            eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Subscribe(onSdkCommandResultEvents);
        }

        private void onSdkCommandResultEvents(TaskResult obj)
        {
            switch (obj.name)
            {
                case TaskResult.TaskName.getCards:
                    if (obj.result)
                    {
                        eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationEventArgs(Events.NotificationEventArgs.notificationTypeEnum.success, "Cards refreshed !"));
                        cardList = (List<dejamobile_takehome_sdk.Models.CardModel>)obj.payload;
                    }
                    else
                    {
                        eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationEventArgs(Events.NotificationEventArgs.notificationTypeEnum.error, "Cards refresh failed :("));
                        eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationEventArgs(Events.NotificationEventArgs.notificationTypeEnum.warning, "Please ensure to be logged in !"));
                    }                    
                    break;
                case TaskResult.TaskName.addCard:
                    if (obj.result)
                    {
                        eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationEventArgs(Events.NotificationEventArgs.notificationTypeEnum.success, "Cards succesfully added !"));
                        currentMode = mode.display;

                        //refresh card list !
                        eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestEventArgs(Events.SdkCommandRequestEventArgs.CommandType.getCards));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
