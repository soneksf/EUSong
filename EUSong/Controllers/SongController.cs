using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EUSong.Data;
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

        public IActionResult Index(string keyword, string country, string singer, int? year)
        {
            var songs = _context.Songs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                songs = songs.Where(s =>
                    s.Title.Contains(keyword) ||
                    s.Description.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(country))
            {
                songs = songs.Where(s => s.Country.Contains(country));
            }

            if (!string.IsNullOrEmpty(singer))
            {
                songs = songs.Where(s => s.Singer.Contains(singer));
            }

            if (year.HasValue)
            {
                songs = songs.Where(s => s.Year == year.Value);
            }

            ViewBag.Keyword = keyword;
            ViewBag.Country = country;
            ViewBag.Singer = singer;
            ViewBag.Year = year;

            return View(songs.ToList());
        }
        public IActionResult Results(int top = 10, int? year = null)
        {
            var songs = _context.Songs
                .Include(s => s.Votes)
                .AsQueryable();

            if (year.HasValue)
            {
                songs = songs.Where(s => s.Year == year.Value);
            }

            var results = songs
                .Select(s => new
                {
                    Song = s,
                    TotalPoints = s.Votes.Sum(v => v.Value)
                })
                .OrderByDescending(x => x.TotalPoints)
                .Take(top)
                .ToList();

            ViewBag.Top = top;
            ViewBag.Year = year;

            return View(results);
        }

    }
}
