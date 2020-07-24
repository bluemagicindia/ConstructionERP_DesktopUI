﻿using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private SheetAPIHelper sheetAPIHelper;
        private TeamSiteManagersAPIHelper teamSiteManagersAPIHelper;
        private TaskMembersAPIHelper taskMembersAPIHelper;

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
            sheetAPIHelper = new SheetAPIHelper();
            teamSiteManagersAPIHelper = new TeamSiteManagersAPIHelper();
            taskMembersAPIHelper = new TaskMembersAPIHelper();

            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetTasks())();
            new Action(async () => await GetTypes())();
            new Action(async () => await GetStatuses())();
            new Action(async () => await GetTeamMembers())();
            new Action(async () => await GetStamps())();
            new Action(async () => await GetSheets())();

            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateTask()); }, () => CanSaveTask);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteTask()); }, () => CanDeleteTask);
            CheckCommand = new RelayCommand(SetCheckedText);

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

        private ObservableCollection<SiteManagerModel> teamMembers;

        public ObservableCollection<SiteManagerModel> TeamMembers
        {
            get { return teamMembers; }
            set
            {
                teamMembers = value;
                OnPropertyChanged("TeamMembers");
            }
        }


        private string teamMembersText;

        public string TeamMembersText
        {
            get { return teamMembersText; }
            set
            {
                teamMembersText = value;
                OnPropertyChanged("TeamMembersText");
            }
        }

        private ObservableCollection<SiteManagerModel> watchingMembers;

        public ObservableCollection<SiteManagerModel> WatchingMembers
        {
            get { return watchingMembers; }
            set
            {
                watchingMembers = value;
                OnPropertyChanged("WatchingMembers");
            }
        }

        private SiteManagerModel selectedWatchingMember;

        public SiteManagerModel SelectedWatchingMember
        {
            get { return selectedWatchingMember; }
            set
            {
                selectedWatchingMember = value;
                OnPropertyChanged("SelectedWatchingMember");
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

        private StampModel selectedStamp;

        public StampModel SelectedStamp
        {
            get { return selectedStamp; }
            set
            {
                selectedStamp = value;
                OnPropertyChanged("SelectedStamp");
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

        private SheetModel selectedSheet;

        public SheetModel SelectedSheet
        {
            get { return selectedSheet; }
            set
            {
                selectedSheet = value;
                OnPropertyChanged("SelectedSheet");
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

        private async void OpenCloseOperations(object value)
        {

            switch (value.ToString())
            {
                case "Edit":
                    if (SelectedTask != null)
                    {
                        await GetTeamMembers();
                        ColSpan = 1;
                        OperationsVisibility = "Visible";

                        ID = SelectedTask.ID;
                        Title = SelectedTask.Title;
                        Description = SelectedTask.Description;
                        SelectedStatus = SelectedTask.Status;
                        SelectedType = SelectedTask.Type;
                      
                        StartDate = SelectedTask.StartDate;
                        DueDate = SelectedTask.DueDate;
                        SelectedStamp = SelectedTask.Stamp;
                        SelectedSheet = SelectedTask.Sheet;

                        var taskMembers = await taskMembersAPIHelper.GetTaskMembersByTaskID(ParentLayout.LoggedInUser.Token, ID);

                        foreach (var tm in taskMembers)
                        {
                            tm.SiteManager.IsChecked = true;
                            TeamMembers.FirstOrDefault(w => w.ID == tm.SiteManager.ID).IsChecked = true;
                        }

                        SetCheckedText(null);
                        SelectedWatchingMember = SelectedTask.Watching;

                        IsUpdate = true;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Please select a record to edit", "Select Record", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                case "Create":
                    ID = 0;
                    IsUpdate = false;
                    ColSpan = 1;
                    OperationsVisibility = "Visible";
                    ClearFields();
                    await GetTeamMembers();
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

        #region Get Team Members

        private async Task GetTeamMembers()
        {
            try
            {
                var teamSiteManagers = await teamSiteManagersAPIHelper.GetTeamSiteManagersByTeamID(ParentLayout.LoggedInUser.Token, ParentLayout.SelectedProject.Team.ID);
                TeamMembers = new ObservableCollection<SiteManagerModel>();
                teamSiteManagers.Distinct().ToList().ForEach(tm => TeamMembers.Add(tm.SiteManager));

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

        #region Create and Edit Task Command

        private bool isUpdate;

        public bool IsUpdate
        {
            get { return isUpdate; }
            set
            {
                isUpdate = value;
                OnPropertyChanged("IsUpdate");
            }
        }


        public ICommand SaveCommand { get; private set; }

        private bool canSaveTask = true;

        public bool CanSaveTask
        {
            get { return canSaveTask; }
            set
            {
                canSaveTask = value;
                OnPropertyChanged("CreateTask");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveTask;

        public string SaveBtnText => canSaveTask ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveTask ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateTask()
        {
            try
            {
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title),
                    new KeyValuePair<string, string>("Task Type", TypeText),
                    new KeyValuePair<string, string>("Task Status", StatusText),
                    new KeyValuePair<string, string>("Task Stamp", StampText),
                    new KeyValuePair<string, string>("Sheet", SelectedSheet?.Title),
                    new KeyValuePair<string, string>("Watching", SelectedWatchingMember?.Name),

                };
                if (FieldValidation.ValidateFields(values))
                {

                    if (WatchingMembers.Count() <= 0)
                    {
                        MessageBox.Show("Please add atleast 1 Team Member to the Task", "Add Team Members", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (WatchingMembers.Where(w => w.ID == SelectedWatchingMember.ID).Count() <= 0)
                    {
                        MessageBox.Show("Please select watching member from the Selected Team Members", "Select Watching Members", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        CanSaveTask = false;

                        List<TaskMembersModel> taskMembers = new List<TaskMembersModel>();

                        TeamMembers.Where(s => s.IsChecked).ToList().ForEach(s => taskMembers.Add(
                            new TaskMembersModel
                            {
                                TaskID = ID,
                                SiteManagerID = s.ID,
                            }));


                        TaskModel taskData = new TaskModel()
                        {
                            ProjectID = ParentLayout.SelectedProject.ID,
                            Title = Title,
                            Description = Description,
                            TaskStatusID = SelectedStatus?.ID,
                            Status = SelectedStatus == null ? new StatusModel { Title = StatusText, CreatedBy = ParentLayout.LoggedInUser.Name } : null,
                            TaskTypeID = SelectedType?.ID,
                            Type = SelectedType == null ? new TypeModel { Title = TypeText, CreatedBy = ParentLayout.LoggedInUser.Name } : null,
                            StartDate = StartDate,
                            DueDate = DueDate,
                            Members = taskMembers,
                            StampID = SelectedStamp?.ID,
                            Stamp = SelectedStamp == null ? new StampModel { Title = StampText, CreatedBy = ParentLayout.LoggedInUser.Name } : null,
                            SheetID = SelectedSheet?.ID,
                            SiteManagerID = SelectedWatchingMember?.ID,

                        };

                        HttpResponseMessage result = null;
                        if (isUpdate)
                        {
                            taskData.ID = ID;
                            taskData.CreatedBy = SelectedTask.CreatedBy;
                            taskData.CreatedOn = SelectedTask.CreatedOn;
                            taskData.ModifiedBy = ParentLayout.LoggedInUser.Name;
                            taskData.ModifiedOn = DateTime.Now;
                            result = await apiHelper.PutTask(ParentLayout.LoggedInUser.Token, taskData).ConfigureAwait(false);
                        }
                        else
                        {
                            taskData.CreatedBy = ParentLayout.LoggedInUser.Name;
                            taskData.CreatedOn = DateTime.Now;
                            result = await apiHelper.PostTask(ParentLayout.LoggedInUser.Token, taskData).ConfigureAwait(false);
                        }
                        if (result.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Task Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            await GetTasks();
                            IsUpdate = false;
                            ClearFields();
                            await GetStamps();
                            await GetTypes();
                            await GetStatuses();
                            await GetTeamMembers();
                        }
                        else
                        {
                            MessageBox.Show("Error in saving Task", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        CanSaveTask = true;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CanSaveTask = true;
            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                Title = Description = StatusText = TypeText = StampText = TeamMembersText = string.Empty;
                StartDate = DateTime.Today;
                DueDate = DateTime.Today;
                SelectedSheet = null;
                SelectedStamp = null;
                SelectedType = null;
                SelectedWatchingMember = null;
                SelectedStatus = null;
                WatchingMembers = null;
            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Delete Project Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteTask = true;

        public bool CanDeleteTask
        {
            get { return canDeleteTask; }
            set
            {
                canDeleteTask = value;
                OnPropertyChanged("DeleteTask");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteTask;

        public string DeleteBtnText => canDeleteTask ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteTask ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteTask()
        {

            if (SelectedTask != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedTask.Title} Task?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteTask = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteTask(ParentLayout.LoggedInUser.Token, SelectedTask.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetTasks();
                        ClearFields();
                        await GetTeamMembers();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Task", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanDeleteTask = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteTask = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a Task to be deleted", "Select Project", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

        #region Check Command

        public ICommand CheckCommand { get; private set; }

        private void SetCheckedText(object param)
        {
            try
            {
                WatchingMembers = new ObservableCollection<SiteManagerModel>();

                List<string> checkedMembers = new List<string>();
                foreach (var tm in TeamMembers.Where(t => t.IsChecked))
                {
                    checkedMembers.Add(tm.Name);
                    WatchingMembers.Add(tm);
                }
                TeamMembersText = string.Join(", ", checkedMembers);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion
    }
}
