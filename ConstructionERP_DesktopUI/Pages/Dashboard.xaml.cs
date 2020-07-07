using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
