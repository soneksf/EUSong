using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Models;
using System.Linq;
using EUSong.Data;
using EUSong.Models;

namespace EUSong.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Comment/Index/5
        public IActionResult Index(int songId)
        {
            var song = _context.Songs
                .Include(s => s.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefault(s => s.Id == songId);

            if (song == null) return NotFound();

            ViewBag.Song = song;
            return View(song.Comments.ToList());
        }

        // GET: Comment/Create/5
        public IActionResult Create(int songId)
        {
            var song = _context.Songs.FirstOrDefault(s => s.Id == songId);
            if (song == null) return NotFound();

            ViewBag.Song = song;
            return View();
        }

        // POST: Comment/Create
        [HttpPost]
        public IActionResult Create(int songId, string text, int rating)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var comment = new Comment
            {
                SongId = songId,
                UserId = userId.Value,
                Text = text,
                Rating = rating,
                Timestamp = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Index", new { songId = songId });
        }
    }
}
