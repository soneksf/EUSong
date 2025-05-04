// Controllers/AdminVoteController.cs
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Data;
using EUSong.Models;

namespace EUSong.Controllers
{
    public class AdminVoteController : Controller
    {
        private readonly AppDbContext _context;
        public AdminVoteController(AppDbContext context) => _context = context;

        private bool IsAdminOrAbove()
            => new[] { "Admin", "SuperAdmin" }
                .Contains(HttpContext.Session.GetString("UserRole"));

        // Show all judge votes
        public IActionResult JudgeVotes()
        {
            if (!IsAdminOrAbove()) return Unauthorized();

            var votes = _context.Votes
                .Include(v => v.Song)
                .Include(v => v.User)
                .Where(v => v.Type == "judge")
                .ToList();

            return View(votes);
        }

        // GET: AdminVote/Create?songId=123
        [HttpGet]
        public IActionResult Create(int songId)
        {
            if (!IsAdminOrAbove()) return Unauthorized();

            var song = _context.Songs.Find(songId);
            if (song == null) return NotFound();

            ViewBag.SongTitle = song.Title;
            return View(new AddVoteViewModel { SongId = songId });
        }

        // POST: AdminVote/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(AddVoteViewModel vm)
        {
            if (!IsAdminOrAbove()) return Unauthorized();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                var song = _context.Songs.Find(vm.SongId);
                ViewBag.SongTitle = song?.Title ?? "—";
                return View(vm);
            }

            var vote = new Vote
            {
                SongId = vm.SongId,
                Value = vm.Value,
                UserId = userId.Value,
                Timestamp = DateTime.UtcNow,
                Type = "judge"
            };

            _context.Votes.Add(vote);
            _context.SaveChanges();

            return RedirectToAction(nameof(JudgeVotes));
        }
    }
}
