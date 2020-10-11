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
    /// Interaction logic for SalesEnquiry.xaml
    /// </summary>
    public partial class SalesEnquiry : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private SalesEnquiryAPIHelper apiHelper;

        public SalesEnquiry(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();

        }

        void SetValues()
        {
            apiHelper = new SalesEnquiryAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetEnquiries())();
            SaveEnquiryCommand = new RelayCommand(async delegate { await Task.Run(() => CreateSalesEnquiry()); }, () => CanSaveEnquiry);
            DeleteEnquiryCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteSalesEnquiry()); }, () => CanDeleteEnquiry);
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
        private SalesEnquiryModel selectedEnquiry;

        public SalesEnquiryModel SelectedEnquiry
        {
            get { return selectedEnquiry; }
            set
            {
                selectedEnquiry = value;
                OnPropertyChanged("SelectedEnquiry");
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

        //Related To
        private string relatedTo;
        public string RelatedTo
        {
            get { return relatedTo; }
            set
            {
                relatedTo = value;
                OnPropertyChanged("RelatedTo");
            }
        }

        //Enquiry Date
        private DateTime enquiryDate = DateTime.Now;
        public DateTime EnquiryDate
        {
            get { return enquiryDate; }
            set
            {
                enquiryDate = value;
                OnPropertyChanged("EnquiryDate");
            }
        }

        //FollowUp Date
        private DateTime followUpDate = DateTime.Now;
        public DateTime FollowUpDate
        {
            get { return followUpDate; }
            set
            {
                followUpDate = value;
                OnPropertyChanged("FollowUpDate");
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
                    if (selectedEnquiry != null)
                    {
                        ID = SelectedEnquiry.ID;
                        Title = SelectedEnquiry.Name;
                        Phone = SelectedEnquiry.Phone;
                        RelatedTo = SelectedEnquiry.RelatedTo;
                        EnquiryDate = SelectedEnquiry.EnquiryDate;
                        FollowUpDate = SelectedEnquiry.FollowUpDate;
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

        #region Get Sales Eqnuiries

        private ObservableCollection<SalesEnquiryModel> salesEnquiries;

        public ObservableCollection<SalesEnquiryModel> SalesEnquiries
        {
            get { return salesEnquiries; }
            set
            {
                salesEnquiries = value;
                OnPropertyChanged("SalesEnquiries");
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

        private async Task GetEnquiries()
        {
            try
            {
                IsProgressing = true;
                SalesEnquiries = await apiHelper.GetSalesEnquiries(ParentLayout.LoggedInUser.Token);
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

        #region Create and Edit Sales Enquiry Command

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


        public ICommand SaveEnquiryCommand { get; private set; }

        private bool canSaveEnquiry = true;

        public bool CanSaveEnquiry
        {
            get { return canSaveEnquiry; }
            set
            {
                canSaveEnquiry = value;
                OnPropertyChanged("CreateSalesEnquiry");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveEnquiry;

        public string SaveBtnText => canSaveEnquiry ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveEnquiry ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateSalesEnquiry()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Name", Title),
                    new KeyValuePair<string, string>("Phone", Phone),
                    new KeyValuePair<string, string>("RelatedTo", RelatedTo),
                    new KeyValuePair<string, string>("EnquiryDate", EnquiryDate.ToString()),
                    new KeyValuePair<string, string>("FollowUpDate", FollowUpDate.ToString()),

                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveEnquiry = false;
                try
                {
                    SalesEnquiryModel salesEnquiryData = new SalesEnquiryModel()
                    {
                        Name = Title,
                        Phone = Phone,
                        RelatedTo = RelatedTo,
                        EnquiryDate = EnquiryDate,
                        FollowUpDate = FollowUpDate,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        salesEnquiryData.ID = ID;
                        salesEnquiryData.CreatedBy = SelectedEnquiry.CreatedBy;
                        salesEnquiryData.ModifiedOn = DateTime.Now;
                        salesEnquiryData.ModifiedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PutSalesEnquiry(ParentLayout.LoggedInUser.Token, salesEnquiryData).ConfigureAwait(false);
                    }
                    else
                    {
                        salesEnquiryData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostSalesEnquiry(ParentLayout.LoggedInUser.Token, salesEnquiryData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Sales Enquiry Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetEnquiries();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Sales Enquiry", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveEnquiry = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveEnquiry = true;
                }

            }


        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                Title = "";
                Phone = "";
                RelatedTo = "";
                EnquiryDate = DateTime.Today;
                FollowUpDate = DateTime.Today;

            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete Sales Enquiry Command

        public ICommand DeleteEnquiryCommand { get; private set; }

        private bool canDeleteEnquiry = true;

        public bool CanDeleteEnquiry
        {
            get { return canDeleteEnquiry; }
            set
            {
                canSaveEnquiry = value;
                OnPropertyChanged("DeleteSalesEnquiry");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteEnquiry;

        public string DeleteBtnText => canDeleteEnquiry ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteEnquiry ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteSalesEnquiry()
        {

            if (SelectedEnquiry != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {selectedEnquiry.Name}'s Enquiry?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteEnquiry = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteSalesEnquiry(ParentLayout.LoggedInUser.Token, SelectedEnquiry.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetEnquiries();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Sales Enquiry", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveEnquiry = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteEnquiry = true;
                }
            }
            else
            {
                MessageBox.Show("Please select an enquiry to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion
    }
}
