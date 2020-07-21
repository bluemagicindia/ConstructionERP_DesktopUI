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
    /// Interaction logic for Material.xaml
    /// </summary>
    public partial class Material : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private MaterialAPIHelper apiHelper;

        public Material(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            apiHelper = new MaterialAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetMaterials())();
            new Action(async () => await GetUnits())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateMaterial()); }, () => CanSaveMaterial);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteMaterial()); }, () => CanDeleteMaterial);
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



        //Selected Material
        private MaterialModel selectedMaterial;

        public MaterialModel SelectedMaterial
        {
            get { return selectedMaterial; }
            set
            {
                selectedMaterial = value;
                OnPropertyChanged("SelectedMaterial");
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

        //Unit
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

        private string unitText;

        public string UnitText
        {
            get { return unitText; }
            set
            {
                unitText = value;
                OnPropertyChanged("UnitText");
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
                    if (SelectedMaterial != null)
                    {
                        ID = SelectedMaterial.ID;
                        Title = SelectedMaterial.Name;
                        SelectedUnit = SelectedMaterial.Unit;

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

        #region Get Materials

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

        private async Task GetMaterials()
        {
            try
            {
                Materials = await apiHelper.GetMaterials(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Units

        private async Task GetUnits()
        {
            try
            {
                Units = await new UnitAPIHelper().GetUnits(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Material Command

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

        private bool canSaveMaterial = true;

        public bool CanSaveMaterial
        {
            get { return canSaveMaterial; }
            set
            {
                canSaveMaterial = value;
                OnPropertyChanged("CreateMaterial");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveMaterial;

        public string SaveBtnText => canSaveMaterial ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveMaterial ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateMaterial()
        {
            try
            {
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Name", Title),
                    new KeyValuePair<string, string>("Unit", UnitText)
                };
                if (FieldValidation.ValidateFields(values))
                {
                    CanSaveMaterial = false;

                    MaterialModel materialData = new MaterialModel()
                    {
                        Name = Title,
                        UnitID = SelectedUnit?.ID,
                        Unit = SelectedUnit == null ? new UnitModel { Title = UnitText, CreatedBy = ParentLayout.LoggedInUser.Name, CreatedOn = DateTime.Now } : null,
                    };

                    HttpResponseMessage result = null;
                    if (isUpdate)
                    {
                        materialData.ID = ID;
                        materialData.CreatedBy = selectedMaterial.CreatedBy;
                        result = await apiHelper.PutMaterial(ParentLayout.LoggedInUser.Token, materialData).ConfigureAwait(false);
                    }
                    else
                    {
                        materialData.CreatedBy = ParentLayout.LoggedInUser.Name;
                        result = await apiHelper.PostMaterial(ParentLayout.LoggedInUser.Token, materialData).ConfigureAwait(false);
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Material Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetMaterials();
                        await GetUnits();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in saving Material", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveMaterial = true;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CanSaveMaterial = true;
            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                Title = "";
                SelectedUnit = null;
                UnitText = "";
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete Material Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteMaterial = true;

        public bool CanDeleteMaterial
        {
            get { return canDeleteMaterial; }
            set
            {
                canSaveMaterial = value;
                OnPropertyChanged("DeleteMaterial");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteMaterial;

        public string DeleteBtnText => canDeleteMaterial ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteMaterial ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteMaterial()
        {

            if (SelectedMaterial != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedMaterial.Name} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteMaterial = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteMaterial(ParentLayout.LoggedInUser.Token, SelectedMaterial.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetMaterials();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Material", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveMaterial = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteMaterial = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a Material to be deleted", "Select Enquiry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion
    }
}
