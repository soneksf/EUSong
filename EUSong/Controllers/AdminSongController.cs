using Microsoft.AspNetCore.Mvc;
using EUSong.Data;
using EUSong.Models;
using System.Linq;

namespace EUSong.Controllers
{
    public class AdminSongController : Controller
    {
        private readonly AppDbContext _context;

        public AdminSongController(AppDbContext context)
        {
            _context = context;
        }

        // Перевірка чи користувач є адміністратором
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // GET: AdminSong/Index
        public IActionResult Index(string country, int? year)
        {
            if (!IsAdmin()) return Unauthorized();

            var songs = _context.Songs.AsQueryable();

            if (!string.IsNullOrEmpty(country))
            {
                songs = songs.Where(s => s.Country.Contains(country));
            }

            if (year.HasValue)
            {
                songs = songs.Where(s => s.Year == year.Value);
            }

            ViewBag.SelectedCountry = country;
            ViewBag.SelectedYear = year;

            return View(songs.ToList());
        }


        // GET: AdminSong/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return Unauthorized();
            return View();
        }

        // POST: AdminSong/Create
        [HttpPost]
        public IActionResult Create(Song song)
        {
            if (!IsAdmin()) return Unauthorized();

            if (ModelState.IsValid)
            {
                _context.Songs.Add(song);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(song);
        }

        // GET: AdminSong/Edit/5
        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var song = _context.Songs.FirstOrDefault(s => s.Id == id);
            if (song == null) return NotFound();
            return View(song);
        }

        // POST: AdminSong/Edit/5
        [HttpPost]
        public IActionResult Edit(Song song)
        {
            if (!IsAdmin()) return Unauthorized();

            if (ModelState.IsValid)
            {
                _context.Songs.Update(song);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(song);
        }

        // GET: AdminSong/Delete/5
        public IActionResult Delete(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var song = _context.Songs.FirstOrDefault(s => s.Id == id);
            if (song == null) return NotFound();

            return View(song); // показуємо сторінку підтвердження
        }

        // POST: AdminSong/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var song = _context.Songs.Find(id);
            if (song == null) return NotFound();

            _context.Songs.Remove(song);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
