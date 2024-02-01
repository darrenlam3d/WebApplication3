using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net;
using WebApplication3.Model;
using WebApplication3.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    // Configure your AuthDbContext options here if needed
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

})
    .AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddDataProtection();

builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.Cookie.IsEssential = true;
    Config.Cookie.SameSite = SameSiteMode.None;
    Config.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    Config.ExpireTimeSpan = TimeSpan.FromSeconds(30);
    Config.LoginPath = "/Login";
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IdleTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<AuditLogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseSession();

app.Use(async (context, next) =>
{
    var session = context.Session;
    if (session.Keys.Contains("LoggedIn") && !session.IsAvailable)
    {
        // Session has expired, redirect to login page
        context.Response.Redirect("/Login");
        return;
    }

    await next();
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

