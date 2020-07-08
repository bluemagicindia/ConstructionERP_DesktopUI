using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for MainLayout.xaml
    /// </summary>
    public partial class MainLayout : INotifyPropertyChanged
    {
        #region Initialization

        public MainLayout()
        {
            InitializeComponent();
            DataContext = this;
            SetValues();

        }

        void SetValues()
        {
            NavigationCommand = new RelayCommand(SetActiveControl);
            LoggedInUser = Application.Current.Properties["LoggedInUser"] as LoggedInUser;
            NavigationCommand.Execute("Dashboard");
        }

        #endregion

        #region ActiveControl

        public ICommand NavigationCommand { get; private set; }

        public void SetActiveControl(object item)
        {
            object view = null;
            switch (item.ToString())
            {
                case "Dashboard":
                    view = new Dashboard(this);
                    break;
                case "Unit":
                    view = new Unit(this);
                    break;
                case "MaterialMain":
                    view = new MaterialMain(this);
                    break;
                case "Material":
                    view = new Material(this);
                    break;
                case "Settings":
                    view = new Settings(this);
                    break;
                default:
                    view = new Dashboard(this);
                    break;
            }
            Container.Content = view;
        }

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private LoggedInUser loggedInUser;

        public LoggedInUser LoggedInUser
        {
            get { return loggedInUser; }
            set
            {
                loggedInUser = value;
                OnPropertyChanged("LoggedInUser");
            }
        }

        #endregion
    }
}
