using ConstructionERP_DesktopUI.Helpers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            DataContext = this;
            this.mainLayout = mainLayout;
            NavigationCommand = new RelayCommand(mainLayout.SetActiveControl);
        }

        #endregion

        #region Navigation Command

        public ICommand NavigationCommand { get; private set; }


        #endregion
    }
}
