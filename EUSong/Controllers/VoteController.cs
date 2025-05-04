// Controllers/VoteController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Data;
using EUSong.Models;
using System;
using System.Linq;

namespace EUSong.Controllers
{
    public class VoteController : Controller
    {
        private readonly AppDbContext _context;
        public VoteController(AppDbContext context) => _context = context;

        // Only real “User” role may vote
        private bool IsRegularUser()
            => HttpContext.Session.GetString("UserRole") == "User";

        // GET: /Vote
        public IActionResult Index()
        {
            ViewBag.Message = TempData["VoteMessage"];
            ViewBag.IsUser = IsRegularUser();
            var songs = _context.Songs.ToList();
            return View(songs);
        }

        // GET: /Vote/Create?songId=123
        [HttpGet]
        public IActionResult Create(int songId)
        {
            if (!IsRegularUser())
            {
                TempData["VoteMessage"] = "Only regular users may vote.";
                return RedirectToAction("Index", "Song");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            bool hasVotedAny = _context.Votes.Any(v => v.UserId == userId.Value);
            if (hasVotedAny)
            {
                TempData["VoteMessage"] = "You have already cast your vote.";
                return RedirectToAction("Index", "Song");
            }

            var song = _context.Songs.Find(songId);
            if (song == null) return NotFound();

            ViewBag.SongTitle = song.Title;
            return View(new AddVoteViewModel { SongId = songId });
        }

        // POST: /Vote/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(AddVoteViewModel vm)
        {
            if (!IsRegularUser())
            {
                TempData["VoteMessage"] = "Only regular users may vote.";
                return RedirectToAction("Index", "Song");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            bool hasVotedAny = _context.Votes.Any(v => v.UserId == userId.Value);
            if (hasVotedAny)
            {
                TempData["VoteMessage"] = "You have already cast your vote.";
                return RedirectToAction("Index", "Song");
            }

            if (!ModelState.IsValid)
            {
                var song = _context.Songs.Find(vm.SongId);
                ViewBag.SongTitle = song?.Title ?? "—";
                return View(vm);
            }

            _context.Votes.Add(new Vote
            {
                SongId = vm.SongId,
                Value = vm.Value,
                UserId = userId.Value,
                Timestamp = DateTime.UtcNow,
                Type = "Listener"
            });
            _context.SaveChanges();

            TempData["VoteMessage"] = "Thank you for voting!";
            return RedirectToAction("Index", "Song");
        }

        // Optional admin views
        public IActionResult JudgeVotes()
        {
            var songs = _context.Songs
                .Include(s => s.Votes)
                .Where(s => s.Votes.Any(v => v.Type == "judge"))
                .ToList();
            return View("VotesList", songs);
        }

        public IActionResult ListenerVotes()
        {
            var songs = _context.Songs
                .Include(s => s.Votes)
                .Where(s => s.Votes.Any(v => v.Type == "listener"))
                .ToList();
            return View("VotesList", songs);
        }
    }
}
