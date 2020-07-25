using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login: INotifyPropertyChanged
    {

        #region Initilization

        LoginAPIHelper loginAPIHelper;
        public Login()
        {
            InitializeComponent();
            DataContext = this;
            loginAPIHelper = new LoginAPIHelper();
            LoginCommand = new RelayCommand(async delegate { await Task.Run(() => DoLogin()); }, CLogin);
        }

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string userName = "hussain@gmail.com";

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }

      
        private string password = "Pass@123";

        public  string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        #endregion

        #region Login Command

        public ICommand LoginCommand { get; private set; }

        private bool canLogin = true;

        public bool CanLogin
        {
            get { return canLogin; }
            set
            {
                canLogin = value;
                OnPropertyChanged("CanLogin");
                OnPropertyChanged("IsSpinning");
                OnPropertyChanged("LoginBtnText");
                OnPropertyChanged("LoginBtnIcon");
            }
        }

        public bool CLogin() => canLogin;

        public bool IsSpinning => !canLogin;

        public string LoginBtnText => canLogin ? "Login" : "Wait...";

        public string LoginBtnIcon => canLogin ? "SignInAltSolid" : "SpinnerSolid";

        private async Task DoLogin()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("UserName", UserName),
                    new KeyValuePair<string, string>("Password", Password)
                };
            if (FieldValidation.ValidateFields(values))
            {
                CanLogin = false;
                try
                {
                    LoginAPIHelper loginAPIHelper = new LoginAPIHelper();
                    var result = await loginAPIHelper.Authenticate(UserName, Password).ConfigureAwait(false);
                    if (result.GetType() == typeof(LoginErrorResponse))
                    {
                        LoginErrorResponse response = result as LoginErrorResponse;
                        MessageBox.Show(response.Error_Description, response.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (result.GetType() == typeof(AuthenticatedUser))
                    {
                        AuthenticatedUser authenticatedUser = result as AuthenticatedUser;
                        LoggedInUser loggedInUser = await loginAPIHelper.GetLoggedInUser(authenticatedUser.Access_Token).ConfigureAwait(false);
                        loggedInUser.Token = authenticatedUser.Access_Token;
                        Application.Current.Properties["LoggedInUser"] = loggedInUser;
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            ProjectSelection projectSelection = new ProjectSelection();
                            projectSelection.Show();
                            this.Close();
                        });

                    }
                    else
                    {
                        MessageBox.Show("OOPS! Unexpected error occured", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanLogin = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanLogin = true;
                }
            }
        }

        #endregion

    }
}
