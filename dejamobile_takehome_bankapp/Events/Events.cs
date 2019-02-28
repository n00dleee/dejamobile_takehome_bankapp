using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dejamobile_takehome_bankapp.Events
{
    public class NavigateToEvent : PubSubEvent<string> { }
    public class LogInfoEvent : PubSubEvent<LogInfoArg> { }
    public class ServiceManagementEvent : PubSubEvent<ServiceManagementArg> { }
    public class NotificationEvent : PubSubEvent<NotificationArgs> { }
    public class SdkCommandRequestEvent : PubSubEvent<SdkCommandRequestArgs> { }
    public class SdkCommandResultEvent : PubSubEvent<dejamobile_takehome_sdk.TaskResult> { }
    public class MerchantOrderEvent : PubSubEvent<MerchantOrderArgs> { }
    public class BankTransactionEvent : PubSubEvent<BankTransactionArgs> { }
}
