using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using EUSong.Data;
using EUSong.Models;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace EUSong.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (_context.Users.Any(u => u.Email == vm.Email))
            {
                ModelState.AddModelError(nameof(vm.Email), "Email is already taken");
                return View(vm);
            }

            // Генеруємо сіль і хеш
            var salt = RandomNumberGenerator.GetBytes(16);
            var saltB64 = Convert.ToBase64String(salt);
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: vm.Password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32));

            var user = new User
            {
                Username = vm.Username,
                Email = vm.Email,
                PasswordSalt = saltB64,
                PasswordHash = hash,
                Country = vm.Country,
                IsAdmin = false
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", "User");

            return RedirectToAction("Index", "Home");
        }
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Ищем пользователя по email
            var user = _context.Users.FirstOrDefault(u => u.Email == vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(vm);
            }

            // Восстанавливаем соль и хеш
            var salt = Convert.FromBase64String(user.PasswordSalt);
            var incomingHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: vm.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32));

            if (incomingHash != user.PasswordHash)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(vm);
            }

            // Сохраняем сессию
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);

            // Редирект на Home/Index
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
