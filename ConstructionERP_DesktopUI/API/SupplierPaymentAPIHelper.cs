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
    public class SupplierPaymentAPIHelper
    {
        #region Initialization

        private HttpClient httpClient;

        public SupplierPaymentAPIHelper()
        {

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["apiUrl"]);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region Post Supplier Payment

        public async Task<HttpResponseMessage> PostSupplierPayment(string token, SupplierPaymentModel paymentData)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/SupplierPayment", paymentData))
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

        #region Get Supplier Payments

        public async Task<ObservableCollection<ContractorPaymentModel>> GetSupplierPayments(long? ContractorID, long? ProjectID, string token)
        {
            try
            {
                var data = new { ContractorID, ProjectID };
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.GetAsync($"/api/ContractorPayment/Transactions/{ContractorID}/{ProjectID}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsAsync<ObservableCollection<ContractorPaymentModel>>();
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

        #region Delete Payment

        public async Task<HttpResponseMessage> DeleteSupplierPayment(string token, long id)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.DeleteAsync($"/api/SupplierPayment/{id}"))
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

        #region Put Contractor

        public async Task<HttpResponseMessage> PutContractorPayment(string token, ContractorPaymentModel paymentData)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.PutAsJsonAsync("/api/ContractorPayment", paymentData))
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
