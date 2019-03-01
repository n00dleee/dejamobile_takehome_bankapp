using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using dejamobile_takehome_sdk;
using System.Windows;
using dejamobile_takehome_sdk.Models;

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

        dejamobile_takehome_sdk.Models.UserModel loggedUser;

        List<dejamobile_takehome_sdk.Models.CardModel> cardList { get; set; }

        dejamobile_takehome_sdk.Models.CardModel currentOriginalCard { get; set; }

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
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestArgs(Events.SdkCommandRequestArgs.CommandType.getCards));

            //get logged user info
            eventAggregator.GetEvent<Events.GetUserLoggedEvent>().Publish();
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
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestArgs(Events.SdkCommandRequestArgs.CommandType.addCard, card));
            currentOriginalCard = card;
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
            eventAggregator.GetEvent<Events.NavigateToEvent>().Subscribe(onNavigateEvents);
            eventAggregator.GetEvent<Events.UserLoggedInEvent>().Subscribe(onUserLoggedInEvents);
        }

        private void onUserLoggedInEvents(UserModel obj)
        {
            loggedUser = obj;
        }

        private void onNavigateEvents(string obj)
        {
            switch (obj)
            {
                case "ViewCards": // cannot switch on Views.ViewList.cardsView as it is not considered constant. Cannot set this as const AND static at the same time :(
                    eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestArgs(Events.SdkCommandRequestArgs.CommandType.getCards));
                    break;
                default:
                    break;
            }
        }

        private void onSdkCommandResultEvents(TaskResult obj)
        {
            switch (obj.name)
            {
                case TaskResult.TaskName.getCards:
                    if (obj.result)
                    {
                        eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.success, "Cards refreshed !"));
                        cardList = (List<CardModel>)obj.payload;
                    }
                    else
                    {
                        eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.error, "Cards refresh failed :("));
                        eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.warning, "Please ensure to be logged in !"));
                    }                    
                    break;
                case TaskResult.TaskName.addCard:
                    if (obj.result)
                    {
                        eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.success, "Cards succesfully added !"));
                        currentMode = mode.display;

                        //notify bank a digitized card has been added
                        eventAggregator.GetEvent<Events.BankManagementEvent>().Publish(new Events.BankManagementArgs(Events.BankManagementArgs.EventType.addDigitizedCard, loggedUser, (CardModel)obj.payload, currentOriginalCard));

                        //refresh card list !
                        eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestArgs(Events.SdkCommandRequestArgs.CommandType.getCards));
                    }
                    else
                    {
                        eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.error, "Digitized card generation failed :("));

                        currentMode = mode.display;
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
