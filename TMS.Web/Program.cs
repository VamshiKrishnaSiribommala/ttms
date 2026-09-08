using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TMS.Core.Auth;
using TMS.Core.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Detect and register DatabaseHelper for TMS (Registers)
string tmsConn = builder.Configuration.GetConnectionString("TMSConnection") ?? "";
string twConn = builder.Configuration.GetConnectionString("TrainWorkingConnection") ?? "";

// Auto-fallback for Windows developers with local SQLEXPRESS
if (OperatingSystem.IsWindows() && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")))
{
    try
    {
        using var testConn = new Microsoft.Data.SqlClient.SqlConnection(tmsConn);
        testConn.Open();
    }
    catch
    {
        string? winFallback = builder.Configuration.GetConnectionString("WindowsLocalFallback_TMS");
        if (!string.IsNullOrWhiteSpace(winFallback))
        {
            try
            {
                using var testWin = new Microsoft.Data.SqlClient.SqlConnection(winFallback);
                testWin.Open();
                tmsConn = winFallback;
                twConn = builder.Configuration.GetConnectionString("WindowsLocalFallback_TrainWorking") ?? winFallback;
            }
            catch { }
        }
    }
}

var tmsDbHelper = new DatabaseHelper(tmsConn, "TMS_2024_New");
var twDbHelper = new DatabaseHelper(twConn, "TrainManagementDB");

builder.Services.AddSingleton(tmsDbHelper);
builder.Services.AddKeyedSingleton("TMSDb", tmsDbHelper);
builder.Services.AddKeyedSingleton("TrainWorkingDb", twDbHelper);
builder.Services.AddSingleton<AuthService>(sp => new AuthService(tmsDbHelper));

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
