using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Quotation.xaml
    /// </summary>
    public partial class Quotation : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private QuotationAPIHelper apiHelper;
        private ActivityLogAPIHelper logAPIHelper;

        public Quotation(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }


        void SetValues()
        {
            apiHelper = new QuotationAPIHelper();
            logAPIHelper = new ActivityLogAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetQuotations())();
            new Action(async () => await GetMaterials())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateQuotation()); }, () => CanSaveQuotation);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteQuotation()); }, () => CanDeleteQuotation);
            SelectFileCommand = new RelayCommand(SelectFile);
            DownloadCommand = new RelayCommand(DownloadFile);
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

        //Selected Quotation
        private QuotationModel selectedQuotation;

        public QuotationModel SelectedQuotation
        {
            get { return selectedQuotation; }
            set
            {
                selectedQuotation = value;
                OnPropertyChanged("SelectedQuotation");
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


        //Vendor
        private string vendor;

        public string Vendor
        {
            get { return vendor; }
            set
            {
                vendor = value;
                OnPropertyChanged("Vendor");
            }
        }

        //Material
        private ObservableCollection<MaterialModel> materials;

        public ObservableCollection<MaterialModel> Materials
        {
            get { return materials; }
            set
            {
                materials = value;
                OnPropertyChanged("Materials");
            }
        }

        private MaterialModel material;

        public MaterialModel Material
        {
            get { return material; }
            set
            {
                material = value;
                OnPropertyChanged("Material");
            }
        }

        private long cost;

        public long Cost
        {
            get { return cost; }
            set
            {
                cost = value;
                OnPropertyChanged("Cost");
            }
        }

        private string narration;

        public string Narration
        {
            get { return narration; }
            set
            {
                narration = value;
                OnPropertyChanged("Narration");
            }
        }


        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged("FilePath");
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
                    if (SelectedQuotation != null)
                    {
                        ID = SelectedQuotation.ID;
                        Vendor = SelectedQuotation.Vendor;
                        Material = SelectedQuotation.Material;
                        Cost = SelectedQuotation.Cost;
                        Narration = SelectedQuotation.Narration;

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

        #region Get Quotations

        private ObservableCollection<QuotationModel> quotations;

        public ObservableCollection<QuotationModel> Quotations
        {
            get { return quotations; }
            set
            {
                quotations = value;
                OnPropertyChanged("Quotations");
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

        private async Task GetQuotations()
        {
            try
            {
                IsProgressing = true;
                Quotations = await apiHelper.GetQuotations(ParentLayout.LoggedInUser.Token);
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

        #region Get Materials

        private async Task GetMaterials()
        {
            try
            {
                Materials = await new MaterialAPIHelper().GetMaterials(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Quotation Command

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

        private bool canSaveQuotation = true;

        public bool CanSaveQuotation
        {
            get { return canSaveQuotation; }
            set
            {
                canSaveQuotation = value;
                OnPropertyChanged("CreateQuotation");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveQuotation;

        public string SaveBtnText => canSaveQuotation ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveQuotation ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateQuotation()
        {
            try
            {
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Vendor", Vendor),
                    new KeyValuePair<string, string>("Narration", Narration),
                    new KeyValuePair<string, string>("Cost", Cost.ToString()),
                    new KeyValuePair<string, string>("Material", Material?.Name),

                };
                if (FieldValidation.ValidateFields(values))
                {
                    CanSaveQuotation = false;
                    string UploadPath = "";

                    QuotationModel quotationData = new QuotationModel()
                    {
                        Vendor = Vendor,
                        MaterialID = Material.ID,
                        Cost = Cost,
                        Narration = Narration,
                        CreatedBy = ParentLayout.LoggedInUser.Name,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        quotationData.ID = ID;
                        quotationData.CreatedBy = SelectedQuotation.CreatedBy;
                        quotationData.ModifiedOn = DateTime.Now;
                        quotationData.ModifiedBy = ParentLayout.LoggedInUser.Name;
                        if (!string.IsNullOrEmpty(FilePath))
                        {
                            UploadPath = $"{ConfigurationManager.AppSettings["FTPUrl"]}/Documents/{Guid.NewGuid()}.{FilePath.Substring(FilePath.IndexOf(".") + 1, FilePath.Length - FilePath.IndexOf(".") - 1)}";
                            FTPHelper.UploadFile(FilePath, UploadPath);
                            quotationData.DocUrl = UploadPath;
                        }
                        result = await apiHelper.PutQuotation(ParentLayout.LoggedInUser.Token, quotationData).ConfigureAwait(false);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(FilePath))
                        {
                            UploadPath = $"{ConfigurationManager.AppSettings["FTPUrl"]}/Documents/{Guid.NewGuid()}.{FilePath.Substring(FilePath.IndexOf(".") + 1, FilePath.Length - FilePath.IndexOf(".") - 1)}";
                            FTPHelper.UploadFile(FilePath, UploadPath);
                        }
                        quotationData.DocUrl = UploadPath;
                        quotationData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostQuotation(ParentLayout.LoggedInUser.Token, quotationData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Quotation Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetQuotations();

                        #region Log Data

                        ActivityLogModel logData = new ActivityLogModel()
                        {
                            Type = "Quotation",
                            Description = $"Quotation for '{quotationData.Vendor}' created by '{ParentLayout.LoggedInUser.Name}'",
                            CreatedBy = ParentLayout.LoggedInUser.Name,
                            CreatedOn = DateTime.Now
                        };
                        if (isUpdate)
                        {
                            logData.Description = $"Quotation for '{quotationData.Vendor}' updated by '{ParentLayout.LoggedInUser.Name}'";
                        }
                        await logAPIHelper.PostActivityLog(ParentLayout.LoggedInUser.Token, logData);

                        #endregion

                        IsUpdate = false;
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Quotation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveQuotation = true;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CanSaveQuotation = true;
            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                Vendor = "";
                Cost = 0;
                Narration = "";
                FilePath = "";
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete Quotation Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteQuotation = true;

        public bool CanDeleteQuotation
        {
            get { return canDeleteQuotation; }
            set
            {
                canSaveQuotation = value;
                OnPropertyChanged("DeleteQuotation");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteQuotation;

        public string DeleteBtnText => canDeleteQuotation ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteQuotation ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteQuotation()
        {

            if (SelectedQuotation != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedQuotation.Vendor} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteQuotation = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteQuotation(ParentLayout.LoggedInUser.Token, SelectedQuotation.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        #region Log Data

                        ActivityLogModel logData = new ActivityLogModel()
                        {
                            Type = "Quotation",
                            Description = $"Quotation for '{SelectedQuotation.Vendor}' deleted by '{ParentLayout.LoggedInUser.Name}'",
                            CreatedBy = ParentLayout.LoggedInUser.Name,
                            CreatedOn = DateTime.Now
                        };

                        await logAPIHelper.PostActivityLog(ParentLayout.LoggedInUser.Token, logData);

                        #endregion

                        if (!string.IsNullOrEmpty(SelectedQuotation.DocUrl))
                        {
                            FTPHelper.DeletFile(SelectedQuotation.DocUrl);
                        }

                        await GetQuotations();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Quotation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveQuotation = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteQuotation = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a Quotation to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

        #region Select File Command

        public ICommand SelectFileCommand { get; private set; }

        private void SelectFile(object parameter)
        {

            try
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    //fileDialog.Filter = "Excel Sheets (*.xlsx)|*.xlsx|Excel Old(*.xls)|*xls";
                    if (fileDialog.ShowDialog() == true)
                    {
                        FilePath = fileDialog.FileName;
                    }

                });

            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ParentLayout.ShowMessageAsync("Error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings() { ColorScheme = MetroDialogColorScheme.Accented });
                });
            }



        }

        #endregion

        #region Download File Command

        public ICommand DownloadCommand { get; private set; }

        private void DownloadFile(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    var downloadingDocument = parameter as QuotationModel;
                    if (!string.IsNullOrEmpty(downloadingDocument.DocUrl))
                    {
                        FTPHelper.DownloadFile(downloadingDocument.DocUrl);
                        MessageBox.Show($"Document has been downloaded to '{Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Downloads")}'", "Download Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No file found to download for this Quotation", "No file", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ParentLayout.ShowMessageAsync("Error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings() { ColorScheme = MetroDialogColorScheme.Accented });
                });
            }



        }

        #endregion
    }
}
