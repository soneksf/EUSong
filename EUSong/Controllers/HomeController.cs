using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EUSong.Models;
using EUSong.Data;

namespace EUSong.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId != null)
        {
            var user = _context.Users.Find(userId.Value);
            ViewBag.Username = user?.Username;
            ViewBag.Role = user?.Role;
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

}
