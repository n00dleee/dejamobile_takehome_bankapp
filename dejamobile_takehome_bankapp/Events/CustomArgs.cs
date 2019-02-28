using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dejamobile_takehome_bankapp.Events
{
    public class LogInfoArg : EventArgs
    {
        public enum Type
        {
            appInfo, error, debug,
            outputData,
            scenario,
            generateScenarioLog
        }
        public Type type { get; set; }
        public string dataTolog { get; set; }

        public LogInfoArg(Type type, string dataToLog = "")
        {
            this.type = type;
            this.dataTolog = dataToLog;
        }
    }

    public class ServiceManagementArg : EventArgs
    {
        public enum Type { start, stop, requestStatus, status }
        public Type type { get; set; }
        public Service.ServiceManager.ServicesEnum service { get; set; }
        public List<object> parametersList { get; set; }
        public bool? status { get; set; }

        public ServiceManagementArg(Type type, Service.ServiceManager.ServicesEnum service, List<object> parametersList, bool? status = null)
        {
            this.type = type;
            this.service = service;
            this.parametersList = parametersList;
            this.status = status;
        }
    }

    public class NotificationArgs
    {
        public notificationTypeEnum type { get; set; }
        public string info { get; set; }
        public enum notificationTypeEnum
        {
            information,
            success,
            error,
            warning,
            unknown
        }

        public NotificationArgs(notificationTypeEnum type, string info)
        {
            this.type = type;
            this.info = info;
        }
    }

    public class SdkCommandRequestArgs
    {
        public enum CommandType { createUser, login, addCard, getCards, deleteCard, getStats }

        public CommandType commandType { get; set; }
        public Object payload { get; set; }
        
        public SdkCommandRequestArgs(CommandType commandType, Object payload = null)
        {
            this.commandType = commandType;
            this.payload = payload;
        }
    }

    public class MerchantOrderArgs
    {
        public OrderStatus orderStatus;
        public dejamobile_takehome_sdk.Models.CardModel paymentInformation;
        public int itemPurchasedId;

        public enum OrderStatus { pendingForApproval, approved, refused }

        public MerchantOrderArgs(dejamobile_takehome_sdk.Models.CardModel card, int itemPurchasedId, OrderStatus orderStatus = OrderStatus.pendingForApproval)
        {
            this.itemPurchasedId = itemPurchasedId;
            this.paymentInformation = card;
            this.orderStatus = orderStatus;
        }
    }

    public class BankTransactionArgs
    {
        public string orderName;
        public OrderStatus orderStatus;
        public dejamobile_takehome_sdk.Models.CardModel paymentInformation;
        public int transactionAmount;

        public enum OrderStatus { pendingForApproval, approved, refused }

        public BankTransactionArgs(int transactionAmount, dejamobile_takehome_sdk.Models.CardModel card, string orderName)
        {
            this.transactionAmount = transactionAmount;
            this.orderName = orderName;
            this.paymentInformation = card;
        }
    }

    public class BankManagementArgs
    {
        public enum EventType { addDigitizedCard, removeDigitizedCard }

        public EventType eventType;
        public dejamobile_takehome_sdk.Models.UserModel userConcerned;
        public dejamobile_takehome_sdk.Models.CardModel originalCard;
        public dejamobile_takehome_sdk.Models.CardModel digitizedCard;

        public BankManagementArgs(dejamobile_takehome_sdk.Models.UserModel userConcerned, dejamobile_takehome_sdk.Models.CardModel originalCard, dejamobile_takehome_sdk.Models.CardModel digitizedCard)
        {
            this.userConcerned = userConcerned;
            this.originalCard = originalCard;
            this.digitizedCard = digitizedCard;
        }

    }
}