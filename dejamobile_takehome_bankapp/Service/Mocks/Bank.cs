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
                    deleteDigitizedCardFromAccount(obj.digitizedCard);

                    break;
                default:
                    break;
            }
        }

        private void deleteDigitizedCardFromAccount(CardModel digitizedCard)
        {
            foreach(BankAccount account in accounts)
            {
                if(account.paymentInformation.digitizedCardCopy == digitizedCard)
                {
                    account.paymentInformation.digitizedCardCopy = null;
                }
            }
        }

        private void onBankTransactionEvents(BankTransactionArgs obj)
        {
            switch (obj.orderStatus)
            {
                case BankTransactionArgs.OrderStatus.pendingForApproval:
                    bool result = processPaymentRequest(obj.paymentInformation, obj.transactionAmount);
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

        private bool processPaymentRequest(CardModel digitizedCard, int transactionAmount)
        {
            foreach(BankAccount account in accounts)
            {
                //check is user has the digitized card
                if(account.paymentInformation.digitizedCardCopy == digitizedCard)
                {
                    //check if account has enough funds for the transaction
                    if (account.amounfOfCash >= transactionAmount)
                    {
                        //remove money from account
                        account.amounfOfCash -= transactionAmount;
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
            throw new NotImplementedException();
        }

        private BankAccount createNewAccount(dejamobile_takehome_sdk.Models.UserModel user, dejamobile_takehome_sdk.Models.CardModel originalCard, dejamobile_takehome_sdk.Models.CardModel digitizedCard, int amoutOfCash)
        {
            BankAccount newAccount = new BankAccount(user, new PaymentInformation(originalCard, digitizedCard), amoutOfCash);
            accounts.Add(newAccount);

            return newAccount;
        }
    }

    public class BankAccount
    {
        public dejamobile_takehome_sdk.Models.UserModel user { get; set; }
        public PaymentInformation paymentInformation { get; set; }
        public int amounfOfCash { get; set; }

        public BankAccount(dejamobile_takehome_sdk.Models.UserModel user, PaymentInformation paymentInformation, int amountOfCash = 5000)
        {
            this.paymentInformation = paymentInformation;
            this.amounfOfCash = amountOfCash;
            this.user = user;
        }
    }

    public class PaymentInformation
    {
        public dejamobile_takehome_sdk.Models.CardModel originalCard { get; set; }
        public dejamobile_takehome_sdk.Models.CardModel digitizedCardCopy { get; set; }

        public PaymentInformation(dejamobile_takehome_sdk.Models.CardModel originalCard, dejamobile_takehome_sdk.Models.CardModel digitizedCardCopy)
        {
            this.originalCard = originalCard;
            this.digitizedCardCopy = digitizedCardCopy;
        }

    }
}
