using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for TentativePaymentPopup.xaml
    /// </summary>
    public partial class TentativePaymentPopup : Window, INotifyPropertyChanged
    {

        #region Initialization

        //private ProjectAPIHelper apiHelper;

        public TentativePaymentPopup(ProjectModel project, ContractorModel contractor)
        {
            InitializeComponent();
            DataContext = this;
            Project = project;
            Contractor = contractor;
            SetValues();
        }

        void SetValues()
        {
            //apiHelper = new ProjectAPIHelper();

            TentativePaymentCommand = new RelayCommand(async delegate { await Task.Run(() => CreateTentativePayment()); }, () => CanPay);
            ClosePopupCommand = new RelayCommand(ClosePopup);
        }

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ProjectModel project;

        public ProjectModel Project
        {
            get { return project; }
            set
            {
                project = value;
                OnPropertyChanged("Project");
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



        private decimal amount;

        public decimal Amount
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


        #endregion


        #region Create Tentative Payment

        public ICommand TentativePaymentCommand { get; private set; }

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

        private async Task CreateTentativePayment()
        {
            try
            {
                if (Amount <= 0)
                {
                    MessageBox.Show("Please enter a tentative amount", "Amount Required", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    CanPay = false;

                    //ProjectModel projectData = new ProjectModel()
                    //{
                    //    Title = Title_,
                    //    ProjectTypeID = SelectedType?.ID,
                    //    Type = SelectedType == null ? new TypeModel { Title = TypeText, CreatedBy = ProjectSelection.LoggedInUser.Name } : null,
                    //    ProjectStatusID = SelectedStatus?.ID,
                    //    Status = SelectedStatus == null ? new StatusModel { Title = StatusText, CreatedBy = ProjectSelection.LoggedInUser.Name } : null,
                    //    StartDate = StartDate,
                    //    DueDate = DueDate,
                    //    Address = Address,
                    //    CreatedBy = ProjectSelection.LoggedInUser.Name,
                    //    CreatedOn = DateTime.Now
                    //};

                    //HttpResponseMessage result = await apiHelper.PostProject(ProjectSelection.LoggedInUser.Token, projectData).ConfigureAwait(false);

                    //if (result.IsSuccessStatusCode)
                    //{
                    //    MessageBox.Show($"Project Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    //    Application.Current.Dispatcher.Invoke((Action)delegate
                    //    {

                    //        new Action(async () => await ProjectSelection.GetProjects(null))();
                    //        ClosePopupCommand.Execute(null);
                    //    });


                    //}
                    //else
                    //{
                    //    MessageBox.Show("Error in saving Project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    //}
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
            }
            catch (Exception)
            {

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
