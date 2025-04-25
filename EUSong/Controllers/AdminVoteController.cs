using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Data;
using EUSong.Models;
using System.Linq;

namespace EUSong.Controllers
{
    public class AdminVoteController : Controller
    {
        private readonly AppDbContext _context;

        public AdminVoteController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public IActionResult JudgeVotes()
        {
            if (!IsAdmin()) return Unauthorized();

            var votes = _context.Votes
                .Include(v => v.Song)
                .Include(v => v.User)
                .Where(v => v.Type == "judge")
                .ToList();

            return View(votes);
        }

        public IActionResult ListenerVotes()
        {
            if (!IsAdmin()) return Unauthorized();

            var votes = _context.Votes
                .Include(v => v.Song)
                .Include(v => v.User)
                .Where(v => v.Type == "listener")
                .ToList();

            return View(votes);
        }

        public IActionResult Delete(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var vote = _context.Votes
                .Include(v => v.Song)
                .Include(v => v.User)
                .FirstOrDefault(v => v.Id == id);

            if (vote == null) return NotFound();

            return View(vote);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var vote = _context.Votes.Find(id);
            if (vote == null) return NotFound();

            _context.Votes.Remove(vote);
            _context.SaveChanges();
            return RedirectToAction(nameof(JudgeVotes)); // Після видалення повертаємось на суддівські голоси
        }
    }
}
