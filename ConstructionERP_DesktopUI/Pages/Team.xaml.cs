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
using System.Linq;
using MahApps.Metro.Controls.Dialogs;

namespace ConstructionERP_DesktopUI.Pages
{
    /// <summary>
    /// Interaction logic for Team.xaml
    /// </summary>
    public partial class Team : UserControl, INotifyPropertyChanged
    {
        #region Initialization

        private TeamAPIHelper apiHelper;
        private TeamSiteManagerLinkageAPIHelper linkageApiHelper;

        public Team(MainLayout mainLayout)
        {
            InitializeComponent();
            DataContext = this;
            ParentLayout = mainLayout;
            SetValues();
        }

        void SetValues()
        {
            SelectedManagers = new ObservableCollection<SiteManagerModel>();
            apiHelper = new TeamAPIHelper();
            linkageApiHelper = new TeamSiteManagerLinkageAPIHelper();
            ToggleOperationCommand = new RelayCommand(OpenCloseOperations);
            new Action(async () => await GetTeams())();
            new Action(async () => await GetSiteManagers())();
            SaveCommand = new RelayCommand(async delegate { await Task.Run(() => CreateTeam()); }, () => CanSaveTeam);
            DeleteCommand = new RelayCommand(async delegate { await Task.Run(() => DeleteTeam()); }, () => CanDeleteTeam);
            AddCommand = new RelayCommand(AddSiteManager);
            RemoveCommand = new RelayCommand(RemoveSiteManager);
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


        //Selected Unit
        private TeamModel selectedTeam;

        public TeamModel SelectedTeam
        {
            get { return selectedTeam; }
            set
            {
                selectedTeam = value;
                OnPropertyChanged("SelectedTeam");
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



        private ObservableCollection<SiteManagerModel> selectedManagers;

        public ObservableCollection<SiteManagerModel> SelectedManagers
        {
            get { return selectedManagers; }
            set
            {
                selectedManagers = value;
                OnPropertyChanged("SelectedManagers");
            }
        }


        private SiteManagerModel selectedManager;

        public SiteManagerModel SelectedManager
        {
            get { return selectedManager; }
            set
            {
                selectedManager = value;
                OnPropertyChanged("SelectedManager");
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

        private async void OpenCloseOperations(object value)
        {
            switch (value.ToString())
            {
                case "Edit":
                    if (SelectedTeam != null)
                    {
                        try
                        {
                            SelectedManagers.Clear();
                            ID = SelectedTeam.ID;
                            Title = SelectedTeam.Name;
                            ColSpan = 1;
                            OperationsVisibility = "Visible";
                            var linkages = await linkageApiHelper.GetLinkagesByTeamID(ParentLayout.LoggedInUser.Token, SelectedTeam.ID);
                            linkages.Distinct().ToList().ForEach(l => SelectedManagers.Add(l.SiteManager));
                            IsUpdate = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Please select a record to edit", "Select Record", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;
                case "Create":
                    ID = 0;
                    IsUpdate = false;
                    ColSpan = 1;
                    OperationsVisibility = "Visible";
                    ClearFields();
                    SelectedManagers.Clear();
                    break;
                default:
                    ColSpan = ColSpan == 1 ? 2 : 1;
                    OperationsVisibility = OperationsVisibility == "Visible" ? "Collapsed" : "Visible";
                    break;

            }



        }

        #endregion

        #region Get Teams

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

        private async Task GetTeams()
        {
            try
            {
                Teams = await apiHelper.GetTeams(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Get SiteManager

        private ObservableCollection<SiteManagerModel> siteManagers;

        public ObservableCollection<SiteManagerModel> SiteManagers
        {
            get { return siteManagers; }
            set
            {
                siteManagers = value;
                OnPropertyChanged("SiteManagers");
            }
        }

        private async Task GetSiteManagers()
        {
            try
            {
                SiteManagers = await new SiteManagerAPIHelper().GetSiteManagers(ParentLayout.LoggedInUser.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        #endregion

        #region Create and Edit Team Command

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

        private bool canSaveTeam = true;

        public bool CanSaveTeam
        {
            get { return canSaveTeam; }
            set
            {
                canSaveTeam = value;
                OnPropertyChanged("CreateTeam");
                OnPropertyChanged("IsSaveSpinning");
                OnPropertyChanged("SaveBtnText");
                OnPropertyChanged("SaveBtnIcon");
            }
        }

        public bool IsSaveSpinning => !canSaveTeam;

        public string SaveBtnText => canSaveTeam ? "Save" : "Saving...";

        public string SaveBtnIcon => canSaveTeam ? "SaveRegular" : "SpinnerSolid";

        private async Task CreateTeam()
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
                {

                    new KeyValuePair<string, string>("Title", Title)
                };
            if (FieldValidation.ValidateFields(values))
            {
                if (SelectedManagers?.Count > 0)
                {
                    CanSaveTeam = false;
                    try
                    {
                        TeamModel teamData = new TeamModel()
                        {
                            Name = Title,
                        };

                        long? teamID = 0;

                        HttpResponseMessage result = null;
                        if (isUpdate)
                        {
                            teamData.ID = ID;
                            teamData.CreatedBy = SelectedTeam.CreatedBy;
                            result = await apiHelper.PutTeam(ParentLayout.LoggedInUser.Token, teamData).ConfigureAwait(false);
                            if (result.IsSuccessStatusCode)
                            {
                                result = await linkageApiHelper.DeleteTeamLinkagesByID(ParentLayout.LoggedInUser.Token, teamData.ID);
                                if (result.IsSuccessStatusCode)
                                    teamID = teamData.ID;
                            }
                        }
                        else
                        {
                            teamData.CreatedBy = ParentLayout.LoggedInUser.Name;
                            teamID = await apiHelper.PostTeam(ParentLayout.LoggedInUser.Token, teamData).ConfigureAwait(false);
                        }
                        if (teamID != null && teamID != 0)
                        {

                            List<TeamSiteManagerLinkageModel> teamLinkages = new List<TeamSiteManagerLinkageModel>();

                            foreach (var siteManager in SelectedManagers)
                            {
                                teamLinkages.Add(new TeamSiteManagerLinkageModel { TeamID = teamID, SiteManagerID = siteManager.ID, CreatedBy = ParentLayout.LoggedInUser.Name });
                            }

                            result = await linkageApiHelper.PostTeamLinkage(ParentLayout.LoggedInUser.Token, teamLinkages);
                            if (result.IsSuccessStatusCode)
                            {
                                MessageBox.Show($"Team Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                await GetTeams();
                                ClearFields();
                                teamID = 0;
                                IsUpdate = false;
                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    SelectedManagers.Clear();
                                });
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error in saving Team", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        CanSaveTeam = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        CanSaveTeam = true;
                    }
                }
                else
                {
                    MessageBox.Show("Please add atleast 1 SiteManager to the Team", "Add Managers", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        #region Delete Team Command

        public ICommand DeleteCommand { get; private set; }

        private bool canDeleteTeam = true;


        public bool CanDeleteTeam
        {
            get { return canDeleteTeam; }
            set
            {
                canSaveTeam = value;
                OnPropertyChanged("DeleteTeam");
                OnPropertyChanged("IsDeleteSpinning");
                OnPropertyChanged("DeleteBtnText");
                OnPropertyChanged("DeleteBtnIcon");
            }
        }

        public bool IsDeleteSpinning => !canDeleteTeam;

        public string DeleteBtnText => canDeleteTeam ? "Delete" : "Deleting...";

        public string DeleteBtnIcon => canDeleteTeam ? "TrashAltRegular" : "SpinnerSolid";

        private async Task DeleteTeam()
        {

            if (SelectedTeam != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete {SelectedTeam.Name}'s Team?", "Delete Record", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;
                CanDeleteTeam = false;
                try
                {
                    HttpResponseMessage result = await linkageApiHelper.DeleteTeamLinkagesByID(ParentLayout.LoggedInUser.Token, SelectedTeam.ID);
                    if (result.IsSuccessStatusCode)
                    {
                        result = await apiHelper.DeleteTeam(ParentLayout.LoggedInUser.Token, SelectedTeam.ID).ConfigureAwait(false);
                        if (result.IsSuccessStatusCode)
                        {
                            await GetTeams();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error in deleting Team", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CanSaveTeam = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CanDeleteTeam = true;
                }
            }
            else
            {
                MessageBox.Show("Please select an team to be deleted", "Select Team", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

        #region Add SiteManager Command

        public ICommand AddCommand { get; private set; }

        private void AddSiteManager(object parameter)
        {

            if (SelectedManager != null)
            {
                if (SelectedManagers.Where(s => s.ID == SelectedManager.ID)?.Count() > 0)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        ParentLayout.ShowMessageAsync("Record Existing", "Site Managers already exists in the selected list", MessageDialogStyle.Affirmative, new MetroDialogSettings() { ColorScheme = MetroDialogColorScheme.Accented });
                    });

                }
                else
                {
                    try
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            SelectedManagers.Add(SelectedManager);
                            //SiteManagers.Remove(SelectedManager);
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
            }
            else
            {
                MessageBox.Show("Please select a manager to be added", "Select Site Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion

        #region Remove SiteManager Command


        public ICommand RemoveCommand { get; private set; }


        private void RemoveSiteManager(object parameter)
        {
            if (parameter != null)
            {
                try
                {
                    var deletingManager = parameter as SiteManagerModel;
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        SelectedManagers.Remove(deletingManager);
                        //SiteManagers.Add(deletingManager);
                    });

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a manager to be deleted", "Select Site Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        #endregion
    }
}
