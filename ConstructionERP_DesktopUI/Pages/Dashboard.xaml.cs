using ConstructionERP_DesktopUI.Models;
using System.ComponentModel;
using System.Windows.Controls;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, INotifyPropertyChanged
    {
        #region Initialization

        //MainLayout parentLayout = null;

        public Dashboard(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            parentLayout = mainLayout;

            SetValues();
        }


        void SetValues()
        {
            
        }

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MainLayout parentLayout;

        public MainLayout ParentLayout
        {
            get { return parentLayout; }
            set
            {
                parentLayout = value;
                OnPropertyChanged("ParentLayout");
            }
        }



        #endregion

    }
}
