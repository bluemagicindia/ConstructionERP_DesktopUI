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
    /// Interaction logic for Contractor.xaml
    /// </summary>
    public partial class Contractor : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private ContractorAPIHelper apiHelper;

        public Contractor(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }


        void SetValues()
        {
            apiHelper = new ContractorAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetContractors())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateContractor()); }, () => CanSaveContractor);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteContractor()); }, () => CanDeleteContractor);
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
        private ContractorModel selectedContractor;

        public ContractorModel SelectedContractor
        {
            get { return selectedContractor; }
            set
            {
                selectedContractor = value;
                OnPropertyChanged("SelectedContractor");
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
        private string contractorName;

        public string ContractorName
        {
            get { return contractorName; }
            set
            {
                contractorName = value;
                OnPropertyChanged("ContractorName");
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

        //GSTN
        private string gstn;

        public string GSTN
        {
            get { return gstn; }
            set
            {
                gstn = value;
                OnPropertyChanged("GSTN");
            }
        }

        //WorkDescription
        private string workdescription;

        public string WorkDescription
        {
            get { return workdescription; }
            set
            {
                workdescription = value;
                OnPropertyChanged("WorkDescription");
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
                    if (SelectedContractor != null)
                    {
                        ID = SelectedContractor.ID;
                        ContractorName = SelectedContractor.Name;
                        Phone = SelectedContractor.Phone;
                        Email = SelectedContractor.Email;
                        GSTN = SelectedContractor.GSTN;
                        WorkDescription = SelectedContractor.WorkDescription;
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

        #region Get Contractors

        private ObservableCollection<ContractorModel> contractors;

        public ObservableCollection<ContractorModel> Contractors
        {
            get { return contractors; }
            set
            {
                contractors = value;
                OnPropertyChanged("Contractors");
            }
        }

        private async Task GetContractors()
        {
            try
            {
                Contractors = await apiHelper.GetContractors(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Contractor Command

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

        private bool canSaveContractor = true;

        public bool CanSaveContractor
        {
            get { return canSaveContractor; }
            set
            {
                canSaveContractor = value;
                OnPropertyChanged("CreateContractor");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveContractor;

        public string SaveBtnText => canSaveContractor ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveContractor ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateContractor()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Name", ContractorName),
                    new KeyValuePair<string, string>("Phone", Phone),
                    new KeyValuePair<string, string>("Email", Email),
                    new KeyValuePair<string, string>("GSTN", GSTN),
                    new KeyValuePair<string, string>("WorkDescription", WorkDescription)

                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveContractor = false;
                try
                {
                    ContractorModel contractorData = new ContractorModel()
                    {
                        Name = ContractorName,
                        Phone = phone,
                        Email = Email,
                        GSTN = GSTN,
                        WorkDescription = WorkDescription,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        contractorData.ID = ID;
                        contractorData.CreatedBy = SelectedContractor.CreatedBy;
                        contractorData.ModifiedOn = DateTime.Now;
                        contractorData.ModifiedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PutContractor(ParentLayout.LoggedInUser.Token, contractorData).ConfigureAwait(false);
                    }
                    else
                    {
                        contractorData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostContractor(ParentLayout.LoggedInUser.Token, contractorData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Contractor Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetContractors();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Contractor", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveContractor = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveContractor = true;
                }

            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                ContractorName = "";
                Phone = "";
                Email = "";
                GSTN = "";
                WorkDescription = "";
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete Contractor Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteContractor = true;

        public bool CanDeleteContractor
        {
            get { return canDeleteContractor; }
            set
            {
                canSaveContractor = value;
                OnPropertyChanged("DeleteContractor");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteContractor;

        public string DeleteBtnText => canDeleteContractor ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteContractor ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteContractor()
        {

            if (SelectedContractor != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedContractor.Name} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteContractor = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteContractor(ParentLayout.LoggedInUser.Token, SelectedContractor.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetContractors();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Contractor", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveContractor = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteContractor = true;
                }
            }
            else
            {
                MessageBox.Show("Please select an Contractor to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion
    }
}
