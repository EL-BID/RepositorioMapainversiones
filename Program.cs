using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IMRepo.Data;
using IMRepo.Services.basic;
using JaosLib.Services.JaoTables;
using IMRepo.Services.Utilities;
using JaosLib.Services.Utilities;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("ServerTest") ?? ""; // Wind ServerTest    
builder.Services.AddDbContext<IMRepoDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IMRepoDbContext>();

builder.Services.AddControllersWithViews();

//builder.Services.AddSession();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // timeout
    //options.Cookie.HttpOnly = true;
    //options.Cookie.IsEssential = true;
});


// Project Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPipToolsService, PipToolsService>();
builder.Services.AddTransient<IFileLoadService, FileLoadService>();
builder.Services.AddScoped<IJaoTableServices, JaoTableServices>();
builder.Services.AddScoped<IJaoTableExcelServices, JaoTableExcelServices>();
builder.Services.AddScoped<IExcelPOILib, ExcelPOILib>();
builder.Services.AddScoped<INPOIWordService, NPOIWordService>();

builder.Services.AddScoped<IParentProjectService, ParentProjectService>();
builder.Services.AddScoped<ILogTools, LogTools>();


//For Authentication
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0; // change to 1 and all other to true

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@$%+#()!*=&";
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.User.RequireUniqueEmail = true;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
//    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix) no localized view files in use
    .AddDataAnnotationsLocalization();


//=========================================================================
var app = builder.Build();

//-----------------------------------
// Set language and culture
var supportedCultures = new[]
        {
            new CultureInfo("es-AR"),
        };
var options = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es-AR", "es-AR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
// Configure the Localization middleware
app.UseRequestLocalization(options);



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapAreaControllerRoute(
    name: "MyAreaReview",
    areaName: "Review",
    pattern: "Review/{controller=Review}/{action=Index}/{id?}");
app.MapAreaControllerRoute(
    name: "MyAreaMonitor",
    areaName: "Monitor",
    pattern: "Monitor/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute(
    name: "MyAreaPEU",
    areaName: "PEU",
    pattern: "PEU/{controller=PEU}/{action=Index}/{id?}");
app.MapAreaControllerRoute(
    name: "MyAreaAppReports",
    areaName: "AppReports",
    pattern: "AppReports/{controller=Pew}/{action=Index}/{id?}");
app.MapAreaControllerRoute(
    name: "MyAreaUserAdmin",
    areaName: "UserAdmin",
    pattern: "UserAdmin/{controller=Home}/{action=Index}/{id?}");


//app.MapControllerRoute(
//    name: "default",
//	pattern: "{controller=Contract}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


app.Run();
