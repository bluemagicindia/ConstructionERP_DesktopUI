using System.Windows;
using System.Windows.Controls;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {

        #region Initialization

        MainLayout mainLayout = null;

        public Settings(MainLayout mainLayout)
        {
            InitializeComponent();
            this.mainLayout = mainLayout;
        }

        #endregion

        #region Navigation Buttons

        private void BtnUnit_Click(object sender, RoutedEventArgs e)
        {
            mainLayout.SetActiveControl("Unit");

        }

        #endregion
    }
}
