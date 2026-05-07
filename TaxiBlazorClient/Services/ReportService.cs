using System.Net.Http.Json;
using System.Net.Http.Headers;
using TaxiBlazorClient.Models;

namespace TaxiBlazorClient.Services
{
    public class ReportService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ReportService(HttpClient httpClient, ILocalStorageService localStorage)
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

        public async Task<MonthlyTaxiReport?> GetReportAsync(int taxiId, int year, int month)
        {
            await SetAuthHeaderAsync();
            try
            {
                return await _httpClient.GetFromJsonAsync<MonthlyTaxiReport>($"api/reports/taxi/{taxiId}/summary?year={year}&month={month}");
            }
            catch
            {
                return null;
            }
        }
    }
}

