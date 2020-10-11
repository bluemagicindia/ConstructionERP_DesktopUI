using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for ProjectSelection.xaml
    /// </summary>
    public partial class ProjectSelection : INotifyPropertyChanged
    {

        #region Initialization

        private ProjectAPIHelper apiHelper;

        public ProjectSelection()
        {
            InitializeComponent();
            DataContext = this;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new ProjectAPIHelper();

            LoggedInUser = Application.Current.Properties["LoggedInUser"] as LoggedInUser;
            new Action(async () => await GetProjects(null))();

            ProjectPopupCommand = new RelayCommand(PopupWindow);
            SearchCommand = new RelayCommand(SearchProject);
            SelectionCommand = new RelayCommand(SelectProject);
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

        //Selected Project
        private ProjectModel selectedProject;

        public ProjectModel SelectedProject
        {
            get { return selectedProject; }
            set
            {
                selectedProject = value;
                OnPropertyChanged("SelectedProject");
                SelectionCommand.Execute(null);
            }
        }

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged("SearchText");
                SearchCommand.Execute(null);
            }
        }


        #endregion

        #region Get Projects

        private ObservableCollection<ProjectModel> projects;

        public ObservableCollection<ProjectModel> Projects
        {
            get { return projects; }
            set
            {
                projects = value;
                OnPropertyChanged("Projects");
            }
        }

        private bool isProgressing;

        public bool IsProgressing
        {
            get { return isProgressing; }
            set
            {
                isProgressing = value;
                OnPropertyChanged("IsProgressing");
            }
        }

        public async Task GetProjects(string searchText)
        {
            try
            {
                IsProgressing = true;
                Projects = string.IsNullOrWhiteSpace(searchText) ? await apiHelper.GetProjects(loggedInUser.Token) : await apiHelper.GetProjects(LoggedInUser.Token, searchText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                IsProgressing = false;
            }

        }

        #endregion

        #region Project Popup Command

        public ICommand ProjectPopupCommand { get; private set; }

        private void PopupWindow(object param)
        {
            try
            {
                ProjectPopup popup = new ProjectPopup(this);
                popup.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion

        #region Project Search Command

        public ICommand SearchCommand { get; private set; }

        private async void SearchProject(object param)
        {
            try
            {
                await GetProjects(SearchText);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion

        #region Project Selection Command

        public ICommand SelectionCommand { get; private set; }

        private void SelectProject(object param)
        {
            try
            {
                MainLayout mainLayout = new MainLayout(SelectedProject);
                mainLayout.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion


    }
}
