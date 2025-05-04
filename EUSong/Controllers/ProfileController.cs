using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Data;
using EUSong.Models;
using System.Linq;

namespace EUSong.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        public ProfileController(AppDbContext context)
            => _context = context;

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Вибираємо тільки Title та Value — без Timestamp, бо його в моделі нема
            var votes = _context.Votes
                .Include(v => v.Song)
                .Where(v => v.UserId == userId.Value)
                .Select(v => new VoteDto
                {
                    SongTitle = v.Song.Title,
                    Value = v.Value
                })
                .ToList();

            var comments = _context.Comments
                .Include(c => c.Song)
                .Where(c => c.UserId == userId.Value)
                .Select(c => new CommentDto
                {
                    SongTitle = c.Song.Title,
                    Text = c.Text,
                    Rating = c.Rating,
                    Timestamp = c.Timestamp
                })
                .ToList();

            var vm = new ProfileViewModel
            {
                Votes = votes,
                Comments = comments
            };

            return View(vm);
        }
    }
}
