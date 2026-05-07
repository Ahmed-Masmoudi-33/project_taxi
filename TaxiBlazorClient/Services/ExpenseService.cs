using System.Net.Http.Json;
using System.Net.Http.Headers;
using TaxiBlazorClient.Models;

namespace TaxiBlazorClient.Services
{
    public class ExpenseService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ExpenseService(HttpClient httpClient, ILocalStorageService localStorage)
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

        public async Task<Expense?> AddExpenseAsync(ExpenseCreateRequest request)
        {
            await SetAuthHeaderAsync();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/expenses", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Expense>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Expense>?> GetMyExpensesAsync()
        {
            await SetAuthHeaderAsync();
            try
            {
                var response = await _httpClient.GetAsync("api/expenses");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Expense>>();
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

