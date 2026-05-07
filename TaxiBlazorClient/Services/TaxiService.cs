using System.Net.Http.Json;
using System.Net.Http.Headers;
using TaxiBlazorClient.Models;

namespace TaxiBlazorClient.Services
{
    public class TaxiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public TaxiService(HttpClient httpClient, ILocalStorageService localStorage)
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

        public async Task<List<Taxi>?> GetMyTaxisAsync()
        {
            await SetAuthHeaderAsync();
            try
            {
                var response = await _httpClient.GetAsync("api/taxis");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Taxi>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    // User doesn't have BOSS role
                    throw new UnauthorizedAccessException("Access denied. BOSS role required.");
                }
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Taxi?> CreateTaxiAsync(TaxiCreateRequest request)
        {
            await SetAuthHeaderAsync();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/taxis", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Taxi>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Taxi?> UpdateTaxiAsync(int id, TaxiUpdateRequest request)
        {
            await SetAuthHeaderAsync();
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/taxis/{id}", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Taxi>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Taxi>?> GetMyAssignedTaxisAsync()
        {
            await SetAuthHeaderAsync();
            try
            {
                var response = await _httpClient.GetAsync("api/taxis/my-assigned");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Taxi>>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}

