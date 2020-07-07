using ConstructionERP_DesktopUI.Models;
using System.Windows.Controls;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        #region Initialization

        MainLayout parentLayout = null;
        LoggedInUser loggedInUser = null;

        public Dashboard(MainLayout mainLayout)
        {
            InitializeComponent();
            parentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            loggedInUser = parentLayout.loggedInUser;
            LblUser.Text = loggedInUser.Name;
        }

        #endregion
    }
}
