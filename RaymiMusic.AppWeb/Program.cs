using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Consumer;
using RaymiMusic.Api.Data;
using RaymiMusic.AppWeb.Services;
using RaymiMusic.Modelos;          // ← tu proyecto de entidades

var builder = WebApplication.CreateBuilder(args);
Crud<Follow>.EndPoint = builder.Configuration.GetConnectionString("RaymiMusicDb")??"";

builder.Services.AddHttpClient<IFollowService, FollowService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
});
/* ---------- Services ---------- */

// MVC
builder.Services.AddControllersWithViews();

// EF Core – SQL Server
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("RaymiMusicDb")));

builder.Services.AddHttpClient<IArtistService, ArtistService>(client =>
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]));

builder.Services.AddHttpClient<ISongService, SongService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
});
// Autenticación por cookies
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Account/Login";
        opt.LogoutPath = "/Account/Logout";
        // opt.ExpireTimeSpan = TimeSpan.FromHours(2); // ← opcional
    });

builder.Services.AddAuthorization();

var app = builder.Build();

/* ---------- Middleware ---------- */

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();                       // 30 días por defecto
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();                 // ⚠️ primero autenticación
app.UseAuthorization();                  // luego autorización

/* ---------- Endpoints ---------- */

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
