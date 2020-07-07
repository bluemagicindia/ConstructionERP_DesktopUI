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
    /// Interaction logic for Unit.xaml
    /// </summary>
    public partial class Unit : UserControl, INotifyPropertyChanged
    {
        #region Initialization

        MainLayout mainLayout;
        private LoggedInUser loggedInUser;
        private UnitAPIHelper apiHelper;

        public event PropertyChangedEventHandler PropertyChanged;


        public Unit(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            this.mainLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            loggedInUser = mainLayout.loggedInUser;
            apiHelper = new UnitAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetUnits())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateUnit()); }, () => CanSaveUnit);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteUnit()); }, () => CanDeleteUnit);
            //Task.Run(() => GetUnits());
        }

        #endregion

        #region Properties

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Selected Unit
        private UnitModel selectedUnit;

        public UnitModel SelectedUnit
        {
            get { return selectedUnit; }
            set
            {
                selectedUnit = value;
                OnPropertyChanged("SelectedUnit");
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

        private string operationsVisibility = "Collapsed";

        public string OperationsVisibility
        {
            get { return operationsVisibility; }
            set
            {
                operationsVisibility = value;
                OnPropertyChanged("OperationsVisibility");
            }
        }

        private int colSpan = 2;

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
                    if (SelectedUnit != null)
                    {
                        ID = SelectedUnit.ID;
                        Title = SelectedUnit.Title;
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
                    break;

            }

            ColSpan = ColSpan == 1 ? 2 : 1;
            OperationsVisibility = OperationsVisibility == "Visible" ? "Collapsed" : "Visible";

        }

        #endregion

        #region Get Units

        private ObservableCollection<UnitModel> units;

        public ObservableCollection<UnitModel> Units
        {
            get { return units; }
            set
            {
                units = value;
                OnPropertyChanged("Units");
            }
        }

        private async Task GetUnits()
        {
            try
            {
                Units = await apiHelper.GetUnits(loggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Unit Command

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

        private bool canSaveUnit = true;

        public bool CanSaveUnit
        {
            get { return canSaveUnit; }
            set
            {
                canSaveUnit = value;
                OnPropertyChanged("CreateUnit");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveUnit;

        public string SaveBtnText => canSaveUnit ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveUnit ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateUnit()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title)
                };
            if (FieldValidation.ValidateFields(values))
            {
                CanSaveUnit = false;
                try
                {
                    UnitModel unitData = new UnitModel()
                    {
                        Title = Title,
                    };
                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        unitData.ID = ID;
                        unitData.CreatedBy = SelectedUnit.CreatedBy;
                        result = await apiHelper.PutUnit(loggedInUser.Token, unitData).ConfigureAwait(false);
                    }
                    else
                    {
                        unitData.CreatedBy = loggedInUser.Name;
                        result = await apiHelper.PostUnit(loggedInUser.Token, unitData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Unit Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetUnits();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Unit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveUnit = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanSaveUnit = true;
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

        #region Delete Unit Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteUnit = true;


        public bool CanDeleteUnit
        {
            get { return canDeleteUnit; }
            set
            {
                canSaveUnit = value;
                OnPropertyChanged("DeleteUnit");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteUnit;

        public string DeleteBtnText => canDeleteUnit ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteUnit ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteUnit()
        {

            if (SelectedUnit != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedUnit.Title}'s Unit?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteUnit = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteUnit(loggedInUser.Token, SelectedUnit.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetUnits();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Unit", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveUnit = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteUnit = true;
                }
            }
            else
            {
                MessageBox.Show("Please select an unit to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

    }
}
