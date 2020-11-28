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
    /// Interaction logic for Stamp.xaml
    /// </summary>
    public partial class TaskCategory : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private StampAPIHelper apiHelper;

        public TaskCategory(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new StampAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetStamps())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateStamp()); }, () => CanSaveStamp);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteStamp()); }, () => CanDeleteStamp);
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

        //Selected Stamp
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
                    if (SelectedStamp != null)
                    {
                        ID = SelectedStamp.ID;
                        Title = SelectedStamp.Title;
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

        #region Get Stamps

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

        private async Task GetStamps()
        {
            try
            {
                IsProgressing = true;
                Stamps = await apiHelper.GetStamps(ParentLayout.LoggedInUser.Token);
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

        #region Create and Edit Stamp Command

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

        private bool canSaveStamp = true;

        public bool CanSaveStamp
        {
            get { return canSaveStamp; }
            set
            {
                canSaveStamp = value;
                OnPropertyChanged("CreateStamp");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveStamp;

        public string SaveBtnText => canSaveStamp ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveStamp ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateStamp()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title)
                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveStamp = false;
                try
                {
                    StampModel typeData = new StampModel()
                    {
                        Title = Title,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        typeData.ID = ID;
                        typeData.CreatedBy = SelectedStamp.CreatedBy;
                        result = await apiHelper.PutStamp(ParentLayout.LoggedInUser.Token, typeData).ConfigureAwait(false);
                    }
                    else
                    {
                        typeData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostStamp(ParentLayout.LoggedInUser.Token, typeData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Stamp Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetStamps();
                        ClearFields();
                        IsUpdate = false;
                    }
                    else
                    {
                        MessageBox.Show("Error in saving stamp", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveStamp = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveStamp = true;
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

        #region Delete Type Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteStamp = true;

        public bool CanDeleteStamp
        {
            get { return canDeleteStamp; }
            set
            {
                canSaveStamp = value;
                OnPropertyChanged("DeleteStamp");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteStamp;

        public string DeleteBtnText => canDeleteStamp ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteStamp ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteStamp()
        {

            if (SelectedStamp != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedStamp.Title} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteStamp = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteStamp(ParentLayout.LoggedInUser.Token, SelectedStamp.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetStamps();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Stamp", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveStamp = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteStamp = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a Stamp to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion
    }
}
