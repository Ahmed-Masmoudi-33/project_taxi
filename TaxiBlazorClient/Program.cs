using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaxiBlazorClient;
using TaxiBlazorClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with API base address
// Update this to match your backend API URL
var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "https://localhost:7067/";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

// Register services
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TaxiService>();
builder.Services.AddScoped<RideService>();
builder.Services.AddScoped<CommissionService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<ReportService>();

await builder.Build().RunAsync();
