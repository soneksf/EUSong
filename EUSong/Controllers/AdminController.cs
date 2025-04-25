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

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return Unauthorized();

            ViewBag.TotalSongs = _context.Songs.Count();
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalVotes = _context.Votes.Count();
            ViewBag.JudgeVotes = _context.Votes.Count(v => v.Type == "judge");
            ViewBag.ListenerVotes = _context.Votes.Count(v => v.Type == "listener");

            // Нові дані
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
                .Take(5) // Топ 5 пісень
                .ToList();

            return View();
        }
    }
}
