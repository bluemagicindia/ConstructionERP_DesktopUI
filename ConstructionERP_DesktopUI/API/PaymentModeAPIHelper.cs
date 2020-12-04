using ConstructionERP_DesktopUI.Models;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConstructionERP_DesktopUI.API
{
    public class PaymentModeAPIHelper
    {
        #region Initialization

        private HttpClient httpClient;

        public PaymentModeAPIHelper()
        {

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["apiUrl"]);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region Post Payment Mode

        public async Task<HttpResponseMessage> PostPaymentMode(string token, PaymentModeModel paymentModeData)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/PaymentMode", paymentModeData))
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

        #region Get Payment Modes

        public async Task<ObservableCollection<PaymentModeModel>> GetPaymentModes(string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.GetAsync("/api/PaymentMode"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsAsync<ObservableCollection<PaymentModeModel>>();
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

        #region Delete Payment Mode

        public async Task<HttpResponseMessage> DeletePaymentMode(string token, long id)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.DeleteAsync($"/api/PaymentMode/{id}"))
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

        #region Put Payment Mode

        public async Task<HttpResponseMessage> PutPaymentMode(string token, PaymentModeModel paymentModeData)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                using (HttpResponseMessage response = await httpClient.PutAsJsonAsync("/api/PaymentMode", paymentModeData))
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
