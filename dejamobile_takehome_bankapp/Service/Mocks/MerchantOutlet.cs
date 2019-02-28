using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dejamobile_takehome_bankapp.Events;

namespace dejamobile_takehome_bankapp.Service.Mocks
{
    class MerchantOutlet : ServiceTemplate
    {
        public MerchantOutlet(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            itemsInShop = initItemsInShop();
        }

        public class itemInShop
        {
            public int id { get; set; }
            public string name { get; set; }
            public int price { get; set; }

            public itemInShop(int id, string name, int price)
            {
                this.id = id;
                this.name = name;
                this.price = price;
            }
        }

        public List<itemInShop> itemsInShop;

        public List<itemInShop> initItemsInShop()
        {
            return new List<itemInShop>()
            {
                { new itemInShop(1, "groceries", 50) },
                { new itemInShop(2, "gpu", 1999) },
                { new itemInShop(3, "smartphone", 2399) },
                { new itemInShop(4, "coffee", 9) }
            };
        }

        public override void subscribe()
        {
            eventAggregator.GetEvent<Events.MerchantOrderEvent>().Subscribe(onMerchantOrderEvents);
            eventAggregator.GetEvent<Events.BankTransactionEvent>().Subscribe(onBankTransactionEvents);
        }

        private void onBankTransactionEvents(BankTransactionArgs obj)
        {
            switch (obj.orderStatus)
            {
                case BankTransactionArgs.OrderStatus.pendingForApproval:
                    break;
                case BankTransactionArgs.OrderStatus.approved:
                    //send info to user
                    eventAggregator.GetEvent<Events.MerchantOrderEvent>().Publish(new MerchantOrderArgs(null, -1, MerchantOrderArgs.OrderStatus.approved));
                    break;
                case BankTransactionArgs.OrderStatus.refused:
                    //send info to user
                    eventAggregator.GetEvent<Events.MerchantOrderEvent>().Publish(new MerchantOrderArgs(null, -1, MerchantOrderArgs.OrderStatus.refused));
                    break;
                default:
                    break;
            }
        }

        private void onMerchantOrderEvents(MerchantOrderArgs obj)
        {
            switch (obj.orderStatus)
            {
                case MerchantOrderArgs.OrderStatus.pendingForApproval:
                    processOrder(obj);
                    break;
                case MerchantOrderArgs.OrderStatus.approved:
                    obj.orderStatus = MerchantOrderArgs.OrderStatus.approved;
                    eventAggregator.GetEvent<Events.MerchantOrderEvent>().Publish(obj);
                    break;
                case MerchantOrderArgs.OrderStatus.refused:
                    obj.orderStatus = MerchantOrderArgs.OrderStatus.approved;
                    eventAggregator.GetEvent<Events.MerchantOrderEvent>().Publish(obj);
                    break;
                default:
                    break;
            }
        }

        private itemInShop getItemFromId(int id)
        {
            foreach(itemInShop item in itemsInShop)
            {
                if (item.id == id)
                    return item;
            }

            return null;
        }

        private void processOrder(MerchantOrderArgs obj)
        {
            itemInShop item = getItemFromId(obj.itemPurchasedId);
            if (item.price > 0)
            {
                sendPaymentRequestToBankService(item.price, obj.paymentInformation, item.name + " " + DateTime.Now.ToString());
            }
        }

        private void sendPaymentRequestToBankService(int amount, dejamobile_takehome_sdk.Models.CardModel card, string orderName)
        {
            eventAggregator.GetEvent<Events.BankTransactionEvent>().Publish(new BankTransactionArgs(amount, card, orderName));
        }

        public override void unsubscribe()
        {
            eventAggregator.GetEvent<Events.MerchantOrderEvent>().Unsubscribe(onMerchantOrderEvents);
        }
    }
}
