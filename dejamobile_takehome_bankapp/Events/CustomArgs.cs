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

    public class NotificationEventArgs
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

        public NotificationEventArgs(notificationTypeEnum type, string info)
        {
            this.type = type;
            this.info = info;
        }
    }


}
