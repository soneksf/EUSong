using System;
using System.Linq;
using System.Security.Cryptography;
using EUSong.Data;
using EUSong.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 1) Session & MVC
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts =>
{
    opts.IdleTimeout = TimeSpan.FromMinutes(30);
    opts.Cookie.HttpOnly = true;
    opts.Cookie.IsEssential = true;
});

// 2) Register both API controllers and Razor controllers
builder.Services.AddControllers();            // <— for [ApiController]s
builder.Services.AddControllersWithViews();   // <— for MVC views

// 3) EF DbContext
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 4) Seed SuperAdmin (as before)…
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!ctx.Users.Any(u => u.Role == "SuperAdmin"))
    {
        const string login = "superadmin@example.com";
        const string password = "Super@123";

        var salt = RandomNumberGenerator.GetBytes(16);
        var saltB64 = Convert.ToBase64String(salt);
        var hashB64 = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
              password: password,
              salt: salt,
              prf: KeyDerivationPrf.HMACSHA256,
              iterationCount: 100_000,
              numBytesRequested: 32));

        ctx.Users.Add(new User
        {
            Username = "SuperAdmin",
            Email = login,
            PasswordSalt = saltB64,
            PasswordHash = hashB64,
            Country = "",
            Role = "SuperAdmin",
            IsAdmin = true
        });
        ctx.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

// 5) Map API endpoints first…
app.MapControllers();

// 6) …and then MVC fallback
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
