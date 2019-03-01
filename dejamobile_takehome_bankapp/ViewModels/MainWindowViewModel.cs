using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Reflection;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace dejamobile_takehome_bankapp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        private string _title = "Live Beacon tester ";
        public string title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string getVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private Service.ServiceManager serviceManager { get; set; }

        //notifications
        public readonly Notifier _notifier;

        System.Timers.Timer timerUpdateCheck { get; set; }

        private IEventAggregator _eventAggregator;
        public IEventAggregator eventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }


        public MainWindowViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;

            serviceManager = new Service.ServiceManager(eventAggregator);
            title += " " + getVersion();

            initEventSubscriptions();

            //notification toaster init
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 25,
                    offsetY: 25);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(2),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(6));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 250;
            });
        }

        private void handleNotificationEvent(object obj)
        {
            try
            {
                Events.NotificationArgs notification = (Events.NotificationArgs)obj;
                switch (notification.type)
                {
                    case Events.NotificationArgs.notificationTypeEnum.information:
                        _notifier.ShowInformation(notification.info);
                        break;
                    case Events.NotificationArgs.notificationTypeEnum.success:
                        _notifier.ShowSuccess(notification.info);
                        break;
                    case Events.NotificationArgs.notificationTypeEnum.warning:
                        _notifier.ShowWarning(notification.info);
                        break;
                    case Events.NotificationArgs.notificationTypeEnum.error:
                        _notifier.ShowError(notification.info);
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Notification ARG casting error ...");
            }
        }

        private void initEventSubscriptions()
        {
            eventAggregator.GetEvent<Events.NavigateToEvent>().Subscribe(handleNavigationEvent);
            eventAggregator.GetEvent<Events.NotificationEvent>().Subscribe(handleNotificationEvent);
        }

        private void handleNavigationEvent(string target)
        {
            regionManager.RequestNavigate("ContentRegion", target);
        }
    }
}
