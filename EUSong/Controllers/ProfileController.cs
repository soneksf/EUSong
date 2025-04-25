using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Data;
using System.Linq;

namespace EUSong.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var votes = _context.Votes
                .Include(v => v.Song)
                .Where(v => v.UserId == userId && v.Type == "listener")
                .ToList();

            var comments = _context.Comments
                .Include(c => c.Song)
                .Where(c => c.UserId == userId)
                .ToList();

            ViewBag.Votes = votes;
            ViewBag.Comments = comments;

            return View();
        }
    }
}
