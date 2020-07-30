using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConstructionERP_DesktopUI.API
{
    public class ProjectSuppliersAPIHelper
    {
        #region Initialization

        private HttpClient httpClient;

        public ProjectSuppliersAPIHelper()
        {

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["apiUrl"]);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region Get Project Suppliers  

        public async Task<ObservableCollection<ProjectSuppliersModel>> GetProjectSuppliersByProjectID(string token, long projectID)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.GetAsync($"/api/ProjectSuppliers/ByProjectID/{projectID}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsAsync<ObservableCollection<ProjectSuppliersModel>>();
                        return result;
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #endregion

    }
}
