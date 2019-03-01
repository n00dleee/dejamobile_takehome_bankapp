using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dejamobile_takehome_bankapp.Events;
using dejamobile_takehome_sdk.Models;

namespace dejamobile_takehome_bankapp.Service.Mocks
{
    class Bank : ServiceTemplate
    {
        private int initialAmountOfCash = 5000;

        public Bank(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            accounts = new List<BankAccount>();
        }

        List<BankAccount> accounts;

        public override void subscribe()
        {
            eventAggregator.GetEvent<Events.BankTransactionEvent>().Subscribe(onBankTransactionEvents);
            eventAggregator.GetEvent<Events.BankManagementEvent>().Subscribe(onBankManagementEvents);
        }

        public BankAccount getAccountFromUser(dejamobile_takehome_sdk.Models.UserModel user)
        {
            foreach(BankAccount account in accounts)
            {
                if(account.user == user)
                {
                    return account;
                }
            }

            return null;
        }

        private void onBankManagementEvents(BankManagementArgs obj)
        {
            switch (obj.eventType)
            {
                case BankManagementArgs.EventType.addDigitizedCard:
                    // create new account
                    createNewAccount(obj.userConcerned, obj.originalCard, obj.digitizedCard, initialAmountOfCash);
                    break;
                case BankManagementArgs.EventType.removeDigitizedCard:
                    //remove account
                    deleteAccountLinkedToThisVirtualCard(obj.idToManage);
                    break;
                case BankManagementArgs.EventType.getHistory:
                    eventAggregator.GetEvent<Events.BankReceiptEvent>().Publish(getAccountInfoForThisCard(obj.digitizedCard));
                    break;
                default:
                    break;
            }
        }

        private AccountDisplayableInfo getAccountInfoForThisCard(CardModel digitizedCard)
        {
            foreach(BankAccount account in accounts)
            {
                if (account.paymentInformation.digitizedCardCopy == digitizedCard)
                {
                    return account.accountInfo;
                }
            }
            return null;
        }

        private bool deleteAccountLinkedToThisVirtualCard(string id)
        {
            BankAccount accountToRemove = null;

            foreach(BankAccount account in accounts)
            {
                if(account.paymentInformation.digitizedCardCopy.uid == id)
                {
                    accountToRemove = account;
                }
            }

            if (accountToRemove != null)
            {
                accounts.Remove(accountToRemove);
                return true;
            }
            else
                return false;
        }

        private void onBankTransactionEvents(BankTransactionArgs obj)
        {
            switch (obj.orderStatus)
            {
                case BankTransactionArgs.OrderStatus.pendingForApproval:
                    bool result = processPaymentRequest(obj.paymentInformation, obj.transactionAmount, obj.orderName);
                    if (result)
                    {
                        obj.orderStatus = BankTransactionArgs.OrderStatus.approved;
                    }
                    else
                    {
                        obj.orderStatus = BankTransactionArgs.OrderStatus.refused;
                    }
                    eventAggregator.GetEvent<Events.BankApprovalEvent>().Publish(obj);
                    break;
                default:
                    break;
            }
        }

        private bool processPaymentRequest(CardModel digitizedCard, int transactionAmount, string transactionName)
        {
            foreach(BankAccount account in accounts)
            {
                //check is user has the digitized card
                if(account.paymentInformation.digitizedCardCopy == digitizedCard)
                {
                    //check if account has enough funds for the transaction
                    if (account.accountInfo.amounfOfCash >= transactionAmount)
                    {
                        //remove money from account and store transaction
                        account.accountInfo.processTransaction(transactionAmount, transactionName);
                        return true;
                    }
                    else
                        return false;
                }
            }
            return false;
        }

        public override void unsubscribe()
        {
            eventAggregator.GetEvent<Events.BankTransactionEvent>().Unsubscribe(onBankTransactionEvents);
            eventAggregator.GetEvent<Events.BankManagementEvent>().Unsubscribe(onBankManagementEvents);
        }

        private BankAccount createNewAccount(UserModel user, CardModel originalCard, CardModel digitizedCard, int amoutOfCash)
        {
            BankAccount newAccount = new BankAccount(user, new PaymentInformation(originalCard, digitizedCard), amoutOfCash);
            accounts.Add(newAccount);

            return newAccount;
        }
    }

    public class BankAccount
    {
        public UserModel user { get; set; }
        public PaymentInformation paymentInformation { get; set; }
        public AccountDisplayableInfo accountInfo { get; set; }

        public BankAccount(UserModel user, PaymentInformation paymentInformation, int amountOfCash = 5000)
        {
            this.paymentInformation = paymentInformation;
            this.accountInfo = new AccountDisplayableInfo(amountOfCash);
            this.user = user;
        }
    }

    public class PaymentInformation
    {
        public CardModel originalCard { get; set; }
        public CardModel digitizedCardCopy { get; set; }

        public PaymentInformation(CardModel originalCard, CardModel digitizedCardCopy)
        {
            this.originalCard = originalCard;
            this.digitizedCardCopy = digitizedCardCopy;
        }

    }

    public class AccountDisplayableInfo
    {
        public int amounfOfCash { get; set; }
        public List<Transaction> transactionHistory { get; set; }

        public AccountDisplayableInfo(int amountOfCash)
        {
            this.amounfOfCash = amountOfCash;
            this.transactionHistory = new List<Transaction>();
        }

        public void processTransaction(int amount, string transactionName)
        {
            this.amounfOfCash -= amount;
            this.transactionHistory.Add(new Transaction(transactionName, DateTime.Now, amount, amounfOfCash));
        }
    }

    public class Transaction
    {
        public string name { get; set; }
        public DateTime date { get; set; }
        public int amount { get; set; }
        public int remainingBalance {get;set; }

        public Transaction(string name, DateTime date, int amount, int remainingBalance)
        {
            this.name = name;
            this.date = date;
            this.amount = amount;
            this.remainingBalance = remainingBalance;
        }
    }
}
