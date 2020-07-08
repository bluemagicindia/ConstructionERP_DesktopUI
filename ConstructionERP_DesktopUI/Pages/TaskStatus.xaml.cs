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
using System.Windows.Controls;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for TaskStatus.xaml
    /// </summary>
    public partial class TaskStatus : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private TaskStatusAPIHelper apiHelper;

        public TaskStatus(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new TaskStatusAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetStatuses())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateStatus()); }, () => CanSaveStatus);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteStatus()); }, () => CanDeleteStatus);
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

        //Selected Status
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


        //Name
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



        #endregion

        #region ToggleOperation Command

        private string operationsVisibility = "Visible";

        public string OperationsVisibility
        {
            get { return operationsVisibility; }
            set
            {
                operationsVisibility = value;
                OnPropertyChanged("OperationsVisibility");
            }
        }

        private int colSpan = 1;

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
                    if (SelectedStatus != null)
                    {
                        ID = SelectedStatus.ID;
                        Title = SelectedStatus.Title;
                        ColSpan = 1;
                        OperationsVisibility = "Visible";
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
                    break;
                default:
                    ColSpan = ColSpan == 1 ? 2 : 1;
                    OperationsVisibility = OperationsVisibility == "Visible" ? "Collapsed" : "Visible";
                    break;

            }

        }

        #endregion

        #region Get Statuses

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

        private async Task GetStatuses()
        {
            try
            {
                Statuses = await apiHelper.GetStatuses(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Status Command

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

        private bool canSaveStatus = true;

        public bool CanSaveStatus
        {
            get { return canSaveStatus; }
            set
            {
                canSaveStatus = value;
                OnPropertyChanged("CreateStatus");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveStatus;

        public string SaveBtnText => canSaveStatus ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveStatus ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateStatus()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title)
                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveStatus = false;
                try
                {
                    StatusModel statusData = new StatusModel()
                    {
                        Title = Title,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        statusData.ID = ID;
                        statusData.CreatedBy = SelectedStatus.CreatedBy;
                        result = await apiHelper.PutStatus(ParentLayout.LoggedInUser.Token, statusData).ConfigureAwait(false);
                    }
                    else
                    {
                        statusData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostStatus(ParentLayout.LoggedInUser.Token, statusData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Status Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetStatuses();
                        ClearFields();
                        IsUpdate = false;
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Status", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveStatus = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveStatus = true;
                }

            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                Title = "";
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete Status Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteStatus = true;

        public bool CanDeleteStatus
        {
            get { return canDeleteStatus; }
            set
            {
                canSaveStatus = value;
                OnPropertyChanged("DeleteStatus");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteStatus;

        public string DeleteBtnText => canDeleteStatus ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteStatus ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteStatus()
        {

            if (SelectedStatus != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedStatus.Title} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteStatus = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteStatus(ParentLayout.LoggedInUser.Token, SelectedStatus.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetStatuses();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Status", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveStatus = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteStatus = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a Status to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion
    }
}
