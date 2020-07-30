using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, INotifyPropertyChanged
    {
        #region Initialization

        MainLayout ParentLayout = null;

        public Dashboard(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();

        }

        void SetValues()
        {
            SetProgress();
            ParentLayout.PropertyChanged += ParentLayout_PropertyChanged;
            NavigationCommand = new RelayCommand(ParentLayout.SetActiveControl);
        }

        private void ParentLayout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetProgress();
        }

        #endregion

        #region Navigation Command

        public ICommand NavigationCommand { get; private set; }

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private int progress;
        public int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
            }
        }

        private int daysLeft;

        public int DaysLeft
        {
            get { return daysLeft; }
            set
            {
                daysLeft = value;
                OnPropertyChanged("DaysLeft");
            }
        }

        private DateTime date;

        public DateTime DueDate
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("DueDate");
            }
        }



        #endregion

        #region SetProgress()

        private void SetProgress()
        {
            try
            {
                int TotalDays = (int)(ParentLayout.SelectedProject.DueDate - ParentLayout.SelectedProject.StartDate).TotalDays;
                int DaysConsumed = (int)(DateTime.Today - ParentLayout.SelectedProject.StartDate).TotalDays;
                DaysLeft = DaysConsumed > TotalDays ? 0 : TotalDays - DaysConsumed;
                Progress = DaysConsumed >= TotalDays ? 100 : (DaysConsumed * 100) / TotalDays;
                DueDate = ParentLayout.SelectedProject.DueDate;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        #endregion

        #region Unload

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ParentLayout.PropertyChanged -= ParentLayout_PropertyChanged;
        }

        #endregion
    }
}
