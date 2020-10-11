using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for TaskType.xaml
    /// </summary>
    public partial class TaskType : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private TaskTypeAPIHelper apiHelper;

        public TaskType(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new TaskTypeAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetTypes())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateType()); }, () => CanSaveType);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteType()); }, () => CanDeleteType);
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

        //Selected Status
        private TypeModel selectedType;

        public TypeModel SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                OnPropertyChanged("SelectedType");
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
                    if (SelectedType != null)
                    {
                        ID = SelectedType.ID;
                        Title = SelectedType.Title;
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

        #region Get Types

        private ObservableCollection<TypeModel> types;

        public ObservableCollection<TypeModel> Types
        {
            get { return types; }
            set
            {
                types = value;
                OnPropertyChanged("Types");
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

        private async Task GetTypes()
        {
            try
            {
                IsProgressing = true;
                Types = await apiHelper.GetTypes(ParentLayout.LoggedInUser.Token);
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

        #region Create and Edit Type Command

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

        private bool canSaveType = true;

        public bool CanSaveType
        {
            get { return canSaveType; }
            set
            {
                canSaveType = value;
                OnPropertyChanged("CreateType");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveType;

        public string SaveBtnText => canSaveType ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveType ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateType()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title)
                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveType = false;
                try
                {
                    TypeModel typeData = new TypeModel()
                    {
                        Title = Title,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        typeData.ID = ID;
                        typeData.CreatedBy = SelectedType.CreatedBy;
                        result = await apiHelper.PutType(ParentLayout.LoggedInUser.Token, typeData).ConfigureAwait(false);
                    }
                    else
                    {
                        typeData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostType(ParentLayout.LoggedInUser.Token, typeData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Type Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetTypes();
                        ClearFields();
                        IsUpdate = false;
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Type", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveType = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveType = true;
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

        #region Delete Type Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteType = true;

        public bool CanDeleteType
        {
            get { return canDeleteType; }
            set
            {
                canSaveType = value;
                OnPropertyChanged("DeleteType");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteType;

        public string DeleteBtnText => canDeleteType ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteType ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteType()
        {

            if (SelectedType != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedType.Title} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteType = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteType(ParentLayout.LoggedInUser.Token, SelectedType.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetTypes();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Type", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveType = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteType = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a Type to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion
    }
}
