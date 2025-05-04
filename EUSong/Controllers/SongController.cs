using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Data;
using EUSong.Models;
using System.Linq;

namespace EUSong.Controllers
{
    public class SongController : Controller
    {
        private readonly AppDbContext _context;

        public SongController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Song?keyword=&country=&singer=&year=
        public IActionResult Index(string keyword, string country, string singer, int? year)
        {
            // 1) Базовий запит
            var query = _context.Songs.AsQueryable();

            // 2) Фільтри
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(s =>
                    s.Title.Contains(keyword) ||
                    s.Description.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(s => s.Country.Contains(country));

            if (!string.IsNullOrWhiteSpace(singer))
                query = query.Where(s => s.Singer.Contains(singer));

            if (year.HasValue)
                query = query.Where(s => s.Year == year.Value);

            // 3) Виконуємо запит
            var songs = query.ToList();

            // 4) Передаємо стан фільтрів у ViewBag
            ViewBag.Keyword = keyword;
            ViewBag.Country = country;
            ViewBag.Singer = singer;
            ViewBag.Year = year;

            // 5) Передаємо прапор аутентифікації й наявності карти
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.IsAuthenticated = userId.HasValue;
            bool hasCard = false;
            if (userId.HasValue)
                hasCard = _context.CreditCards.Any(c => c.UserId == userId.Value);
            ViewBag.HasCard = hasCard;

            return View(songs);
        }

        // GET: /Song/Results?top=5&year=2025
        public IActionResult Results(int top = 10, int? year = null)
        {
            // 1) Базовий запит із підключенням Votes
            var query = _context.Songs
                                .Include(s => s.Votes)
                                .AsQueryable();

            // 2) Фільтр по року
            if (year.HasValue)
                query = query.Where(s => s.Year == year.Value);

            // 3) Рахуємо і сортуємо
            var results = query
                .Select(s => new
                {
                    Song = s,
                    TotalPoints = s.Votes.Sum(v => v.Value)
                })
                .OrderByDescending(x => x.TotalPoints)
                .Take(top)
                .ToList();

            // 4) Зберігаємо параметри у ViewBag
            ViewBag.Top = top;
            ViewBag.Year = year;

            return View(results);
        }
    }
}
