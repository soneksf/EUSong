using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Data;
using System.Linq;

namespace EUSong.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Allow both Admin and SuperAdmin
        private bool IsAdminOrAbove()
            => new[] { "Admin", "SuperAdmin" }
                .Contains(HttpContext.Session.GetString("UserRole"));

        // Only SuperAdmin (if you need it for other actions)
        private bool IsSuperAdmin()
            => HttpContext.Session.GetString("UserRole") == "SuperAdmin";

        public IActionResult Dashboard()
        {
            // Guard: Admin or SuperAdmin may view
            if (!IsAdminOrAbove())
                return Unauthorized();

            ViewBag.TotalSongs = _context.Songs.Count();
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalVotes = _context.Votes.Count();
            ViewBag.JudgeVotes = _context.Votes.Count(v => v.Type == "judge");
            ViewBag.ListenerVotes = _context.Votes.Count(v => v.Type == "listener");

            ViewBag.SongsByYear = _context.Songs
                .GroupBy(s => s.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .OrderBy(x => x.Year)
                .ToList();

            ViewBag.SongAverageVotes = _context.Songs
                .Select(s => new
                {
                    s.Title,
                    AverageVote = s.Votes.Any() ? s.Votes.Average(v => v.Value) : 0
                })
                .OrderByDescending(x => x.AverageVote)
                .Take(5)
                .ToList();

            return View();
        }

        // Example of an action only SuperAdmin can perform:
        public IActionResult Secrets()
        {
            if (!IsSuperAdmin())
                return Unauthorized();

            // ... super-secret stuff ...
            return View();
        }
    }
}
