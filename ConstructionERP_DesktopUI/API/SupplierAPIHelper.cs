using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConstructionERP_DesktopUI.API
{
    class SupplierAPIHelper
    {
        #region Initialization

        private HttpClient httpClient;

        public SupplierAPIHelper()
        {

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["apiUrl"]);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region Post Supplier

        public async Task<HttpResponseMessage> PostSupplier(string token, SupplierModel supplierData)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/Supplier", supplierData))
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #endregion

        #region Get Supplier

        public async Task<ObservableCollection<SupplierModel>> GetSuppliers(string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.GetAsync("/api/Supplier"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsAsync<ObservableCollection<SupplierModel>>();
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

        #region Delete Supplier

        public async Task<HttpResponseMessage> DeleteSupplier(string token, long id)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Supplier/{id}"))
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #endregion

        #region Put Supplier

        public async Task<HttpResponseMessage> PutSupplier(string token, SupplierModel supplierData)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.PutAsJsonAsync("/api/Supplier", supplierData))
                {
                    return response;
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
