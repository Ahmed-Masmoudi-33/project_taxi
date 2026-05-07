using System.Net.Http.Json;
using System.Net.Http.Headers;
using TaxiBlazorClient.Models;

namespace TaxiBlazorClient.Services
{
    public class CommissionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public CommissionService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        private async Task SetAuthHeaderAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            // Clear any existing authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<decimal?> GetCommissionAsync()
        {
            await SetAuthHeaderAsync();
            try
            {
                return await _httpClient.GetFromJsonAsync<decimal>("api/commission");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SetCommissionAsync(CommissionRequest request)
        {
            await SetAuthHeaderAsync();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/commission", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}

