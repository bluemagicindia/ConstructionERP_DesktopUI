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
    /// Interaction logic for Document.xaml
    /// </summary>
    public partial class Document : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private DocumentAPIHelper apiHelper;

        public Document(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new DocumentAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetDocuments())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateDocument()); }, () => CanSaveDocument);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteDocument()); }, () => CanDeleteDocument);
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


        //Selected Document
        private DocumentModel selectedDocument;

        public DocumentModel SelectedDocument
        {
            get { return selectedDocument; }
            set
            {
                selectedDocument = value;
                OnPropertyChanged("SelectedDocument");
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
                    if (SelectedDocument != null)
                    {
                        ID = SelectedDocument.ID;
                        Title = SelectedDocument.Title;
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

        #region Get Documents

        private ObservableCollection<DocumentModel> documents;

        public ObservableCollection<DocumentModel> Documents
        {
            get { return documents; }
            set
            {
                documents = value;
                OnPropertyChanged("Documents");
            }
        }

        private async Task GetDocuments()
        {
            try
            {
                Documents = await apiHelper.GetDocuments(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Document Command

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

        private bool canSaveDocument = true;

        public bool CanSaveDocument
        {
            get { return canSaveDocument; }
            set
            {
                canSaveDocument = value;
                OnPropertyChanged("CreateDocument");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveDocument;

        public string SaveBtnText => canSaveDocument ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveDocument ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateDocument()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title),
                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveDocument = false;
                try
                {
                    string UploadPath = "";
                    DocumentModel documentData = new DocumentModel()
                    {
                        Title = Title,
                        ProjectID = ParentLayout.SelectedProject.ID,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        documentData.ID = ID;
                        documentData.CreatedBy = SelectedDocument.CreatedBy;
                        documentData.CreatedOn = SelectedDocument.CreatedOn;
                        documentData.DocUrl = SelectedDocument.DocUrl;
                        if (!string.IsNullOrEmpty(FilePath))
                        {
                            UploadPath = $"{ConfigurationManager.AppSettings["FTPUrl"]}/Documents/{Guid.NewGuid()}.{FilePath.Substring(FilePath.IndexOf(".") + 1, FilePath.Length - FilePath.IndexOf(".") - 1)}";
                            FTPHelper.UploadFile(FilePath, UploadPath);
                            documentData.DocUrl = UploadPath;
                        }
                        result = await apiHelper.PutDocument(ParentLayout.LoggedInUser.Token, documentData).ConfigureAwait(false);
                    }
                    else
                    {

                        if (string.IsNullOrEmpty(FilePath))
                        {
                            MessageBox.Show("Please select a document to upload", "Select Document", MessageBoxButton.OK, MessageBoxImage.Warning);
                            CanSaveDocument = true;
                            return;
                        }
                        else
                        {
                            UploadPath = $"{ConfigurationManager.AppSettings["FTPUrl"]}/Documents/{Guid.NewGuid()}.{FilePath.Substring(FilePath.IndexOf(".") + 1, FilePath.Length - FilePath.IndexOf(".") - 1)}";
                            FTPHelper.UploadFile(FilePath, UploadPath);

                        }
                        documentData.DocUrl = UploadPath;
                        documentData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostDocument(ParentLayout.LoggedInUser.Token, documentData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Document Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetDocuments();
                        ClearFields();

                        IsUpdate = false;
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Document", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveDocument = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveDocument = true;
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

        #region Delete Document Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteDocument = true;


        public bool CanDeleteDocument
        {
            get { return canDeleteDocument; }
            set
            {
                canSaveDocument = value;
                OnPropertyChanged("DeleteDocument");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteDocument;

        public string DeleteBtnText => canDeleteDocument ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteDocument ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteDocument()
        {

            if (SelectedDocument != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete '{SelectedDocument.Title}' Document?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteDocument = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteDocument(ParentLayout.LoggedInUser.Token, SelectedDocument.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetDocuments();
                        FTPHelper.DeletFile(SelectedDocument.DocUrl);
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Document", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveDocument = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteDocument = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a document to be deleted", "Select Document", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    var downloadingDocument = parameter as DocumentModel;
                    if (downloadingDocument.DocUrl != null)
                    {
                        FTPHelper.DownloadFile(downloadingDocument.DocUrl);
                        MessageBox.Show($"Document has been downloaded to '{Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Downloads")}'", "Download Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Invalid FTP Path for the document download", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Error);
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
