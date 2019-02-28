using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using dejamobile_takehome_sdk.Models;
using dejamobile_takehome_sdk;
using dejamobile_takehome_bankapp.Service.Mocks;

namespace dejamobile_takehome_bankapp.ViewModels
{
    public class ViewHistoryViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        public IEventAggregator eventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }

        private string _currentAmountOfCashRemaining;
        public string currentAmountOfCashRemaining
        {
            get { return _currentAmountOfCashRemaining; }
            set { SetProperty(ref _currentAmountOfCashRemaining, value) ; }
        }

        private List<Transaction> _listOfTransaction;
        public List<Transaction> listOfTransaction
        {
            get { return _listOfTransaction; }
            set { SetProperty(ref _listOfTransaction, value); }
        }

        private List<string> _listOfDisplayableTransaction;
        public List<string> listOfDisplayableTransaction
        {
            get { return _listOfDisplayableTransaction; }
            set { SetProperty(ref _listOfDisplayableTransaction, value); }
        }



        private List<string> _listOfAvaibleDisplayableCardNumber;
        public List<string> listOfAvaibleDisplayableCardNumber
        {
            get { return _listOfAvaibleDisplayableCardNumber; }
            set { SetProperty(ref _listOfAvaibleDisplayableCardNumber, value); }
        }

        private string _selectedDisplayableCardNumber = "";
        public string selectedDisplayableCardNumber
        {
            get { return _selectedDisplayableCardNumber; }
            set { SetProperty(ref _selectedDisplayableCardNumber, value); }
        }

        public DelegateCommand<string> onBtnClickGetReceipts { get; set; }

        private Dictionary<string, CardModel> availableCardDictionnary;
        private List<CardModel> availableCards;

        public ViewHistoryViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            onBtnClickGetReceipts = new DelegateCommand<string>(executeonBtnClickGetReceipts, canExecuteonBtnClickGetReceipts).ObservesProperty(() => selectedDisplayableCardNumber);

            availableCardDictionnary = new Dictionary<string, dejamobile_takehome_sdk.Models.CardModel>();
            listOfAvaibleDisplayableCardNumber = new List<string>();
            initSubscriptions();
            eventAggregator.GetEvent<Events.SdkCommandRequestEvent>().Publish(new Events.SdkCommandRequestArgs(Events.SdkCommandRequestArgs.CommandType.getCards));
        }

        private void initSubscriptions()
        {
            eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Subscribe(onSdkCommandResultEvents);
            eventAggregator.GetEvent<Events.BankReceiptEvent>().Subscribe(onBankReceiptEvents);
        }

        private void onBankReceiptEvents(AccountDisplayableInfo obj)
        {
            currentAmountOfCashRemaining = obj.amounfOfCash.ToString() + "€";
            listOfTransaction = obj.transactionHistory;
            listOfDisplayableTransaction = buildDisplayableTransactionList(listOfTransaction);
        }

        private List<string> buildDisplayableTransactionList(List<Transaction> listOfTransaction)
        {
            List<string> list = new List<string>();

            foreach (Transaction transaction in listOfTransaction)
            {
                list.Add(transaction.name + " - " + transaction.date + " - " + transaction.amount.ToString() + "€ (balance = " + transaction.remainingBalance + ")");
            }

            return list;
        }

        //duplicated code :( I should have made a cardManagement service where every card info is stored/process... if I had enough time
        private void onCardsRefresh()
        {
            listOfAvaibleDisplayableCardNumber.Clear();
            buildDictionnary(availableCards);

            foreach (KeyValuePair<string, dejamobile_takehome_sdk.Models.CardModel> kvp in availableCardDictionnary)
            {
                listOfAvaibleDisplayableCardNumber.Add(kvp.Key);
            }
        }

        private CardModel getSelectedCardFromLast6Digits(string s)
        {
            dejamobile_takehome_sdk.Models.CardModel card;

            availableCardDictionnary.TryGetValue(s, out card);

            if (card != null)
            {
                return card;
            }
            else
            {
                eventAggregator.GetEvent<Events.NotificationEvent>().Publish(new Events.NotificationArgs(Events.NotificationArgs.notificationTypeEnum.error, "Error while retrieving your payment method :("));
                return null;
            }
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

        private void onSdkCommandResultEvents(TaskResult obj)
        {
            switch (obj.name)
            {
                case TaskResult.TaskName.getCards:
                    if (obj.result)
                    {
                        availableCards = (List<dejamobile_takehome_sdk.Models.CardModel>)obj.payload;
                        onCardsRefresh();
                    }
                    break;
                default:
                    break;
            }
        }

        private bool canExecuteonBtnClickGetReceipts(string arg)
        {
            if (selectedDisplayableCardNumber.Count() == 9)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void executeonBtnClickGetReceipts(string obj)
        {
            eventAggregator.GetEvent<Events.BankManagementEvent>().Publish(new Events.BankManagementArgs(Events.BankManagementArgs.EventType.getHistory, null, getSelectedCardFromLast6Digits(selectedDisplayableCardNumber)));
        }
    }
}
