using ConstructionERP_DesktopUI.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for MainLayout.xaml
    /// </summary>
    public partial class MainLayout
    {
        #region Initialization

        public LoggedInUser loggedInUser = null;

        public MainLayout()
        {
            InitializeComponent();
            SetValues();
        }

        void SetValues()
        {
            loggedInUser = Application.Current.Properties["LoggedInUser"] as LoggedInUser;
            if (loggedInUser != null)
            {
                LblUser.Text = loggedInUser.Name;

            }
            SetActiveControl("Dashboard");
        }

        #endregion

        #region ActiveControl

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveControl(((Button)sender).CommandParameter);
        }

        void SetActiveControl(object item)
        {
            object view = null;
            switch (item.ToString())
            {
                case "Dashboard":
                    view = new Dashboard(this);
                    break;
                //case "ProjectView":
                //    view = new ProjectView();
                //    break;
                //case "SalesEnquiryView":
                //    view = new SalesEnquiryView();
                //    break;
                //case "UnitView":
                //    view = new UnitView();
                //    break;
                //case "ContractorView":
                //    view = new ContractorView();
                //    break;
                //case "SupplierView":
                //    view = new SupplierView();
                //    break;
                //case "SiteManagerView":
                //    view = new SiteManagerView();
                //    break;
                //case "StatusView":
                //    view = new StatusView();
                //    break;
                //case "TaskTypeView":
                //    view = new TaskTypeView();
                //    break;
                //case "StampView":
                //    view = new StampView();
                //    break;
                //case "MaterialView":
                //    view = new MaterialView();
                //    break;
                //case "TeamView":
                //    view = new TeamView();
                //    break;
                //case "QuotationView":
                //    view = new QuotationView();
                //    break;
                default:
                    view = new Dashboard(this);
                    break;
            }
            Container.Content = view;
        }

        #endregion

    }
}
