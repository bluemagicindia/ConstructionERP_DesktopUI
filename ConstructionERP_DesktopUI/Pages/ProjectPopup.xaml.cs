using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ProjectPopup : Window, INotifyPropertyChanged
    {

        #region Initialization

        private ProjectAPIHelper apiHelper;
        private ProjectStatusAPIHelper statusAPIHelper;
        private ProjectTypeAPIHelper typeAPIHelper;

        public ProjectPopup(ProjectSelection projectSelection)
        {
            InitializeComponent();
            DataContext = this;
            ProjectSelection = projectSelection;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new ProjectAPIHelper();
            statusAPIHelper = new ProjectStatusAPIHelper();
            typeAPIHelper = new ProjectTypeAPIHelper();

            new Action(async () => await GetTypes())();
            new Action(async () => await GetStatuses())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateProject()); }, () => CanSaveProject);
            ClosePopupCommand = new RelayCommand(ClosePopup);
        }

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ProjectSelection projectSelection;

        public ProjectSelection ProjectSelection
        {
            get { return projectSelection; }
            set
            {
                projectSelection = value;
                OnPropertyChanged("ProjectSelection");
            }
        }


        //Title
        private string title;

        public string Title_
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title_");
            }
        }


        //Statuses
        private ObservableCollection<StatusModel> statuses;

        public ObservableCollection<StatusModel> Statuses
        {
            get { return statuses; }
            set
            {
                statuses = value;
                OnPropertyChanged("Statuses");
            }
        }

        private StatusModel selectedStatus;

        public StatusModel SelectedStatus
        {
            get { return selectedStatus; }
            set
            {
                selectedStatus = value;
                OnPropertyChanged("SelectedStatus");
            }
        }

        private string statusText;

        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                OnPropertyChanged("StatusText");
            }
        }


        //Teams
        private ObservableCollection<TypeModel> types;

        public ObservableCollection<TypeModel> Types
        {
            get { return types; }
            set
            {
                types = value;
                OnPropertyChanged("Types");
            }
        }

        private TypeModel selectedType;

        public TypeModel SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                OnPropertyChanged("SelectedType");
            }
        }

        private string typeText;

        public string TypeText
        {
            get { return typeText; }
            set
            {
                typeText = value;
                OnPropertyChanged("TypeText");
            }
        }


        //Enquiry Date
        private DateTime startDate = DateTime.Today;
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        //FollowUp Date
        private DateTime dueDate = DateTime.Today;
        public DateTime DueDate
        {
            get { return dueDate; }
            set
            {
                dueDate = value;
                OnPropertyChanged("DueDate");
            }
        }

        private string address;

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }


        #endregion

        #region Get Project Types

        private async Task GetTypes()
        {
            try
            {
                Types = await typeAPIHelper.GetTypes(ProjectSelection.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Project Statuses

        private async Task GetStatuses()
        {
            try
            {
                Statuses = await statusAPIHelper.GetStatuses(ProjectSelection.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create Project Command

        public ICommand SaveCommand { get; private set; }

        private bool canSaveProject = true;

        public bool CanSaveProject
        {
            get { return canSaveProject; }
            set
            {
                canSaveProject = value;
                OnPropertyChanged("CreateProject");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveProject;

        public string SaveBtnText => canSaveProject ? "Save Project" : "Saving...";

        public string SaveBtnIcon => canSaveProject ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateProject()
        {
            try
            {
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title_),
                    new KeyValuePair<string, string>("Project Type", TypeText),
                    new KeyValuePair<string, string>("Project Status", StatusText),
                    new KeyValuePair<string, string>("Address", Address)

                };
                if (FieldValidation.ValidateFields(values))
                {

                    CanSaveProject = false;

                    ProjectModel projectData = new ProjectModel()
                    {
                        Title = Title_,
                        ProjectTypeID = SelectedType?.ID,
                        Type = SelectedType == null ? new TypeModel { Title = TypeText, CreatedBy = ProjectSelection.LoggedInUser.Name } : null,
                        ProjectStatusID = SelectedStatus?.ID,
                        Status = SelectedStatus == null ? new StatusModel { Title = StatusText, CreatedBy = ProjectSelection.LoggedInUser.Name } : null,
                        StartDate = StartDate,
                        DueDate = DueDate,
                        Address = Address,
                        CreatedBy = ProjectSelection.LoggedInUser.Name,
                        CreatedOn = DateTime.Now
                    };

                    HttpResponseMessage result = await apiHelper.PostProject(ProjectSelection.LoggedInUser.Token, projectData).ConfigureAwait(false);

                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Project Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {

                            new Action(async () => await ProjectSelection.GetProjects(null))();
                            ClosePopupCommand.Execute(null);
                        });


                    }
                    else
                    {
                        MessageBox.Show("Error in saving Project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveProject = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CanSaveProject = true;
            }

        }

        private void ClearFields()
        {
            try
            {
                Title_ = string.Empty;
                Address = string.Empty;
                StartDate = DateTime.Today;
                DueDate = DateTime.Today;
                SelectedStatus = null;
                SelectedType = null;
                StatusText = string.Empty;
                TypeText = string.Empty;
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Close Popup Command

        public ICommand ClosePopupCommand { get; private set; }

        private void ClosePopup(object param)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }


        #endregion
    }
}
