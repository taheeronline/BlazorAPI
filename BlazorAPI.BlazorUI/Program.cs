using Microsoft.AspNetCore.Components.Authorization;
using BlazorAPI.BlazorUI.Components;
using BlazorAPI.BlazorUI.Providers;
using BlazorAPI.BlazorUI.Services.Implementation;
using BlazorAPI.BlazorUI.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// 1. Register the concrete class itself
builder.Services.AddScoped<CustomAuthStateProvider>();

// 2. Tell Blazor's internal systems to use that exact same instance
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

// --- API CLIENT REGISTRATIONS ---

// Optimization: Store the URL once so if your port changes, you only update it here!
var apiBaseAddress = new Uri("http://localhost:5212/");

// Register MovieService
builder.Services.AddHttpClient<IMovieService, MovieService>(client =>
{
    client.BaseAddress = apiBaseAddress;
});

builder.Services.AddHttpClient<IBookService, BookService>(client =>
{
    client.BaseAddress = apiBaseAddress;
});

// Register UserService (NEW)
builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = apiBaseAddress;
});

// --------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();