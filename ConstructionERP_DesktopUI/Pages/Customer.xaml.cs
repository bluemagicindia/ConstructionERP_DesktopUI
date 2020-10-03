using ConstructionERP_DesktopUI.API;
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
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private CustomerAPIHelper apiHelper;

        public Customer(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }


        void SetValues()
        {
            apiHelper = new CustomerAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetCustomers())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateCustomer()); }, () => CanSaveCustomer);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteCustomer()); }, () => CanDeleteCustomer);
            ToggleFlatPopupCommand = new RelayCommand(ToggleFlatPopup);
            SaveFlatCommand = new RelayCommand(SaveFlat);
            DeleteFlatCommand = new RelayCommand(DeleteFlat);
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

        private string customerName;

        public string CustomerName
        {
            get { return customerName; }
            set
            {
                customerName = value;
                OnPropertyChanged("CustomerName");
            }
        }

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

        private string gSTN;

        public string GSTN
        {
            get { return gSTN; }
            set
            {
                gSTN = value;
                OnPropertyChanged("GSTN");
            }
        }

        private string phoneNumber;

        public string PhoneNumbers
        {
            get { return phoneNumber; }
            set
            {
                phoneNumber = value;
                OnPropertyChanged("PhoneNumbers");
            }
        }



        private string currentAddress;

        public string CurrentAddress
        {
            get { return currentAddress; }
            set
            {
                currentAddress = value;
                OnPropertyChanged("CurrentAddress");
            }
        }

        private string pAN;

        public string PAN
        {
            get { return pAN; }
            set
            {
                pAN = value;
                OnPropertyChanged("PAN");
            }
        }

        private string aadhaar;

        public string Aadhaar
        {
            get { return aadhaar; }
            set
            {
                aadhaar = value;
                OnPropertyChanged("Aadhaar");
            }
        }


        private string referredBy;

        public string ReferredBy
        {
            get { return referredBy; }
            set
            {
                referredBy = value;
                OnPropertyChanged("ReferredBy");
            }
        }


        private CustomerModel customerModel;

        public CustomerModel SelectedCustomer
        {
            get { return customerModel; }
            set
            {
                customerModel = value;
                OnPropertyChanged("SelectedCustomer");
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
                    if (SelectedCustomer != null)
                    {
                        ID = SelectedCustomer.ID;
                        CustomerName = SelectedCustomer.Name;
                        Email = SelectedCustomer.Email;
                        GSTN = SelectedCustomer.GSTN;
                        PhoneNumbers = SelectedCustomer.PhoneNumbers;
                        CurrentAddress = SelectedCustomer.CurrentAddress;
                        PAN = SelectedCustomer.PAN;
                        Aadhaar = SelectedCustomer.Aadhaar;
                        ReferredBy = SelectedCustomer.ReferredBy;
                        if (SelectedCustomer.Flats != null)
                        {
                            Flats = new ObservableCollection<FlatModel>(SelectedCustomer.Flats);
                        }
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

        #region Get Customers

        private ObservableCollection<CustomerModel> customers;

        public ObservableCollection<CustomerModel> Customers
        {
            get { return customers; }
            set
            {
                customers = value;
                OnPropertyChanged("Customers");
            }
        }

        private async Task GetCustomers()
        {
            try
            {
                Customers = await apiHelper.GetCustomers(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Customer Command

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

        private bool canSaveCustomer = true;

        public bool CanSaveCustomer
        {
            get { return canSaveCustomer; }
            set
            {
                canSaveCustomer = value;
                OnPropertyChanged("CreateCustomer");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveCustomer;

        public string SaveBtnText => canSaveCustomer ? "Save Customer" : "Saving...";

        public string SaveBtnIcon => canSaveCustomer ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateCustomer()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Name", CustomerName),
                    new KeyValuePair<string, string>("Phone Numbers", PhoneNumbers),
                    new KeyValuePair<string, string>("GSTN", GSTN),

                };
            if (FieldValidation.ValidateFields(values))
            {

                try
                {
                    if (Flats == null && MessageBox.Show("Are you sure you want to proceed without adding flats?", "Add Flats", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    CanSaveCustomer = false;
                    CustomerModel customerData = new CustomerModel()
                    {
                        Name = CustomerName,
                        Email = Email,
                        GSTN = GSTN,
                        PhoneNumbers = PhoneNumbers,
                        CurrentAddress = CurrentAddress,
                        PAN = PAN,
                        Aadhaar = Aadhaar,
                        ReferredBy = ReferredBy,
                        Flats = Flats
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        customerData.ID = ID;
                        customerData.Flats = customerData.Flats.Select(f => { f.CustomerId = customerData.ID; return f; }).ToList();
                        customerData.CreatedBy = SelectedCustomer.CreatedBy;
                        customerData.ModifiedOn = DateTime.Now;
                        customerData.ModifiedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PutCustomer(ParentLayout.LoggedInUser.Token, customerData).ConfigureAwait(false);
                    }
                    else
                    {
                        customerData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostCustomer(ParentLayout.LoggedInUser.Token, customerData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Customer Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetCustomers();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Customer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveCustomer = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveCustomer = true;
                }

            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                CustomerName = Email = GSTN = PhoneNumbers = CurrentAddress = PAN = Aadhaar = ReferredBy = String.Empty;
                Flats = null;
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete Contractor Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteCustomer = true;

        public bool CanDeleteCustomer
        {
            get { return canDeleteCustomer; }
            set
            {
                canSaveCustomer = value;
                OnPropertyChanged("DeleteCustomer");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteCustomer;

        public string DeleteBtnText => canDeleteCustomer ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteCustomer ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteCustomer()
        {

            if (SelectedCustomer != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedCustomer.Name} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteCustomer = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteCustomer(ParentLayout.LoggedInUser.Token, SelectedCustomer.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetCustomers();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Customer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveCustomer = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteCustomer = true;
                }
            }
            else
            {
                MessageBox.Show("Please select an Customer to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

        #region Flat Popup Open / Dimensions

        private bool isPopupOpen;

        public bool IsPopupOpen
        {
            get { return isPopupOpen; }
            set
            {
                isPopupOpen = value;
                OnPropertyChanged("IsPopupOpen");
            }
        }

        private double popupHeight;

        public double PopupHeight
        {
            get { return popupHeight; }
            set
            {
                popupHeight = value;
                OnPropertyChanged("PopupHeight");
            }
        }

        private double popupWidth;

        public double PopupWidth
        {
            get { return popupWidth; }
            set
            {
                popupWidth = value;
                OnPropertyChanged("PopupWidth");
            }
        }

        public ICommand ToggleFlatPopupCommand { get; private set; }

        void ToggleFlatPopup(object param)
        {
            if (param?.ToString() == "Open")
            {
                PopupHeight = this.ActualHeight;
                PopupWidth = this.ActualWidth;
                IsPopupOpen = true;
            }
            else
            {
                IsPopupOpen = false;
            }
        }


        #endregion

        #region Flat Operations

        private string flatNumber;

        public string FlatNumber
        {
            get { return flatNumber; }
            set
            {
                flatNumber = value;
                OnPropertyChanged("FlatNumber");
            }
        }

        private ProjectModel selectedProject;

        public ProjectModel SelectedProject
        {
            get { return selectedProject; }
            set
            {
                selectedProject = value;
                OnPropertyChanged("SelectedProject");
            }
        }

        private decimal estimatedAmount;

        public decimal EstimatedAmount
        {
            get { return estimatedAmount; }
            set
            {
                estimatedAmount = value;
                OnPropertyChanged("EstimatedAmount");
            }
        }

        private decimal averageTotal;

        public decimal AverageTotal
        {
            get { return averageTotal; }
            set
            {
                averageTotal = value;
                OnPropertyChanged("AverageTotal");
            }
        }

        private int eMI;

        public int EMI
        {
            get { return eMI; }
            set
            {
                eMI = value;
                OnPropertyChanged("EMI");
            }
        }

        private int days;

        public int Days
        {
            get { return days; }
            set
            {
                days = value;
                OnPropertyChanged("Days");
            }
        }


        private ObservableCollection<FlatModel> flats;

        public ObservableCollection<FlatModel> Flats
        {
            get { return flats; }
            set
            {
                flats = value;
                OnPropertyChanged("Flats");
            }
        }

        private FlatModel selectedFlat;

        public FlatModel SelectedFlat
        {
            get { return selectedFlat; }
            set
            {
                selectedFlat = value;
                OnPropertyChanged("SelectedFlat");
            }
        }

        private string errorVisibility;

        public string ErrorVisibility
        {
            get { return errorVisibility; }
            set
            {
                errorVisibility = value;
                OnPropertyChanged("ErrorVisibility");
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        public ICommand SaveFlatCommand { get; private set; }

        void SaveFlat(object param)
        {
            try
            {
                ErrorVisibility = "Collapsed";

                if (validateFlat())
                {
                    if (Flats == null)
                    {
                        Flats = new ObservableCollection<FlatModel>();
                    }
                    Flats.Add(new FlatModel
                    {
                        Number = FlatNumber,
                        ProjectId = SelectedProject.ID,
                        EstimatedAmount = EstimatedAmount,
                        AverageTotal = AverageTotal,
                        EMI = EMI,
                        Days = Days
                    });
                    ClearFlatFields();
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                ErrorVisibility = "Visible";
            }
        }


        public ICommand DeleteFlatCommand { get; private set; }

        void DeleteFlat(object param)
        {
            try
            {
                ErrorVisibility = "Collapsed";
                if (SelectedFlat != null)
                {
                    Flats.Remove(SelectedFlat);
                }
                else
                {
                    ErrorMessage = "Please select a flat to be deleted";
                    ErrorVisibility = "Visible";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                ErrorVisibility = "Visible";
            }

        }

        bool validateFlat()
        {
            if (string.IsNullOrEmpty(FlatNumber))
            {
                ErrorMessage = "Please enter flat number";
                ErrorVisibility = "Visible";
                return false;
            }
            else if (SelectedProject == null)
            {
                ErrorMessage = "Please select a project";
                ErrorVisibility = "Visible";
                return false;
            }
            else if (EstimatedAmount <= 0)
            {
                ErrorMessage = "Please enter Estimated Amount";
                ErrorVisibility = "Visible";
                return false;
            }
            else if (AverageTotal <= 0)
            {
                ErrorMessage = "Please enter Average Total";
                ErrorVisibility = "Visible";
                return false;
            }
            else if (EMI <= 0)
            {
                ErrorMessage = "Please enter EMI";
                ErrorVisibility = "Visible";
                return false;
            }
            else if (Days <= 0)
            {
                ErrorMessage = "Please enter Days";
                ErrorVisibility = "Visible";
                return false;
            }
            else
            {
                ErrorMessage = string.Empty;
                ErrorVisibility = "Collapsed";
                return true;
            }
        }

        void ClearFlatFields()
        {
            FlatNumber = string.Empty;
            SelectedProject = null;
            EstimatedAmount = AverageTotal = EMI = Days = 0;
        }


        #endregion

    }
}
