using ConstructionERP_DesktopUI.API;
using ConstructionERP_DesktopUI.Helpers;
using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Project.xaml
    /// </summary>
    public partial class Project : UserControl, INotifyPropertyChanged
    {

        #region Initialization

        private ProjectAPIHelper apiHelper;
        private TeamAPIHelper teamAPIHelper;
        private ProjectStatusAPIHelper statusAPIHelper;
        private ProjectTypeAPIHelper typeAPIHelper;
        private ContractorAPIHelper contractorAPIHelper;
        private SupplierAPIHelper supplierAPIHelper;
        private ProjectContractorsAPIHelper projectContractorsAPIHelper;
        private ProjectSuppliersAPIHelper projectSuppliersAPIHelper;

        public Project(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }


        void SetValues()
        {
            apiHelper = new ProjectAPIHelper();
            teamAPIHelper = new TeamAPIHelper();
            statusAPIHelper = new ProjectStatusAPIHelper();
            typeAPIHelper = new ProjectTypeAPIHelper();
            contractorAPIHelper = new ContractorAPIHelper();
            supplierAPIHelper = new SupplierAPIHelper();
            projectContractorsAPIHelper = new ProjectContractorsAPIHelper();
            projectSuppliersAPIHelper = new ProjectSuppliersAPIHelper();

            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetProjects())();
            new Action(async () => await GetTypes())();
            new Action(async () => await GetStatuses())();
            new Action(async () => await GetTeams())();
            new Action(async () => await GetContractors())();
            new Action(async () => await GetSuppliers())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateProject()); }, () => CanSaveProject);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteProject()); }, () => CanDeleteProject);
            CheckContractorCommand = new RelayCommand(SetContractorsCheckedText);
            CheckSupplierCommand = new RelayCommand(SetSuppliersCheckedText);
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

        //Selected Project
        private ProjectModel selectedProject;

        public ProjectModel SelectedProject
        {
            get { return selectedProject; }
            set
            {
                selectedProject = value;
                OnPropertyChanged("SelectedProject");
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


        //Title
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



        //Statuses
        private ObservableCollection<StatusModel> statuses;

        public ObservableCollection<StatusModel> Statuses
        {
            get { return statuses; }
            set
            {
                statuses = value;
                OnPropertyChanged("Statuses");
            }
        }

        private StatusModel selectedStatus;

        public StatusModel SelectedStatus
        {
            get { return selectedStatus; }
            set
            {
                selectedStatus = value;
                OnPropertyChanged("SelectedStatus");
            }
        }

        private string statusText;

        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                OnPropertyChanged("StatusText");
            }
        }


        //Teams
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

        private string typeText;

        public string TypeText
        {
            get { return typeText; }
            set
            {
                typeText = value;
                OnPropertyChanged("TypeText");
            }
        }


        private ObservableCollection<TeamModel> teams;

        public ObservableCollection<TeamModel> Teams
        {
            get { return teams; }
            set
            {
                teams = value;
                OnPropertyChanged("Teams");
            }
        }

        private TeamModel team;

        public TeamModel Team
        {
            get { return team; }
            set
            {
                team = value;
                OnPropertyChanged("Team");
            }
        }


        //Contractors
        private ObservableCollection<ContractorModel> contractors;

        public ObservableCollection<ContractorModel> Contractors
        {
            get { return contractors; }
            set
            {
                contractors = value;
                OnPropertyChanged("Contractors");
            }
        }

        private string contractorsText;

        public string ContractorsText
        {
            get { return contractorsText; }
            set
            {
                contractorsText = value;
                OnPropertyChanged("ContractorsText");
            }
        }

        private ObservableCollection<SupplierModel> suppliers;

        public ObservableCollection<SupplierModel> Suppliers
        {
            get { return suppliers; }
            set
            {
                suppliers = value;
                OnPropertyChanged("Suppliers");
            }
        }

        private string suppliersText;

        public string SuppliersText
        {
            get { return suppliersText; }
            set
            {
                suppliersText = value;
                OnPropertyChanged("SuppliersText");
            }
        }




        //Enquiry Date
        private DateTime startDate = DateTime.Today;
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        //FollowUp Date
        private DateTime dueDate = DateTime.Today;
        public DateTime DueDate
        {
            get { return dueDate; }
            set
            {
                dueDate = value;
                OnPropertyChanged("DueDate");
            }
        }

        private string address;

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }


        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
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

        private async void OpenCloseOperations(object value)
        {
            try
            {
                switch (value.ToString())
                {
                    case "Edit":
                        if (SelectedProject != null)
                        {
                            await GetContractors();
                            await GetSuppliers();
                            ColSpan = 1;
                            OperationsVisibility = "Visible";

                            ID = SelectedProject.ID;
                            Title = SelectedProject.Title;
                            Description = SelectedProject.Description;
                            StartDate = SelectedProject.StartDate;
                            DueDate = SelectedProject.DueDate;
                            Address = SelectedProject.Address;
                            Title = SelectedProject.Title;
                            SelectedType = SelectedProject.Type;
                            SelectedStatus = SelectedProject.Status;
                            Team = SelectedProject.Team;

                            var projectContractors = await projectContractorsAPIHelper.GetProjectContractorsByProjectID(ParentLayout.LoggedInUser.Token, ID);
                            var projectSuppliers = await projectSuppliersAPIHelper.GetProjectSuppliersByProjectID(ParentLayout.LoggedInUser.Token, ID);

                            foreach (var pc in projectContractors)
                            {
                                pc.Contractor.IsChecked = true;
                                Contractors.FirstOrDefault(c => c.ID == pc.Contractor.ID).IsChecked = true;
                            }


                            foreach (var ps in projectSuppliers)
                            {
                                ps.Supplier.IsChecked = true;
                                Suppliers.FirstOrDefault(s => s.ID == ps.Supplier.ID).IsChecked = true;
                            }

                            SetContractorsCheckedText(null);
                            SetSuppliersCheckedText(null);

                            
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
                        await GetSuppliers();
                        await GetContractors();
                        break;
                    default:
                        ColSpan = ColSpan == 1 ? 2 : 1;
                        OperationsVisibility = OperationsVisibility == "Visible" ? "Collapsed" : "Visible";
                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        #endregion

        #region Get Projects

        private ObservableCollection<ProjectModel> projects;

        public ObservableCollection<ProjectModel> Projects
        {
            get { return projects; }
            set
            {
                projects = value;
                OnPropertyChanged("Projects");
            }
        }

        private async Task GetProjects()
        {
            try
            {
                Projects = await apiHelper.GetProjects(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Project Types

        private async Task GetTypes()
        {
            try
            {
                Types = await typeAPIHelper.GetTypes(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Project Statuses

        private async Task GetStatuses()
        {
            try
            {
                Statuses = await statusAPIHelper.GetStatuses(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Teams

        private async Task GetTeams()
        {
            try
            {
                Teams = await teamAPIHelper.GetTeams(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Contractors

        private async Task GetContractors()
        {
            try
            {
                Contractors = await contractorAPIHelper.GetContractors(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get Suppliers

        private async Task GetSuppliers()
        {
            try
            {
                Suppliers = await supplierAPIHelper.GetSuppliers(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Project Command

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

        private bool canSaveProject = true;

        public bool CanSaveProject
        {
            get { return canSaveProject; }
            set
            {
                canSaveProject = value;
                OnPropertyChanged("CreateProject");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveProject;

        public string SaveBtnText => canSaveProject ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveProject ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateProject()
        {
            try
            {
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", Title),
                    new KeyValuePair<string, string>("Project Type", TypeText),
                    new KeyValuePair<string, string>("Project Status", StatusText),
                    new KeyValuePair<string, string>("Address", Address)

                };
                if (FieldValidation.ValidateFields(values))
                {
                    if (Contractors.Where(c => c.IsChecked).Count() <= 0)
                    {
                        MessageBox.Show("Please add atleast 1 Contractor to the Project", "Add Contractor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (Suppliers.Where(c => c.IsChecked).Count() <= 0)
                    {
                        MessageBox.Show("Please add atleast 1 Supplier to the Project", "Add Supplier", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        CanSaveProject = false;

                        List<ProjectContractorsModel> projectContractors = new List<ProjectContractorsModel>();
                        List<ProjectSuppliersModel> projectSuppliers = new List<ProjectSuppliersModel>();

                        Contractors.Where(s => s.IsChecked).ToList().ForEach(s => projectContractors.Add(
                            new ProjectContractorsModel
                            {
                                ProjectID = ID,
                                ContractorID = s.ID,
                            }));

                        Suppliers.Where(s => s.IsChecked).ToList().ForEach(s => projectSuppliers.Add(
                            new ProjectSuppliersModel
                            {
                                ProjectID = ID,
                                SupplierID = s.ID,
                            }));

                        ProjectModel projectData = new ProjectModel()
                        {
                            Title = Title,
                            Description = Description,
                            ProjectTypeID = SelectedType?.ID,
                            Type = SelectedType == null ? new TypeModel { Title = TypeText, CreatedBy = ParentLayout.LoggedInUser.Name } : null,
                            ProjectStatusID = SelectedStatus?.ID,
                            Status = SelectedStatus == null ? new StatusModel { Title = StatusText, CreatedBy = ParentLayout.LoggedInUser.Name } : null,
                            StartDate = StartDate,
                            DueDate = DueDate,
                            Address = Address,
                            TeamID = Team.ID,
                            Contractors = projectContractors,
                            Suppliers = projectSuppliers
                        };

                        HttpResponseMessage result = null;
                        if (isUpdate)
                        {
                            projectData.ID = ID;
                            projectData.CreatedBy = SelectedProject.CreatedBy;
                            projectData.CreatedOn = SelectedProject.CreatedOn;
                            projectData.ModifiedBy = ParentLayout.LoggedInUser.Name;
                            projectData.ModifiedOn = DateTime.Now;
                            result = await apiHelper.PutProject(ParentLayout.LoggedInUser.Token, projectData).ConfigureAwait(false);
                        }
                        else
                        {
                            projectData.CreatedBy = ParentLayout.LoggedInUser.Name;
                            projectData.CreatedOn = DateTime.Now;
                            result = await apiHelper.PostProject(ParentLayout.LoggedInUser.Token, projectData).ConfigureAwait(false);
                        }
                        if (result.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Project Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            await GetProjects();
                            await ParentLayout.GetProjects();
                            IsUpdate = false;
                            ClearFields();
                            await GetContractors();
                            await GetSuppliers();
                            await GetTypes();
                            await GetStatuses();
                        }
                        else
                        {
                            MessageBox.Show("Error in saving Project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        CanSaveProject = true;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CanSaveProject = true;
            }

        }

        private void ClearFields()
        {
            try
            {
                ID = 0;
                Title = string.Empty;
                Description = string.Empty;
                Address = string.Empty;
                StartDate = DateTime.Today;
                DueDate = DateTime.Today;
                SelectedStatus = null;
                SelectedType = null;
                SelectedProject = null;
                StatusText = string.Empty;
                TypeText = string.Empty;
                Team = null;
                ContractorsText = string.Empty;
                SuppliersText = string.Empty;
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Delete Project Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteProject = true;

        public bool CanDeleteProject
        {
            get { return canDeleteProject; }
            set
            {
                canSaveProject = value;
                OnPropertyChanged("DeleteProject");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteProject;

        public string DeleteBtnText => canDeleteProject ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteProject ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteProject()
        {

            if (SelectedProject != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedProject.Title} ?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteProject = false;
                try
                {
                    HttpResponseMessage result = await apiHelper.DeleteProject(ParentLayout.LoggedInUser.Token, SelectedProject.ID).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        await GetProjects();
                        await ParentLayout.GetProjects();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveProject = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteProject = true;
                }
            }
            else
            {
                MessageBox.Show("Please select a Project to be deleted", "Select Project", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

        #region Check Contractor Command


        public ICommand CheckContractorCommand { get; private set; }


        private void SetContractorsCheckedText(object param)
        {
            try
            {
                List<string> checkedContractors = new List<string>();
                Contractors.Where(s => s.IsChecked).Distinct().ToList().ForEach(s => checkedContractors.Add(s.Name));
                ContractorsText = string.Join(", ", checkedContractors);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion

        #region Check Supplier Command

        public ICommand CheckSupplierCommand { get; private set; }

        private void SetSuppliersCheckedText(object param)
        {
            try
            {
                List<string> checkedSuppliers = new List<string>();
                Suppliers.Where(s => s.IsChecked).Distinct().ToList().ForEach(s => checkedSuppliers.Add(s.Name));
                SuppliersText = string.Join(", ", checkedSuppliers);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion
    }
}
