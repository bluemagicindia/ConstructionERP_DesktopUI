using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for TentativePaymentPopup.xaml
    /// </summary>
    public partial class CustomerPaymentPopup : Window, INotifyPropertyChanged
    {

        #region Initialization

        private CustomerPaymentAPIHelper apiHelper;

        public CustomerPaymentPopup(MainLayout parentLayout, CustomerModel customer)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = parentLayout;
            Customer = customer;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new CustomerPaymentAPIHelper();
            new Action(async () => await GetTransactions())();
            PaymentCommand = new RelayCommand(async delegate { await Task.Run(() => CreatePayment()); }, () => CanPay);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteTransaction()); }, () => CanDeleteTransaction);
            ClosePopupCommand = new RelayCommand(ClosePopup);

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

        private FlatModel selectedFlat;

        public FlatModel SelectedFlat
        {
            get { return selectedFlat; }
            set
            {
                selectedFlat = value;
                OnPropertyChanged("SelectedFlat");
                SetExtraWorkTotal();
            }
        }

        private CustomerModel customer;

        public CustomerModel Customer
        {
            get { return customer; }
            set
            {
                customer = value;
                OnPropertyChanged("Customer");
            }
        }

        private decimal aggregateAmountReceived;

        public decimal AggregateAmountReceived
        {
            get { return aggregateAmountReceived; }
            set
            {
                aggregateAmountReceived = value;
                OnPropertyChanged("AggregateAmountReceived");
            }
        }

        private decimal extraWorkTotal;

        public decimal ExtraWorkTotal
        {
            get { return extraWorkTotal; }
            set
            {
                extraWorkTotal = value;
                OnPropertyChanged("ExtraWorkTotal");
            }
        }

        private decimal extraWorkReceived;

        public decimal ExtraWorkReceived
        {
            get { return extraWorkReceived; }
            set
            {
                extraWorkReceived = value;
                OnPropertyChanged("ExtraWorkReceived");
            }
        }

        private decimal gSTReceived;

        public decimal GSTReceived
        {
            get { return gSTReceived; }
            set
            {
                gSTReceived = value;
                OnPropertyChanged("GSTReceived");
            }
        }

        private string stampDutyBalance;

        public string StampDutyBalance
        {
            get { return stampDutyBalance; }
            set
            {
                stampDutyBalance = value;
                OnPropertyChanged("StampDutyBalance");
            }
        }




        private DateTime paymentDate = DateTime.Today;
        public DateTime PaymentDate
        {
            get { return paymentDate; }
            set
            {
                paymentDate = value;
                OnPropertyChanged("AmountDate");
            }
        }

        private string remarks;

        public string Remarks
        {
            get { return remarks; }
            set
            {
                remarks = value;
                OnPropertyChanged("Remarks");
            }
        }

        #endregion

        #region Get Transactions

        private ObservableCollection<CustomerPaymentModel> customerTransactions;

        public ObservableCollection<CustomerPaymentModel> CustomerTransactions
        {
            get { return customerTransactions; }
            set
            {
                customerTransactions = value;
                OnPropertyChanged("CustomerTransactions");
            }
        }

        private CustomerPaymentModel selectedTransaction;

        public CustomerPaymentModel SelectedTransaction
        {
            get { return selectedTransaction; }
            set
            {
                selectedTransaction = value;
                OnPropertyChanged("SelectedTransaction");
            }
        }


        private async Task GetTransactions()
        {
            long cumulative = 0;
            try
            {
                CustomerTransactions = await apiHelper.GetCustomerPayments(Customer.ID, ParentLayout.LoggedInUser.Token);
                //CustomerTransactions.ToList().ForEach(ct => ct.Cumulative = cumulative = cumulative + ct.TentativeAmount - ct.PaidAmount);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }


        #endregion

        #region Create Transaction

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

        public ICommand PaymentCommand { get; private set; }

        private bool canPay = true;

        public bool CanPay
        {
            get { return canPay; }
            set
            {
                canPay = value;
                OnPropertyChanged("CreatePayment");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canPay;

        public string SaveBtnText => canPay ? "Save" : "Saving...";

        public string SaveBtnIcon => canPay ? "SaveRegular" : "SpinnerSolid";

        private async Task CreatePayment()
        {
            try
            {
                ErrorMessage = "";
                ErrorVisibility = "Collapsed";
                if (validatePayment())
                {

                    CanPay = false;

                    CustomerPaymentModel paymentData = new CustomerPaymentModel
                    {
                        FlatID = SelectedFlat.Id,
                        CustomerID = Customer.ID,
                        AggregateAmountReceived = AggregateAmountReceived,
                        ExtraWorkTotal = ExtraWorkTotal,
                        ExtraWorkReceived = ExtraWorkReceived,
                        GSTReceived = GSTReceived,
                        StampDutyBalance = StampDutyBalance,
                        PaymentDate = PaymentDate,
                        Remarks = Remarks,
                        Status = false,
                        CreatedOn = DateTime.Now,
                        CreatedBy = ParentLayout.LoggedInUser.Name
                    };


                    HttpResponseMessage result = await apiHelper.PostCustomerPayment(ParentLayout.LoggedInUser.Token, paymentData).ConfigureAwait(false);

                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Customer Payment Done Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetTransactions();
                        ClearFields();
                        //Application.Current.Dispatcher.Invoke((Action)delegate
                        //{
                        //    ClearFields();
                        //});
                    }
                    else
                    {
                        MessageBox.Show("Error in payment", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanPay = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CanPay = true;
            }

        }

        bool validatePayment()
        {
            if (SelectedFlat == null)
            {
                ErrorMessage = "Please select a flat";
                ErrorVisibility = "Visible";
                return false;
            }
            else if (AggregateAmountReceived <= 0 && ExtraWorkTotal <= 0 && ExtraWorkReceived <= 0)
            {
                ErrorMessage = "Please enter Aggregate or Extra Amount";
                ErrorVisibility = "Visible";
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ClearFields()
        {
            try
            {
                AggregateAmountReceived = ExtraWorkReceived = GSTReceived = 0;
                PaymentDate = DateTime.Today;
                Remarks = StampDutyBalance = string.Empty;
            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Delete Transaction Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteTransaction = true;

        public bool CanDeleteTransaction
        {
            get { return canDeleteTransaction; }
            set
            {
                canDeleteTransaction = value;
                OnPropertyChanged("DeleteTransaction");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteTransaction;

        public string DeleteBtnText => canDeleteTransaction ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteTransaction ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteTransaction()
        {

            if (SelectedTransaction != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete this transaction?", "Delete Transaction", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteTransaction = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteCustomerPayment(ParentLayout.LoggedInUser.Token, SelectedTransaction.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetTransactions();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Transaction", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanDeleteTransaction = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteTransaction = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a Transaction to be deleted", "Select Transaction", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

        #region Close Popup Command

        public ICommand ClosePopupCommand { get; private set; }

        private void ClosePopup(object param)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }


        #endregion

        #region Popup Dimensions

        private double popupMaxHeight;

        public double PopupMaxHeight
        {
            get { return popupMaxHeight; }
            set
            {
                popupMaxHeight = value;
                OnPropertyChanged("PopupMaxHeight");
            }
        }

        private double popupMaxWidth;

        public double PopupMaxWidth
        {
            get { return popupMaxWidth; }
            set
            {
                popupMaxWidth = value;
                OnPropertyChanged("PopupMaxWidth");
            }
        }

        void SetDimensions()
        {
            PopupMaxHeight = SystemParameters.WorkArea.Width;
            PopupMaxWidth = SystemParameters.WorkArea.Height;
        }

        #endregion

        #region Set Extra Work Total

        void SetExtraWorkTotal()
        {
            if (SelectedFlat != null)
            {
                ExtraWorkTotal = CustomerTransactions.Where(ct => ct.Flat.Id == SelectedFlat.Id).Select(ct => ct.ExtraWorkTotal).FirstOrDefault();
            }

        }

        #endregion
    }
}
