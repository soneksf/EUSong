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

// 1) Register the in-memory cache which Session uses under the hood
builder.Services.AddDistributedMemoryCache();

// 2) Add and configure Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3) Add controllers with views
builder.Services.AddControllersWithViews();

// 4) Register EF Core DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!ctx.Users.Any(u => u.Role == "SuperAdmin"))
    {
        // Ћог≥н ≥ пароль
        const string superLogin = "superadmin@example.com";
        const string superPassword = "Super@123";

        // «генерувати с≥ль та хеш
        var salt = RandomNumberGenerator.GetBytes(16);
        var saltB64 = Convert.ToBase64String(salt);
        var hashB64 = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(password: superPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32));

        ctx.Users.Add(new User
        {
            Username = "SuperAdmin",
            Email = superLogin,
            PasswordSalt = saltB64,
            PasswordHash = hashB64,
            Country = "",
            Role = "SuperAdmin",
            IsAdmin = true
        });
        ctx.SaveChanges();
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 6) Enable Session before authorization and endpoints
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
