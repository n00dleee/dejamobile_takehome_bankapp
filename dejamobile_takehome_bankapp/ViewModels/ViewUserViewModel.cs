using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace dejamobile_takehome_bankapp.ViewModels
{
    public class ViewUserViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        public IEventAggregator eventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }

        //login
        private string _userName ="";
        public string userName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value) ; }
        }

        private string _password ="";
        public string password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        //user creation
        private string _userCreationUserName ="";
        public string _serCreationUserName
        {
            get { return _userCreationUserName; }
            set { SetProperty(ref _userCreationUserName, value); }
        }

        private string _userCreationPassword ="";
        public string userCreationPassword
        {
            get { return _userCreationPassword; }
            set { SetProperty(ref _userCreationPassword, value); }
        }

        private string _userCreationPhone = "";
        public string userCreationPhone
        {
            get { return _userCreationPhone; }
            set { SetProperty(ref _userCreationPhone, value); }
        }

        private Visibility _stackPanelLoginVisibility = Visibility.Visible;
        public Visibility stackPanelLoginVisibility
        {
            get { return _stackPanelLoginVisibility; }
            set { SetProperty(ref _stackPanelLoginVisibility, value); }
        }

        private Visibility _stackPanelUserCreationVisibility = Visibility.Collapsed;
        public Visibility stackPanelUserCreationVisibility
        {
            get { return _stackPanelUserCreationVisibility; }
            set { SetProperty(ref _stackPanelUserCreationVisibility, value); }
        }

        private Visibility _stackPanelUserHelperVisibility = Visibility.Collapsed;
        public Visibility stackPanelUserHelperVisibility
        {
            get { return _stackPanelUserHelperVisibility; }
            set { SetProperty(ref _stackPanelUserHelperVisibility, value); }
        }

        private mode _currentMode;
        public mode currentMode
        {
            get { return _currentMode; }
            set { _currentMode = value; onModeChanged(value); }
        }

        private void onModeChanged(mode value)
        {
            switch (value)
            {
                case mode.login:
                default:
                    stackPanelLoginVisibility = Visibility.Visible;
                    stackPanelUserCreationVisibility = Visibility.Collapsed;
                    stackPanelUserHelperVisibility = Visibility.Collapsed;
                    break;
                case mode.creation:
                    stackPanelLoginVisibility = Visibility.Collapsed;
                    stackPanelUserHelperVisibility = Visibility.Collapsed;
                    stackPanelUserCreationVisibility = Visibility.Visible;
                    break;
                case mode.helper:
                    stackPanelLoginVisibility = Visibility.Collapsed;
                    stackPanelUserHelperVisibility = Visibility.Visible;
                    stackPanelUserCreationVisibility = Visibility.Collapsed;
                    break;
            }
        }

        public enum mode { login, creation, helper }

        public DelegateCommand<string> onBtnClickLogin { get; set; }
        public DelegateCommand<string> onBtnClickGoToUserCreation { get; set; }
        public DelegateCommand<string> onBtnClickGoBackToLogin { get; set; }

        public ViewUserViewModel()
        {
            currentMode = mode.login;

            onBtnClickLogin = new DelegateCommand<string>(executeonBtnClickLogin, canExecuteonBtnClickLogin).ObservesProperty(() => userName).ObservesProperty(() => password);
            onBtnClickGoToUserCreation = new DelegateCommand<string>(executeonBtnClickGoToUserCreation, canExecuteonBtnClickGoToUserCreation);
            onBtnClickGoBackToLogin = new DelegateCommand<string>(executeonBtnClickGoBackToLogin, canExecuteonBtnClickGoBackToLogin);
        }

        private bool canExecuteonBtnClickLogin(string arg)
        {
            if (userName.Count() > 0 && password.Count() > 7) //password should be at least 8 chars
            {
                return true;
            }
            else
                return false;
        }

        private void executeonBtnClickLogin(string obj)
        {
            
        }

        private void executeonBtnClickGoToUserCreation(string obj)
        {
            throw new NotImplementedException();
        }

        private bool canExecuteonBtnClickGoToUserCreation(string arg)
        {
            return true;
        }

        private bool canExecuteonBtnClickGoBackToLogin(string arg)
        {
            return true;
        }

        private void executeonBtnClickGoBackToLogin(string obj)
        {
            throw new NotImplementedException();
        }
    }
}
