using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Models.Entities;
using EventManagementSystem.Security;
// using EventManagementSystem.Services.Interfaces;
// using EventManagementSystem.Services.Implementations;
// using EventManagementSystem.Repositories.Interfaces;
// using EventManagementSystem.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// External Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
        options.CallbackPath = "/signin-google";
    })
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"] ?? "";
        options.CallbackPath = "/signin-github";
    })
    .AddApple(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Apple:ClientId"] ?? "";
        options.KeyId = builder.Configuration["Authentication:Apple:KeyId"] ?? "";
        options.TeamId = builder.Configuration["Authentication:Apple:TeamId"] ?? "";
        options.CallbackPath = "/signin-apple";
    });

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers(options =>
{
    options.Filters.Add<SanitizeModelFilter>();
});


// Register Repositories
// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<IEventScheduleRepository, EventScheduleRepository>();
// builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();

// // Register Services
// builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<IEventScheduleService, EventScheduleService>();
// builder.Services.AddScoped<ICertificateService, CertificateService>();
// builder.Services.AddScoped<ICertificateGeneratorService, CertificateGeneratorService>();

// MVC
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ISecurityLogger, SecurityLogger>();


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var sanitizingLogger= services.GetRequiredService<ISecurityLogger>();
        SanitizerEngine.Logger = sanitizingLogger;

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}


app.Run();