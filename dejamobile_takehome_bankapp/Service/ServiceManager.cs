using dejamobile_takehome_bankapp.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dejamobile_takehome_bankapp.Service
{
    public class ServiceManager
    {
        private IEventAggregator _eventAggregator;
        public IEventAggregator eventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }

        //add services here
        public enum ServicesEnum
        {
            LogManager,
            SdkManager,

        }

        //services
        private Logs.LogManager logManager { get; set; }
        private Sdk.SdkManager sdkManager { get; set; }

        public ServiceManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<Events.ServiceManagementEvent>().Subscribe(onServiceManagementRequest);
            initStartUpServices();
        }

        private void onServiceManagementRequest(ServiceManagementArg obj)
        {
            switch (obj.type)
            {
                case ServiceManagementArg.Type.start:
                    manageThisServiceState(obj, true);
                    break;
                case ServiceManagementArg.Type.stop:
                    manageThisServiceState(obj, false);
                    break;
                case ServiceManagementArg.Type.requestStatus:
                    checkThisServiceStatus(obj);
                    break;
                case ServiceManagementArg.Type.status:
                    break;
                default:
                    break;
            }
        }

        private void checkThisServiceStatus(ServiceManagementArg obj)
        {
            switch (obj.service)
            {
                
                case ServicesEnum.LogManager:
                    if (logManager != null)
                    {
                        eventAggregator.GetEvent<Events.ServiceManagementEvent>().Publish(new ServiceManagementArg(ServiceManagementArg.Type.status, ServicesEnum.LogManager, null, logManager.isStarted));
                    }
                    else
                    {
                        eventAggregator.GetEvent<Events.ServiceManagementEvent>().Publish(new ServiceManagementArg(ServiceManagementArg.Type.status, ServicesEnum.LogManager, null, false));
                    }
                    break;
                default:
                    break;
            }
        }

        private void manageThisServiceState(ServiceManagementArg obj, bool state)
        {
            if (state)
                startThisService(obj);
            else
                stopThisService(obj);
        }

        private void stopThisService(ServiceManagementArg obj)
        {
            switch (obj.service)
            {

                case ServicesEnum.LogManager:
                    logManager.stop();
                    break;
                default:
                    break;
            }
        }

        private void startThisService(ServiceManagementArg obj)
        {
            switch (obj.service)
            {
                case ServicesEnum.LogManager:
                    logManager = new Logs.LogManager(eventAggregator);
                    logManager.start();
                    eventAggregator.GetEvent<Events.ServiceManagementEvent>().Publish(new ServiceManagementArg(ServiceManagementArg.Type.status, ServicesEnum.LogManager, null, true));
                    break;
                case ServicesEnum.SdkManager:
                    sdkManager = new Sdk.SdkManager(eventAggregator);
                    sdkManager.start();
                    eventAggregator.GetEvent<Events.ServiceManagementEvent>().Publish(new ServiceManagementArg(ServiceManagementArg.Type.status, ServicesEnum.SdkManager, null, true));
                    break;

                default:
                    break;
            }
        }

        private void initStartUpServices()
        {
            //services to be started at boot
            startThisService(new ServiceManagementArg(ServiceManagementArg.Type.start, ServicesEnum.LogManager, null));
            startThisService(new ServiceManagementArg(ServiceManagementArg.Type.start, ServicesEnum.SdkManager, null));
        }
    }
}
