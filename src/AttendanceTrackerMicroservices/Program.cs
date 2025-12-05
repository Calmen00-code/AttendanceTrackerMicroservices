using AttendanceTrackerMicroservices.Hubs;
using AttendanceTrackerMicroservices.Service;
using AttendanceTrackerMicroservices.Service.IService;
using AttendanceTrackerMicroservices.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// add SignalR
builder.Services.AddSignalR();

// WARNING: This line shall be removed from production
builder.Services.AddHttpClient("AttendanceTrackerAPI")
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

builder.Services.AddHttpClient<IBaseService, BaseService>();

SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.TrackerAPIBase = builder.Configuration["ServiceUrls:TrackerAPI"];
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<ITrackerService, TrackerService>();

// configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR RefreshHub to endpoint "/refreshHub"
app.MapHub<RefreshHub>("/refreshHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
