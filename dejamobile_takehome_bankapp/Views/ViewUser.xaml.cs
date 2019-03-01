using System.Windows.Controls;

namespace dejamobile_takehome_bankapp.Views
{
    /// <summary>
    /// Interaction logic for ViewUser
    /// </summary>
    public partial class ViewUser : UserControl
    {
        public ViewUser()
        {
            InitializeComponent();
        }

        private void passwordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            trickyHiddenTextbox.Text = ((PasswordBox)sender).Password;
        }
    }
}
