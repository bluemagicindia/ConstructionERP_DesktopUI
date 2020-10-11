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
    /// Interaction logic for SiteManager.xaml
    /// </summary>
    public partial class SiteManager : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private SiteManagerAPIHelper apiHelper;

        public SiteManager(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }
        void SetValues()
        {
            apiHelper = new SiteManagerAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetSiteManagers())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateSiteManager()); }, () => CanSaveSiteManager);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteSiteManager()); }, () => CanDeleteSiteManager);
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

        //Selected Enquiry
        private SiteManagerModel selectedSiteManager;

        public SiteManagerModel SelectedSiteManager
        {
            get { return selectedSiteManager; }
            set
            {
                selectedSiteManager = value;
                OnPropertyChanged("SelectedSiteManager");
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
        private string siteManagerName;

        public string SiteManagerName
        {
            get { return siteManagerName; }
            set
            {
                siteManagerName = value;
                OnPropertyChanged("SiteManagerName");
            }
        }

        //Phone
        private string phone;

        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                OnPropertyChanged("Phone");
            }
        }

        //Email
        private string email;

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged("Email");
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
                    if (SelectedSiteManager != null)
                    {
                        ID = SelectedSiteManager.ID;
                        SiteManagerName = SelectedSiteManager.Name;
                        Phone = SelectedSiteManager.Phone;
                        Email = SelectedSiteManager.Email;
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

        #region Get SiteManagers

        private ObservableCollection<SiteManagerModel> siteManagers;

        public ObservableCollection<SiteManagerModel> SiteManagers
        {
            get { return siteManagers; }
            set
            {
                siteManagers = value;
                OnPropertyChanged("SiteManagers");
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

        private async Task GetSiteManagers()
        {
            try
            {
                IsProgressing = true;
                SiteManagers = await apiHelper.GetSiteManagers(ParentLayout.LoggedInUser.Token);
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

        #region Create and Edit SiteManager Command

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

        private bool canSaveSiteManager = true;

        public bool CanSaveSiteManager
        {
            get { return canSaveSiteManager; }
            set
            {
                canSaveSiteManager = value;
                OnPropertyChanged("CreateSiteManager");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveSiteManager;

        public string SaveBtnText => canSaveSiteManager ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveSiteManager ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateSiteManager()
        {

            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Name", SiteManagerName),
                    new KeyValuePair<string, string>("Phone", Phone),
                    new KeyValuePair<string, string>("Email", Email)

                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveSiteManager = false;
                try
                {
                    SiteManagerModel siteManagerData = new SiteManagerModel()
                    {
                        Name = SiteManagerName,
                        Phone = phone,
                        Email = Email,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        siteManagerData.ID = ID;
                        siteManagerData.CreatedBy = SelectedSiteManager.CreatedBy;
                        siteManagerData.ModifiedOn = DateTime.Now;
                        siteManagerData.ModifiedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PutSiteManager(ParentLayout.LoggedInUser.Token, siteManagerData).ConfigureAwait(false);
                    }
                    else
                    {
                        siteManagerData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostSiteManager(ParentLayout.LoggedInUser.Token, siteManagerData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Site Manager Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetSiteManagers();
                        IsUpdate = false;
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in saving SiteManager", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveSiteManager = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveSiteManager = true;
                }

            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                SiteManagerName = "";
                Phone = "";
                Email = "";
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete SiteManager Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteSiteManager = true;

        public bool CanDeleteSiteManager
        {
            get { return canDeleteSiteManager; }
            set
            {
                canSaveSiteManager = value;
                OnPropertyChanged("DeleteSiteManager");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteSiteManager;

        public string DeleteBtnText => canDeleteSiteManager ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteSiteManager ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteSiteManager()
        {

            if (SelectedSiteManager != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedSiteManager.Name} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteSiteManager = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteSiteManager(ParentLayout.LoggedInUser.Token, SelectedSiteManager.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetSiteManagers();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting SiteManager", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveSiteManager = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteSiteManager = true;
                }
            }
            else
            {
                MessageBox.Show("Please select an SiteManager to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion
    }
}
