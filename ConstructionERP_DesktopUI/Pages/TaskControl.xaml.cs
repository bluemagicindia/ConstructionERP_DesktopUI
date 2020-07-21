using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private TaskAPIHelper apiHelper;
        private TaskTypeAPIHelper typeAPIHelper;
        private TaskStatusAPIHelper statusAPIHelper;
        private StampAPIHelper stampAPIHelper;
        private TeamAPIHelper teamAPIHelper;
        private SheetAPIHelper sheetAPIHelper;

        public TaskControl(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }



        void SetValues()
        {
            apiHelper = new TaskAPIHelper();
            typeAPIHelper = new TaskTypeAPIHelper();
            statusAPIHelper = new TaskStatusAPIHelper();
            stampAPIHelper = new StampAPIHelper();
            teamAPIHelper = new TeamAPIHelper();
            sheetAPIHelper = new SheetAPIHelper();


            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetTasks())();
            new Action(async () => await GetTypes())();
            new Action(async () => await GetStatuses())();
            new Action(async () => await GetTeams())();
            new Action(async () => await GetStamps())();
            new Action(async () => await GetSheets())();

            //SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateProject()); }, () => CanSaveProject);
            //DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteProject()); }, () => CanDeleteProject);
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

        //Selected Project
        private TaskModel selectedTask;

        public TaskModel SelectedTask
        {
            get { return selectedTask; }
            set
            {
                selectedTask = value;
                OnPropertyChanged("SelectedTask");
            }
        }

        //ID
        private long id;

        public long ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }


        //Title
        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
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

        private StatusModel status;

        public StatusModel Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
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

        private TypeModel type;

        public TypeModel Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
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


        //Contractors
        private ObservableCollection<TeamModel> teams;

        public ObservableCollection<TeamModel> Teams
        {
            get { return teams; }
            set
            {
                teams = value;
                OnPropertyChanged("Teams");
            }
        }

        private TeamModel team;

        public TeamModel Team
        {
            get { return team; }
            set
            {
                team = value;
                OnPropertyChanged("Team");
            }
        }

        private string teamText;

        public string TeamText
        {
            get { return teamText; }
            set
            {
                teamText = value;
                OnPropertyChanged("TeamText");
            }
        }



        private ObservableCollection<LoggedInUser> users;

        public ObservableCollection<LoggedInUser> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged("Users");
            }
        }

        private LoggedInUser user;

        public LoggedInUser User
        {
            get { return user; }
            set
            {
                user = value;
                OnPropertyChanged("User");
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

        private ObservableCollection<StampModel> stamps;

        public ObservableCollection<StampModel> Stamps
        {
            get { return stamps; }
            set
            {
                stamps = value;
                OnPropertyChanged("Stamps");
            }
        }

        private StampModel stamp;

        public StampModel Stamp
        {
            get { return stamp; }
            set
            {
                stamp = value;
                OnPropertyChanged("Stamp");
            }
        }

        private string stampText;

        public string StampText
        {
            get { return stampText; }
            set
            {
                stampText = value;
                OnPropertyChanged("StampText");
            }
        }


        private ObservableCollection<SheetModel> sheets;

        public ObservableCollection<SheetModel> Sheets
        {
            get { return sheets; }
            set
            {
                sheets = value;
                OnPropertyChanged("Sheets");
            }
        }

        private SheetModel sheet;

        public SheetModel Sheet
        {
            get { return sheet; }
            set
            {
                sheet = value;
                OnPropertyChanged("Sheet");
            }
        }


        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }



        #endregion

        #region ToggleOperation Command

        private string operationsVisibility = "Collapsed";

        public string OperationsVisibility
        {
            get { return operationsVisibility; }
            set
            {
                operationsVisibility = value;
                OnPropertyChanged("OperationsVisibility");
            }
        }

        private int colSpan = 2;

        public int ColSpan
        {
            get { return colSpan; }
            set
            {
                colSpan = value;
                OnPropertyChanged("ColSpan");
            }
        }

        public ICommand ToggleOperationCommand { get; private set; }

        private void OpenCloseOperations(object value)
        {

            switch (value.ToString())
            {
                case "Edit":
                    if (SelectedTask != null)
                    {
                        ID = SelectedTask.ID;
                        Title = SelectedTask.Title;
                        Description = SelectedTask.Description;
                        Status = SelectedTask.Status;
                        Type = SelectedTask.Type;
                        Team = SelectedTask.Team;
                        User = SelectedTask.User;
                        StartDate = SelectedTask.StartDate;
                        DueDate = SelectedTask.DueDate;
                        Stamp = SelectedTask.Stamp;
                        Sheet = SelectedTask.Sheet;
                        ColSpan = 1;
                        OperationsVisibility = "Visible";
                        //IsUpdate = true;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Please select a record to edit", "Select Record", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                case "Create":
                    ID = 0;
                    //IsUpdate = false;
                    ColSpan = 1;
                    OperationsVisibility = "Visible";
                    ClearFields();
                    break;
                default:
                    ColSpan = ColSpan == 1 ? 2 : 1;
                    OperationsVisibility = OperationsVisibility == "Visible" ? "Collapsed" : "Visible";
                    break;

            }

        }

        #endregion

        #region Get Tasks

        private ObservableCollection<TaskModel> tasks;

        public ObservableCollection<TaskModel> Tasks
        {
            get { return tasks; }
            set
            {
                tasks = value;
                OnPropertyChanged("Tasks");
            }
        }

        private async Task GetTasks()
        {
            try
            {
                Tasks = await apiHelper.GetTasks(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Task Types

        private async Task GetTypes()
        {
            try
            {
                Types = await typeAPIHelper.GetTypes(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Task Statuses

        private async Task GetStatuses()
        {
            try
            {
                Statuses = await statusAPIHelper.GetStatuses(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Teams

        private async Task GetTeams()
        {
            try
            {
                Teams = await teamAPIHelper.GetTeams(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Stamps

        private async Task GetStamps()
        {
            try
            {
                Stamps = await stampAPIHelper.GetStamps(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }


        #endregion

        #region Get Sheets

        private async Task GetSheets()
        {
            try
            {
                Sheets = await sheetAPIHelper.GetSheets(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        //#region Create and Edit Project Command

        //private bool isUpdate;

        //public bool IsUpdate
        //{
        //    get { return isUpdate; }
        //    set
        //    {
        //        isUpdate = value;
        //        OnPropertyChanged("IsUpdate");
        //    }
        //}


        //public ICommand SaveCommand { get; private set; }

        //private bool canSaveProject = true;

        //public bool CanSaveProject
        //{
        //    get { return canSaveProject; }
        //    set
        //    {
        //        canSaveProject = value;
        //        OnPropertyChanged("CreateProject");
        //        OnPropertyChanged("IsSaveSpinning");
        //        OnPropertyChanged("SaveBtnText");
        //        OnPropertyChanged("SaveBtnIcon");
        //    }
        //}

        //public bool IsSaveSpinning => !canSaveProject;

        //public string SaveBtnText => canSaveProject ? "Save" : "Saving...";

        //public string SaveBtnIcon => canSaveProject ? "SaveRegular" : "SpinnerSolid";

        //private async Task CreateProject()
        //{
        //    try
        //    {
        //        List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
        //        {
        //            new KeyValuePair<string, string>("Title", Title),
        //            new KeyValuePair<string, string>("Address", Address)

        //        };
        //        if (FieldValidation.ValidateFields(values))
        //        {
        //            CanSaveProject = false;

        //            ProjectModel projectData = new ProjectModel()
        //            {
        //                Title = Title,
        //                Description = Description,
        //                ProjectTypeID = Type.ID,
        //                ProjectStatusID = Status.ID,
        //                StartDate = StartDate,
        //                DueDate = DueDate,
        //                Address = Address,
        //                ContractorID = Contractor.ID
        //            };
        //            HttpResponseMessage result = null;
        //            if (isUpdate)
        //            {
        //                projectData.ID = ID;
        //                projectData.CreatedBy = SelectedTask.CreatedBy;
        //                projectData.CreatedOn = SelectedTask.CreatedOn;
        //                projectData.ModifiedBy = ParentLayout.LoggedInUser.Name;
        //                projectData.ModifiedOn = DateTime.Now;
        //                result = await apiHelper.PutProject(ParentLayout.LoggedInUser.Token, projectData).ConfigureAwait(false);
        //            }
        //            else
        //            {
        //                projectData.CreatedBy = ParentLayout.LoggedInUser.Name;
        //                projectData.CreatedOn = DateTime.Now;
        //                result = await apiHelper.PostProject(ParentLayout.LoggedInUser.Token, projectData).ConfigureAwait(false);
        //            }
        //            if (result.IsSuccessStatusCode)
        //            {
        //                MessageBox.Show($"Project Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //                await GetTasks();
        //                await ParentLayout.GetProjects();
        //                IsUpdate = false;
        //                ClearFields();
        //            }
        //            else
        //            {
        //                MessageBox.Show("Error in saving Project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //            CanSaveProject = true;


        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        CanSaveProject = true;
        //    }

        //}

        private void ClearFields()
        {
            try
            {
                ID = 0;
                Title = "";
                Description = "";
                StartDate = DateTime.Today;
                DueDate = DateTime.Today;
            }
            catch (Exception)
            {

            }

        }
        //#endregion

        //#region Delete Project Command

        //public ICommand DeleteCommand { get; private set; }

        //private bool canDeleteProject = true;

        //public bool CanDeleteProject
        //{
        //    get { return canDeleteProject; }
        //    set
        //    {
        //        canSaveProject = value;
        //        OnPropertyChanged("DeleteProject");
        //        OnPropertyChanged("IsDeleteSpinning");
        //        OnPropertyChanged("DeleteBtnText");
        //        OnPropertyChanged("DeleteBtnIcon");
        //    }
        //}

        //public bool IsDeleteSpinning => !canDeleteProject;

        //public string DeleteBtnText => canDeleteProject ? "Delete" : "Deleting...";

        //public string DeleteBtnIcon => canDeleteProject ? "TrashAltRegular" : "SpinnerSolid";

        //private async Task DeleteProject()
        //{

        //    if (SelectedTask != null)
        //    {
        //        if (MessageBox.Show($"Are you sure you want to delete {SelectedTask.Title} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
        //            return;
        //        CanDeleteProject = false;
        //        try
        //        {
        //            HttpResponseMessage result = await apiHelper.DeleteProject(ParentLayout.LoggedInUser.Token, SelectedTask.ID).ConfigureAwait(false);
        //            if (result.IsSuccessStatusCode)
        //            {
        //                await GetTasks();
        //                await ParentLayout.GetProjects();
        //            }
        //            else
        //            {
        //                MessageBox.Show("Error in deleting Project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //            CanSaveProject = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            CanDeleteProject = true;
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select a Project to be deleted", "Select Project", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }

        //}

        //#endregion
    }
}
