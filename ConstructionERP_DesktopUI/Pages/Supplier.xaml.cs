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
    /// Interaction logic for Supplier.xaml
    /// </summary>
    public partial class Supplier : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private SupplierAPIHelper apiHelper;
        public Supplier(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new SupplierAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetSuppliers())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateSupplier()); }, () => CanSaveSupplier);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteSupplier()); }, () => CanDeleteSupplier);
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
        private SupplierModel selectedSupplier;

        public SupplierModel SelectedSupplier
        {
            get { return selectedSupplier; }
            set
            {
                selectedSupplier = value;
                OnPropertyChanged("SelectedSupplier");
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
        private string supplierName;

        public string SupplierName
        {
            get { return supplierName; }
            set
            {
                supplierName = value;
                OnPropertyChanged("SupplierName");
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
                    if (SelectedSupplier != null)
                    {
                        ID = SelectedSupplier.ID;
                        SupplierName = SelectedSupplier.Name;
                        Phone = SelectedSupplier.Phone;
                        Email = SelectedSupplier.Email;
                        GSTN = SelectedSupplier.GSTN;
                        WorkDescription = SelectedSupplier.WorkDescription;
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

        #region Get Suppliers

        private ObservableCollection<SupplierModel> suppliers;

        public ObservableCollection<SupplierModel> Suppliers
        {
            get { return suppliers; }
            set
            {
                suppliers = value;
                OnPropertyChanged("Suppliers");
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

        private async Task GetSuppliers()
        {
            try
            {
                IsProgressing = true;
                Suppliers = await apiHelper.GetSuppliers(ParentLayout.LoggedInUser.Token);
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

        #region Create and Edit Supplier Command

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

        private bool canSaveSupplier = true;

        public bool CanSaveSupplier
        {
            get { return canSaveSupplier; }
            set
            {
                canSaveSupplier = value;
                OnPropertyChanged("CreateSupplier");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveSupplier;

        public string SaveBtnText => canSaveSupplier ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveSupplier ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateSupplier()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Name", SupplierName),
                    new KeyValuePair<string, string>("Phone", Phone),
                    new KeyValuePair<string, string>("Email", Email),
                    new KeyValuePair<string, string>("GSTN", GSTN),
                    new KeyValuePair<string, string>("WorkDescription", WorkDescription)

                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveSupplier = false;
                try
                {
                    SupplierModel supplierData = new SupplierModel()
                    {
                        Name = SupplierName,
                        Phone = phone,
                        Email = Email,
                        GSTN = GSTN,
                        WorkDescription = WorkDescription,

                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        supplierData.ID = ID;
                        supplierData.CreatedBy = SelectedSupplier.CreatedBy;
                        supplierData.ModifiedOn = DateTime.Now;
                        supplierData.ModifiedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PutSupplier(ParentLayout.LoggedInUser.Token, supplierData).ConfigureAwait(false);
                    }
                    else
                    {
                        supplierData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostSupplier(ParentLayout.LoggedInUser.Token, supplierData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Supplier Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetSuppliers();
                        IsUpdate = false;
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Supplier", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveSupplier = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveSupplier = true;
                }

            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                SupplierName = "";
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

        #region Delete Supplier Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteSupplier = true;

        public bool CanDeleteSupplier
        {
            get { return canDeleteSupplier; }
            set
            {
                canSaveSupplier = value;
                OnPropertyChanged("DeleteSupplier");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteSupplier;

        public string DeleteBtnText => canDeleteSupplier ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteSupplier ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteSupplier()
        {

            if (SelectedSupplier != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedSupplier.Name} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteSupplier = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteSupplier(ParentLayout.LoggedInUser.Token, SelectedSupplier.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetSuppliers();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Supplier", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveSupplier = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteSupplier = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a Supplier to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion
    }
}
