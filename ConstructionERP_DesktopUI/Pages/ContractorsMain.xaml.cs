using ConstructionERP_DesktopUI.Helpers;
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
    /// Interaction logic for ContractorsMain.xaml
    /// </summary>
    public partial class ContractorsMain : UserControl
    {

        #region Initialization

        MainLayout mainLayout = null;

        public ContractorsMain(MainLayout mainLayout)
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
