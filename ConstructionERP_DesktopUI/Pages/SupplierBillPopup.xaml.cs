using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for TentativePaymentPopup.xaml
    /// </summary>
    public partial class SupplierBillPopup : Window, INotifyPropertyChanged
    {

        #region Initialization

        private SupplierBillAPIHelper apiHelper;
        private SupplierPaymentAPIHelper paymentApiHelper;

        public SupplierBillPopup(MainLayout parentLayout, SupplierModel supplier)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = parentLayout;
            Supplier = supplier;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new SupplierBillAPIHelper();
            paymentApiHelper = new SupplierPaymentAPIHelper();
            new Action(async () => await GetBills())();
            BillCommand = new RelayCommand(async delegate { await Task.Run(() => CreateBill()); }, () => CanBill);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteTransaction()); }, () => CanDeleteTransaction);
            ClosePopupCommand = new RelayCommand(ClosePopup);
            SwitchPayBillCommand = new RelayCommand(SwitchPayBill);

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


        private SupplierModel supplier;

        public SupplierModel Supplier
        {
            get { return supplier; }
            set
            {
                supplier = value;
                OnPropertyChanged("Supplier");
            }
        }

        private string billNo;

        public string BillNo
        {
            get { return billNo; }
            set
            {
                billNo = value;
                OnPropertyChanged("BillNo");
            }
        }


        private long amount;

        public long Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnPropertyChanged("Amount");
            }
        }


        private DateTime billDate = DateTime.Today;
        public DateTime BillDate
        {
            get { return billDate; }
            set
            {
                billDate = value;
                OnPropertyChanged("BillDate");
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

        private bool isBill = true;

        public bool IsBill
        {
            get { return isBill; }
            set
            {
                isBill = value;
                OnPropertyChanged("IsBill");
            }
        }

        private bool isPayment;

        public bool IsPayment
        {
            get { return isPayment; }
            set
            {
                isPayment = value;
                OnPropertyChanged("IsPayment");
            }
        }



        #endregion

        #region Get Bills

        private ObservableCollection<SupplierBillPaymentModel> supplierBills;

        public ObservableCollection<SupplierBillPaymentModel> SupplierBills
        {
            get { return supplierBills; }
            set
            {
                supplierBills = value;
                OnPropertyChanged("SupplierBills");
            }
        }

        private SupplierBillPaymentModel selectedBill;

        public SupplierBillPaymentModel SelectedBill
        {
            get { return selectedBill; }
            set
            {
                selectedBill = value;
                OnPropertyChanged("SelectedBill");
            }
        }


        private async Task GetBills()
        {
            try
            {
                var bills = await apiHelper.GetSupplierBills(supplier.ID, ParentLayout.SelectedProject.ID, ParentLayout.LoggedInUser.Token);
                SupplierBills = new ObservableCollection<SupplierBillPaymentModel>();
                foreach (var bill in bills)
                {
                    long pending = bill.Amount;
                    SupplierBills.Add(new SupplierBillPaymentModel { ID = bill.ID, Date = bill.BillDate, BillNo = bill.BillNo, BillAmount = bill.Amount, IsBill = true, Pending = pending });

                    foreach (var supplierPayment in bill.SupplierPayments)
                    {
                        pending = pending - supplierPayment.Amount;
                        SupplierBills.Add(new SupplierBillPaymentModel { ID = bill.ID, PaymentID = supplierPayment.ID, Date = supplierPayment.PaidOn, PaymentAmount = supplierPayment.Amount, IsBill = false, Pending = pending });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }


        #endregion

        #region Switch Pay Bill

        public ICommand SwitchPayBillCommand { get; private set; }

        public void SwitchPayBill(object value)
        {
            try
            {
                if (SelectedBill?.IsBill == true)
                {
                    if (MessageBox.Show($"Are you sure you want to pay for Bill No '{SelectedBill.BillNo}'", "Pay Bill", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        BillNo = SelectedBill.BillNo;
                        Amount = SelectedBill.BillAmount;
                        IsPayment = true;
                        IsBill = false;
                    }
                }
                else
                {
                    MessageBox.Show("Please select a bill to be paid", "Select Bill", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        #endregion

        #region Create or Pay Bill

        public ICommand BillCommand { get; private set; }

        private bool canBill = true;

        public bool CanBill
        {
            get { return canBill; }
            set
            {
                canBill = value;
                OnPropertyChanged("CreateBill");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canBill;

        public string SaveBtnText => canBill ? "Save" : "Saving...";

        public string SaveBtnIcon => canBill ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateBill()
        {
            try
            {
                if (!(IsPayment || IsBill))
                {
                    MessageBox.Show("Please select either a Bill or Payment Option", "Option Required", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (Amount <= 0)
                {
                    MessageBox.Show("Please enter an amount", "Amount Required", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (BillNo.Trim().Length == 0)
                {
                    MessageBox.Show("Please enter Bill Number", "Bill Number Required", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else
                {
                    CanBill = false;
                    if (IsBill)
                    {
                        SupplierBillModel billData = new SupplierBillModel
                        {
                            SupplierID = Supplier.ID,
                            ProjectID = ParentLayout.SelectedProject.ID,
                            BillNo = BillNo,
                            BillDate = BillDate,
                            Amount = Amount,
                            Remarks = Remarks,
                            Status = true,
                            CreatedOn = DateTime.Now,
                            CreatedBy = ParentLayout.LoggedInUser.Name
                        };


                        HttpResponseMessage result = await apiHelper.PostSupplierBill(ParentLayout.LoggedInUser.Token, billData).ConfigureAwait(false);

                        if (result.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Bill Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            await GetBills();
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Error in adding bill", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (IsPayment)
                    {
                        SupplierPaymentModel paymentData = new SupplierPaymentModel
                        {
                            SupplierBillID = SelectedBill.ID,
                            PaidOn = BillDate,
                            Amount = Amount,
                            Remarks = Remarks,
                            Status = true,
                            CreatedOn = DateTime.Now,
                            CreatedBy = ParentLayout.LoggedInUser.Name
                        };


                        HttpResponseMessage result = await paymentApiHelper.PostSupplierPayment(ParentLayout.LoggedInUser.Token, paymentData).ConfigureAwait(false);

                        if (result.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Payment Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            await GetBills();
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Error in adding bill", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    CanBill = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CanBill = true;
            }
        }

        private void ClearFields()
        {
            try
            {
                BillNo = Remarks = string.Empty;
                Amount = 0;
                BillDate = DateTime.Today;
                IsPayment = false;
                IsBill = true;
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete Transaction Bill / Payment

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

            if (SelectedBill != null)
            {
                try
                {
                    if (SelectedBill.IsBill)
                    {
                        if (MessageBox.Show($"Are you sure you want to delete the bill and its related transactions?", "Delete Bill and Transactions", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            CanDeleteTransaction = false;
                            HttpResponseMessage result = await apiHelper.DeleteSupplierBill(ParentLayout.LoggedInUser.Token, SelectedBill.ID).ConfigureAwait(false);
                            if (result.IsSuccessStatusCode)
                            {
                                await GetBills();
                            }
                            else
                            {
                                MessageBox.Show("Error in deleting bill and its related payments", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            CanDeleteTransaction = true;
                        }
                    }
                    else
                    {
                        if (MessageBox.Show($"Are you sure you want to delete this bill payment?", "Delete Bill Payment", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            CanDeleteTransaction = false;
                            HttpResponseMessage result = await paymentApiHelper.DeleteSupplierPayment(ParentLayout.LoggedInUser.Token, SelectedBill.PaymentID).ConfigureAwait(false);
                            if (result.IsSuccessStatusCode)
                            {
                                await GetBills();
                            }
                            else
                            {
                                MessageBox.Show("Error in deleting bill payment", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            CanDeleteTransaction = true;

                        }
                    }

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
    }
}
