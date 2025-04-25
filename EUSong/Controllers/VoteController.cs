using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Models;
using System.Linq;
using EUSong.Data;
using EUSong.Models;

namespace EUSong.Controllers
{
    public class VoteController : Controller
    {
        private readonly AppDbContext _context;

        public VoteController(AppDbContext context)
        {
            _context = context;
        }

        // Список пісень для голосування
        public IActionResult Index()
        {
            var songs = _context.Songs.ToList();
            return View(songs);
        }

        // GET: Vote/Create/5
        public IActionResult Create(int id)
        {
            var song = _context.Songs.FirstOrDefault(s => s.Id == id);
            if (song == null) return NotFound();

            ViewBag.Song = song;
            return View();
        }

        // POST: Vote/Create
        [HttpPost]
        public IActionResult Create(int songId, string type, int value)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users
                        .Include(u => u.CreditCards)
                        .FirstOrDefault(u => u.Id == userId);

            var song = _context.Songs.Find(songId);

            if (user.Country == song.Country)
            {
                ViewBag.Error = "You cannot vote for your own country.";
                ViewBag.Song = song;
                return View();
            }

            // ➡ Перевірка наявності кредитної картки
            if (user.CreditCards == null || !user.CreditCards.Any())
            {
                TempData["ErrorMessage"] = "You must add a credit card before voting.";
                return RedirectToAction("AddCard", "Payment");
            }

            var vote = new Vote
            {
                SongId = songId,
                UserId = user.Id,
                Value = value,
                Type = type
            };

            _context.Votes.Add(vote);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
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
