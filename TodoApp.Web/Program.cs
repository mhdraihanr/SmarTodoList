using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TodoApp.Web;
using TodoApp.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseSetting = builder.Configuration["Api:BaseUrl"];
if (!Uri.TryCreate(apiBaseSetting, UriKind.Absolute, out var apiBaseUri))
{
    apiBaseUri = new Uri("http://localhost:5191");
}

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddScoped(_ => new TodoApiClient(new HttpClient { BaseAddress = apiBaseUri }));

await builder.Build().RunAsync();




