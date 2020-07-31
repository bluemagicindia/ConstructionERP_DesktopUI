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
    /// Interaction logic for Firm.xaml
    /// </summary>
    public partial class Firm : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private FirmAPIHelper apiHelper;

        public Firm(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new FirmAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetFirms())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateFirm()); }, () => CanSaveFirm);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteFirm()); }, () => CanDeleteFirm);
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

        private FirmModel selectedFirm;

        public FirmModel SelectedFirm
        {
            get { return selectedFirm; }
            set
            {
                selectedFirm = value;
                OnPropertyChanged("SelectedFirm");
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
                    if (SelectedFirm != null)
                    {
                        ID = SelectedFirm.ID;
                        Title = SelectedFirm.Name;
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

        #region Get Firms

        private ObservableCollection<FirmModel> firms;

        public ObservableCollection<FirmModel> Firms
        {
            get { return firms; }
            set
            {
                firms = value;
                OnPropertyChanged("Firms");
            }
        }

        private async Task GetFirms()
        {
            try
            {
                Firms = await apiHelper.GetFirms(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Firm Command

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

        private bool canSaveFirm = true;

        public bool CanSaveFirm
        {
            get { return canSaveFirm; }
            set
            {
                canSaveFirm = value;
                OnPropertyChanged("CreateFirm");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveFirm;

        public string SaveBtnText => canSaveFirm ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveFirm ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateFirm()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title)
                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveFirm = false;
                try
                {
                    FirmModel firmData = new FirmModel()
                    {
                        Name = Title,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        firmData.ID = ID;
                        firmData.CreatedBy = SelectedFirm.CreatedBy;
                        result = await apiHelper.PutFirm(ParentLayout.LoggedInUser.Token, firmData).ConfigureAwait(false);
                    }
                    else
                    {
                        firmData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostFirm(ParentLayout.LoggedInUser.Token, firmData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Firm Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetFirms();
                        ClearFields();
                        IsUpdate = false;
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Firm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveFirm = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveFirm = true;
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

        #region Delete Firm Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteFirm = true;

        public bool CanDeleteFirm
        {
            get { return canDeleteFirm; }
            set
            {
                canSaveFirm = value;
                OnPropertyChanged("DeleteFirm");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteFirm;

        public string DeleteBtnText => canDeleteFirm ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteFirm ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteFirm()
        {

            if (SelectedFirm != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete '{SelectedFirm.Name}' Firm?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteFirm = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteFirm(ParentLayout.LoggedInUser.Token, SelectedFirm.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetFirms();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Firm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveFirm = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteFirm = true;
                }
            }
            else
            {
                MessageBox.Show("Please select an firm to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

    }
}
