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
    public partial class ContractorPaymentPopup : Window, INotifyPropertyChanged
    {

        #region Initialization

        private ContractorPaymentAPIHelper apiHelper;

        public ContractorPaymentPopup(MainLayout parentLayout, ContractorModel contractor)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = parentLayout;
            Contractor = contractor;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new ContractorPaymentAPIHelper();
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


        private ContractorModel contractor;

        public ContractorModel Contractor
        {
            get { return contractor; }
            set
            {
                contractor = value;
                OnPropertyChanged("Contractor");
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


        private DateTime amountDate = DateTime.Today;
        public DateTime AmountDate
        {
            get { return amountDate; }
            set
            {
                amountDate = value;
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

        private bool isTentativeAmount;

        public bool IsTentativeAmount
        {
            get { return isTentativeAmount; }
            set
            {
                isTentativeAmount = value;
                OnPropertyChanged("IsTentativeAmount");
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

        #region Get Transactions

        private ObservableCollection<ContractorPaymentModel> contractorTransactions;

        public ObservableCollection<ContractorPaymentModel> ContractorTransactions
        {
            get { return contractorTransactions; }
            set
            {
                contractorTransactions = value;
                OnPropertyChanged("ContractorTransactions");
            }
        }

        private ContractorPaymentModel selectedTransaction;

        public ContractorPaymentModel SelectedTransaction
        {
            get { return selectedTransaction; }
            set
            {
                selectedTransaction = value;
                OnPropertyChanged("SelectedTransaction");
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

        private async Task GetTransactions()
        {
            long cumulative = 0;
            try
            {
                IsProgressing = true;
                ContractorTransactions = await apiHelper.GetContractorPayments(Contractor.ID, ParentLayout.SelectedProject.ID, ParentLayout.LoggedInUser.Token);
                await Task.Run(() =>
                {
                    ContractorTransactions.ToList().ForEach(ct => ct.Cumulative = cumulative = cumulative + ct.TentativeAmount - ct.PaidAmount);
                });
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

        #region Create Transaction

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
                if (!(IsPayment || IsTentativeAmount))
                {
                    MessageBox.Show("Please select either a Tentative Amount or Payment Option", "Option Required", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (Amount <= 0)
                {
                    MessageBox.Show("Please enter an amount", "Amount Required", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    CanPay = false;

                    ContractorPaymentModel paymentData = new ContractorPaymentModel
                    {
                        ContractorID = Contractor.ID,
                        ProjectID = ParentLayout.SelectedProject.ID,
                        PaidAmount = IsPayment ? Amount : 0,
                        TentativeAmount = isTentativeAmount ? Amount : 0,
                        PaymentDate = AmountDate,
                        Remarks = Remarks,
                        Status = false,
                        CreatedOn = DateTime.Now,
                        CreatedBy = ParentLayout.LoggedInUser.Name
                    };


                    HttpResponseMessage result = await apiHelper.PostContractorPayment(ParentLayout.LoggedInUser.Token, paymentData).ConfigureAwait(false);

                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"TentativeAmount / Payment Done Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ClearFields()
        {
            try
            {
                Amount = 0;
                AmountDate = DateTime.Today;
                Remarks = string.Empty;
                IsPayment = false;
                IsTentativeAmount = false;
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
                    HttpResponseMessage result = await apiHelper.DeleteContractorPayment(ParentLayout.LoggedInUser.Token, SelectedTransaction.ID).ConfigureAwait(false);
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
    }
}
