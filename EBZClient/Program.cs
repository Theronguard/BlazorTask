using EBZClient;
using EBZClient.Scripts;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<IEBZWebApiController,EBZWebApiController>();
builder.Services.AddScoped<IAuth,Auth>();
builder.Services.AddSingleton<IStorage,TemporaryStorage>();

builder.Services.AddMudServices();

var app = builder.Build();
await app.RunAsync();