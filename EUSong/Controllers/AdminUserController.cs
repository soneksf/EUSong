using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EUSong.Data;
using EUSong.Models;

namespace EUSong.Controllers
{
    public class AdminUserController : Controller
    {
        private readonly AppDbContext _context;
        public AdminUserController(AppDbContext context) => _context = context;

        private bool IsSuperAdmin()
            => HttpContext.Session.GetString("UserRole") == "SuperAdmin";

        // GET: /AdminUser
        public IActionResult Index()
        {
            if (!IsSuperAdmin()) return Unauthorized();

            // load all users
            var users = _context.Users.ToList();

            // pass current user's ID to the view
            ViewBag.CurrentUserId = HttpContext.Session.GetInt32("UserId");
            return View(users);
        }

        // GET: /AdminUser/Delete/5
        public IActionResult Delete(int id)
        {
            if (!IsSuperAdmin()) return Unauthorized();

            var currentId = HttpContext.Session.GetInt32("UserId");
            if (id == currentId)
                return BadRequest("You cannot delete your own SuperAdmin account.");

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /AdminUser/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsSuperAdmin()) return Unauthorized();

            var currentId = HttpContext.Session.GetInt32("UserId");
            if (id == currentId)
                return BadRequest("You cannot delete your own SuperAdmin account.");

            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
