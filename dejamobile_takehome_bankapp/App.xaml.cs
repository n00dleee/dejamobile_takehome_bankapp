using dejamobile_takehome_bankapp.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace dejamobile_takehome_bankapp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Prism.Unity.PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWindow>(ViewList.mainWindow);
            containerRegistry.RegisterForNavigation<ViewUser>(ViewList.userView);
            containerRegistry.RegisterForNavigation<ViewCards>(ViewList.cardsView);
        }
    }
}
