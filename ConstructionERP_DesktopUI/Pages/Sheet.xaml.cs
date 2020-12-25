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
    /// Interaction logic for Sheet.xaml
    /// </summary>
    public partial class Sheet : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private SheetAPIHelper apiHelper;
        private ActivityLogAPIHelper logAPIHelper;
        public Sheet(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new SheetAPIHelper();
            logAPIHelper = new ActivityLogAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetSheets(null))();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateSheet()); }, () => CanSaveSheet);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteSheet()); }, () => CanDeleteSheet);
            SelectFileCommand = new RelayCommand(SelectFile);
            DownloadCommand = new RelayCommand(DownloadFile);
            SearchCommand = new RelayCommand(SearchSheet);
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


        //Selected Sheet
        private SheetModel selectedSheet;

        public SheetModel SelectedSheet
        {
            get { return selectedSheet; }
            set
            {
                selectedSheet = value;
                OnPropertyChanged("SelectedSheet");
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

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged("SearchText");
                SearchCommand.Execute(null);
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
                    if (SelectedSheet != null)
                    {
                        ID = SelectedSheet.ID;
                        Title = SelectedSheet.Title;
                        //FilePath = SelectedSheet.DocUrl;
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

        #region Get Sheets

        private ObservableCollection<SheetModel> sheets;

        public ObservableCollection<SheetModel> Sheets
        {
            get { return sheets; }
            set
            {
                sheets = value;
                OnPropertyChanged("Sheets");
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
        private async Task GetSheets(string searchText)
        {
            try
            {
                IsProgressing = true;
                Sheets = string.IsNullOrWhiteSpace(searchText) ? await apiHelper.GetSheets(ParentLayout.LoggedInUser.Token) : await apiHelper.GetSheets(ParentLayout.LoggedInUser.Token, searchText);
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

        #region Create and Edit Sheet Command

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

        private bool canSaveSheet = true;

        public bool CanSaveSheet
        {
            get { return canSaveSheet; }
            set
            {
                canSaveSheet = value;
                OnPropertyChanged("CreateSheet");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveSheet;

        public string SaveBtnText => canSaveSheet ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveSheet ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateSheet()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title),
                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveSheet = false;
                try
                {
                    string UploadPath = "";
                    SheetModel sheetData = new SheetModel()
                    {
                        Title = Title,
                        ProjectID = ParentLayout.SelectedProject.ID,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        sheetData.ID = ID;
                        sheetData.CreatedBy = SelectedSheet.CreatedBy;
                        sheetData.CreatedOn = SelectedSheet.CreatedOn;
                        sheetData.DocUrl = SelectedSheet.DocUrl;
                        if (!string.IsNullOrEmpty(FilePath))
                        {
                            UploadPath = $"{ConfigurationManager.AppSettings["FTPUrl"]}/Sheets/{Guid.NewGuid()}.{FilePath.Substring(FilePath.IndexOf(".") + 1, FilePath.Length - FilePath.IndexOf(".") - 1)}";
                            FTPHelper.UploadFile(FilePath, UploadPath);
                            sheetData.DocUrl = UploadPath;
                        }
                        result = await apiHelper.PutSheet(ParentLayout.LoggedInUser.Token, sheetData).ConfigureAwait(false);
                    }
                    else
                    {

                        if (string.IsNullOrEmpty(FilePath))
                        {
                            MessageBox.Show("Please select a sheet to upload", "Select Sheet", MessageBoxButton.OK, MessageBoxImage.Warning);
                            CanSaveSheet = true;
                            return;
                        }
                        else
                        {
                            UploadPath = $"{ConfigurationManager.AppSettings["FTPUrl"]}/Sheets/{Guid.NewGuid()}.{FilePath.Substring(FilePath.IndexOf(".") + 1, FilePath.Length - FilePath.IndexOf(".") - 1)}";
                            FTPHelper.UploadFile(FilePath, UploadPath);

                        }
                        sheetData.DocUrl = UploadPath;
                        sheetData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostSheet(ParentLayout.LoggedInUser.Token, sheetData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Sheet Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetSheets(null);

                        #region Log Data

                        ActivityLogModel logData = new ActivityLogModel()
                        {
                            Type = "Sheet",
                            Description = $"Sheet '{sheetData.Title}' created by '{ParentLayout.LoggedInUser.Name}'",
                            ProjectID = ParentLayout.SelectedProject.ID,
                            CreatedBy = ParentLayout.LoggedInUser.Name,
                            CreatedOn = DateTime.Now
                        };
                        if (isUpdate)
                        {
                            logData.Description = $"Sheet '{sheetData.Title}' updated by '{ParentLayout.LoggedInUser.Name}'";
                        }
                        await logAPIHelper.PostActivityLog(ParentLayout.LoggedInUser.Token, logData);

                        #endregion

                        ClearFields();

                        IsUpdate = false;
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Sheet", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveSheet = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveSheet = true;
                }

            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                Title = "";
                FilePath = "";
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete Sheet Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteSheet = true;


        public bool CanDeleteSheet
        {
            get { return canDeleteSheet; }
            set
            {
                canSaveSheet = value;
                OnPropertyChanged("DeleteSheet");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteSheet;

        public string DeleteBtnText => canDeleteSheet ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteSheet ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteSheet()
        {

            if (SelectedSheet != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete '{SelectedSheet.Title}' Sheet?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteSheet = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteSheet(ParentLayout.LoggedInUser.Token, SelectedSheet.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {

                        #region Log Data

                        ActivityLogModel logData = new ActivityLogModel()
                        {
                            Type = "Sheet",
                            Description = $"Sheet '{SelectedSheet.Title}' deleted by '{ParentLayout.LoggedInUser.Name}'",
                            ProjectID = ParentLayout.SelectedProject.ID,
                            CreatedBy = ParentLayout.LoggedInUser.Name,
                            CreatedOn = DateTime.Now
                        };

                        await logAPIHelper.PostActivityLog(ParentLayout.LoggedInUser.Token, logData);

                        #endregion

                        if (!string.IsNullOrEmpty(SelectedSheet.DocUrl))
                        {
                            FTPHelper.DeletFile(SelectedSheet.DocUrl);
                        }

                        await GetSheets(null);
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Sheet", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveSheet = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteSheet = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a sheet to be deleted", "Select Sheet", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    OpenFileDialog fileDialog = new OpenFileDialog
                    {
                        Filter = "Excel Sheets (*.xlsx)|*.xlsx|Excel Old(*.xls)|*xls"
                    };
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
                    var downloadingSheet = parameter as SheetModel;
                    if (downloadingSheet.DocUrl != null)
                    {
                        FTPHelper.DownloadFile(downloadingSheet.DocUrl);
                        MessageBox.Show($"Sheet has been downloaded to '{Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Downloads")}'", "Download Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Invalid FTP Path for the sheet download", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Error);
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

        #region Sheet Search Command

        public ICommand SearchCommand { get; private set; }

        private async void SearchSheet(object param)
        {
            try
            {
                await GetSheets(SearchText);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion
    }
}
