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

        private List<TextBlock> navBarTextBlocksList { get; set; }
        private List<Expander> navBarExpanderList { get; set; }

        private bool navBarIsOpened = true;

        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            getUiControlsFromNavBar();
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

        private void getUiControlsFromNavBar()
        {
            navBarExpanderList = new List<Expander>();
            navBarTextBlocksList = new List<TextBlock>();

            foreach (ListViewItem item in ListViewNavBar.Items)
            {
                if (item.Content.GetType() == typeof(StackPanel))
                {
                    foreach (object subItem in ((StackPanel)item.Content).Children)
                    {
                        if (subItem.GetType() == typeof(TextBlock))
                        {
                            //add to textblock lists
                            navBarTextBlocksList.Add((TextBlock)subItem);
                            printFormattedConsoleWriteLine("Textblock has been added to navBarTextBlocksList : " + ((TextBlock)subItem).Text);
                        }
                        else if (subItem.GetType() == typeof(Expander))
                        {
                            navBarExpanderList.Add((Expander)subItem);
                            printFormattedConsoleWriteLine("Expander has been added to navBarTreeViewList : " + ((Expander)subItem).Name);
                        }
                    }
                }
            }
        }

        private void printFormattedConsoleWriteLine(string s)
        {
            Console.WriteLine(DateTime.Now.ToString() + "UI/MainWindow.xaml -> " + s);
        }

        private void buttonCloseNavBar_Click(object sender, RoutedEventArgs e)
        {
            onNavBarClosed();
        }

        private void buttonOpenNavBar_Click(object sender, RoutedEventArgs e)
        {
            onNavBarOpened();
        }

        private void manageNavBarTextBlocksState(bool state)
        {
            Visibility visibility = Visibility.Visible;

            if (state)
                visibility = Visibility.Visible;
            else
                visibility = Visibility.Collapsed;

            foreach (TextBlock tb in navBarTextBlocksList)
            {
                tb.Visibility = visibility;
            }
        }

        private void manageNavBarExpanderState(bool state)
        {
            //only collapse when 
            foreach (Expander expd in navBarExpanderList)
            {
                if (state)
                    expd.Visibility = Visibility.Visible;
                else
                    expd.Visibility = Visibility.Collapsed;
            }
        }

        private void onNavBarClosed()
        {
            navBarIsOpened = false;
            buttonOpenNavBar.Visibility = Visibility.Visible;
            buttonCloseNavBar.Visibility = Visibility.Collapsed;
            imageBoxLogoOmwave.Visibility = Visibility.Collapsed;

            manageNavBarTextBlocksState(false);
            manageNavBarExpanderState(false);
        }

        private void onNavBarOpened()
        {
            navBarIsOpened = true;
            buttonOpenNavBar.Visibility = Visibility.Collapsed;
            buttonCloseNavBar.Visibility = Visibility.Visible;
            imageBoxLogoOmwave.Visibility = Visibility.Visible;

            manageNavBarTextBlocksState(true);
            manageNavBarExpanderState(true);
        }

        /*
        private void listViewItemTests_GotFocus(object sender, RoutedEventArgs e)
        {
            //eventAggregator.GetEvent<Events.NavigateToEvent>().Publish(Views.scenariiManagementView);
        }
        */

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void listViewItemUser_GotFocus(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<Events.NavigateToEvent>().Publish(ViewList.userView);
        }

        private void listViewItemCards_GotFocus(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<Events.NavigateToEvent>().Publish(ViewList.cardsView);
        }
    }
}
