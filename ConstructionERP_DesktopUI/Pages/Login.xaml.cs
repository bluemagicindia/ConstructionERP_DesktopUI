using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using MahApps.Metro.IconPacks;
using ConstructionERP_DesktopUI.Helpers;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login
    {
        #region Initilization

        LoginAPIHelper loginAPIHelper;
        public Login()
        {
            InitializeComponent();
            loginAPIHelper = new LoginAPIHelper();
        }

        #endregion

        #region Login

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            #region Field Buildup

            List<KeyValuePair<string, string>> fields = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("UserName", TxtUserName.Text),
                    new KeyValuePair<string, string>("Password", TxtPassword.Password)
                };

            #endregion

            if (FieldValidation.ValidateFields(fields))
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        BtnIcon.Kind = PackIconFontAwesomeKind.SpinnerSolid;
                        BtnIcon.Spin = true;
                        BtnLogin.IsEnabled = false;
                    });

                    var result = await loginAPIHelper.Authenticate(TxtUserName.Text, TxtPassword.Password).ConfigureAwait(false);
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
                        //Application.Current.Dispatcher.Invoke((Action)delegate
                        //{
                        //    AdminLayoutView adminLayoutView = new AdminLayoutView();
                        //    adminLayoutView.Show();
                        //    Application.Current.Windows[0].Close();

                        //});

                    }
                    else
                    {
                        MessageBox.Show("OOPS! Unexpected error occured", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        BtnIcon.Kind = PackIconFontAwesomeKind.SignInAltSolid;
                        BtnIcon.Spin = false;
                        BtnLogin.IsEnabled = true;
                    });

                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        BtnIcon.Kind = PackIconFontAwesomeKind.SignInAltSolid;
                        BtnIcon.Spin = false;
                        BtnLogin.IsEnabled = true;

                    });
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

    }
}
