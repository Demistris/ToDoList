using Blazorise;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ToDoListProject;
using ToDoListProject.Pages;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using ToDoListProject.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ToDoListProject.Provider;
using ToDoListProject.Handlers;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazorise(options =>
{
    options.Immediate = true;
})
.AddBootstrapProviders()
.AddFontAwesomeIcons();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddHttpClient("API", client => client.BaseAddress = new Uri("https://localhost:7291/"))
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7291/") });
builder.Services.AddScoped<ToDoListComponent>();
builder.Services.AddScoped<NavigationService>();
builder.Services.AddScoped<ToDoService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();