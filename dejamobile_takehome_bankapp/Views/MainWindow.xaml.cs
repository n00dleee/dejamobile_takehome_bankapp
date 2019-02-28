using dejamobile_takehome_bankapp.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace dejamobile_takehome_bankapp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private IEventAggregator _eventAggregator;
        public IEventAggregator eventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }


        private bool navBarIsOpened = true;

        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.eventAggregator = eventAggregator;

            testOnAppLaunch();
        }

        private void testOnAppLaunch()
        {

        }

        private void onServiceManagerEvents(ServiceManagementArg obj)
        {
            switch (obj.type)
            {
                case ServiceManagementArg.Type.status:

                    break;
                default:
                    break;
            }
        }


        private void printFormattedConsoleWriteLine(string s)
        {
            Console.WriteLine(DateTime.Now.ToString() + "UI/MainWindow.xaml -> " + s);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<Events.NavigateToEvent>().Publish(ViewList.userView);
        }

        private void listViewItemUser_GotFocus(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<Events.NavigateToEvent>().Publish(ViewList.userView);
        }

        private void listViewItemCards_GotFocus(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<Events.NavigateToEvent>().Publish(ViewList.cardsView);
        }

        private void listViewItemTest1_GotFocus(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<Events.NavigateToEvent>().Publish(ViewList.shoppingView);
        }
    }
}
