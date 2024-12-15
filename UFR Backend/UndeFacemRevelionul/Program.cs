using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;
using UndeFacemRevelionul.ContextModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<RevelionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Revelion")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Calea către login dacă utilizatorul nu este autentificat
        options.LogoutPath = "/Account/Logout";  // Calea către logout
        //options.AccessDeniedPath = "/Account/AccessDenied";  // Calea pentru accesul interzis
        options.ExpireTimeSpan = TimeSpan.FromDays(7);  // Setăm timpul de expirare al cookie-ului
        options.SlidingExpiration = true;  // Face ca cookie-ul să se reînnoiască când utilizatorul interacționează cu aplicația
    });
builder.Services.AddHttpContextAccessor(); // Adăugăm IHttpContextAccessor


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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
