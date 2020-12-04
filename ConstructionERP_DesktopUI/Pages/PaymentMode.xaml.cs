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
    /// Interaction logic for PaymentMode.xaml
    /// </summary>
    public partial class PaymentMode : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private PaymentModeAPIHelper apiHelper;

        public PaymentMode(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new PaymentModeAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetPaymentModes())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreatePaymentMode()); }, () => CanSavePaymentMode);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeletePaymentMode()); }, () => CanDeletePaymentMode);
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


        //Selected PaymentMode
        private PaymentModeModel selectedPaymentMode;

        public PaymentModeModel SelectedPaymentMode
        {
            get { return selectedPaymentMode; }
            set
            {
                selectedPaymentMode = value;
                OnPropertyChanged("SelectedPaymentMode");
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
                    if (SelectedPaymentMode != null)
                    {
                        ID = SelectedPaymentMode.ID;
                        Title = SelectedPaymentMode.Title;
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

        #region Get Payment Modes

        private ObservableCollection<PaymentModeModel> paymentModes;

        public ObservableCollection<PaymentModeModel> PaymentModes
        {
            get { return paymentModes; }
            set
            {
                paymentModes = value;
                OnPropertyChanged("PaymentModes");
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

        private async Task GetPaymentModes()
        {
            try
            {
                IsProgressing = true;
                PaymentModes = await apiHelper.GetPaymentModes(ParentLayout.LoggedInUser.Token);
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

        #region Create and Edit Payment Modes

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

        private bool canSavePaymentMode = true;

        public bool CanSavePaymentMode
        {
            get { return canSavePaymentMode; }
            set
            {
                canSavePaymentMode = value;
                OnPropertyChanged("CreatePaymentMode");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSavePaymentMode;

        public string SaveBtnText => canSavePaymentMode ? "Save" : "Saving...";

        public string SaveBtnIcon => canSavePaymentMode ? "SaveRegular" : "SpinnerSolid";

        private async Task CreatePaymentMode()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title)
                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSavePaymentMode = false;
                try
                {
                    PaymentModeModel paymentModeData = new PaymentModeModel()
                    {
                        Title = Title,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        paymentModeData.ID = ID;
                        paymentModeData.CreatedBy = SelectedPaymentMode.CreatedBy;
                        result = await apiHelper.PutPaymentMode(ParentLayout.LoggedInUser.Token, paymentModeData).ConfigureAwait(false);
                    }
                    else
                    {
                        paymentModeData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostPaymentMode(ParentLayout.LoggedInUser.Token, paymentModeData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Payment Mode Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetPaymentModes();
                        ClearFields();
                        IsUpdate = false;
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Payment Mode", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSavePaymentMode = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSavePaymentMode = true;
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

        #region Delete Payment Mode Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeletePaymentMode = true;


        public bool CanDeletePaymentMode
        {
            get { return canDeletePaymentMode; }
            set
            {
                canSavePaymentMode = value;
                OnPropertyChanged("DeletePaymentMode");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeletePaymentMode;

        public string DeleteBtnText => canDeletePaymentMode ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeletePaymentMode ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeletePaymentMode()
        {

            if (SelectedPaymentMode != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedPaymentMode.Title}'s Payment Mode?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeletePaymentMode = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeletePaymentMode(ParentLayout.LoggedInUser.Token, SelectedPaymentMode.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetPaymentModes();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Payment Mode", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSavePaymentMode = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeletePaymentMode = true;
                }
            }
            else
            {
                MessageBox.Show("Please select an Payment Mode to be deleted", "Select Payment Mode", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

    }
}
