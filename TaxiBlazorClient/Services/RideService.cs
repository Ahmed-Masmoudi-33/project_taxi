using System.Net.Http.Json;
using System.Net.Http.Headers;
using TaxiBlazorClient.Models;

namespace TaxiBlazorClient.Services
{
    public class RideService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public RideService(HttpClient httpClient, ILocalStorageService localStorage)
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

        public async Task<Ride?> AddRideAsync(RideCreateRequest request)
        {
            await SetAuthHeaderAsync();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/rides", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Ride>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed to add ride: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding ride: {ex.Message}", ex);
            }
        }

        public async Task<List<Ride>?> GetMyRidesAsync(int? taxiId = null)
        {
            await SetAuthHeaderAsync();
            try
            {
                string url = "api/rides";
                if (taxiId.HasValue)
                {
                    url += $"?taxiId={taxiId.Value}";
                }
                
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Ride>>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed to get rides: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting rides: {ex.Message}", ex);
            }
        }
    }
}

