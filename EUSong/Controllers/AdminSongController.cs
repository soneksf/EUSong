// Controllers/AdminSongController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using EUSong.Data;
using EUSong.Models;

namespace EUSong.Controllers
{
    public class AdminSongController : Controller
    {
        private readonly AppDbContext _context;
        public AdminSongController(AppDbContext context) => _context = context;

        // Roles check
        private bool IsAdminOrAbove()
            => new[] { "Admin", "SuperAdmin" }
                .Contains(HttpContext.Session.GetString("UserRole"));

        // GET: AdminSong
        public IActionResult Index(string country, int? year)
        {
            if (!IsAdminOrAbove()) return Unauthorized();

            var songs = _context.Songs.AsQueryable();
            if (!string.IsNullOrEmpty(country))
                songs = songs.Where(s => s.Country.Contains(country));
            if (year.HasValue)
                songs = songs.Where(s => s.Year == year.Value);

            ViewBag.SelectedCountry = country;
            ViewBag.SelectedYear = year;
            return View(songs.ToList());
        }

        // Helper for country dropdown
        private void PopulateCountries(string? selected = null)
        {
            var countries = new[] {
                "Sweden","Finland","Italy","Norway","Australia",
                "Israel","Switzerland","Estonia","Germany","Ukraine"
            };
            ViewBag.Countries = new SelectList(countries, selected);
        }

        // GET: AdminSong/Create
        public IActionResult Create()
        {
            if (!IsAdminOrAbove()) return Unauthorized();
            PopulateCountries();
            return View();
        }

        // POST: AdminSong/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Description,Singer,Year,Country")] Song song)
        {
            if (!IsAdminOrAbove()) return Unauthorized();
            if (!ModelState.IsValid)
            {
                PopulateCountries(song.Country);
                return View(song);
            }

            _context.Songs.Add(song);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminSong/Edit/5
        public IActionResult Edit(int id)
        {
            if (!IsAdminOrAbove()) return Unauthorized();
            var song = _context.Songs.Find(id);
            if (song == null) return NotFound();

            PopulateCountries(song.Country);
            return View(song);
        }

        // POST: AdminSong/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Title,Description,Singer,Year,Country")] Song song)
        {
            if (!IsAdminOrAbove()) return Unauthorized();
            if (!ModelState.IsValid)
            {
                PopulateCountries(song.Country);
                return View(song);
            }

            _context.Songs.Update(song);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminSong/Delete/5
        public IActionResult Delete(int id)
        {
            if (!IsAdminOrAbove()) return Unauthorized();
            var song = _context.Songs.Find(id);
            if (song == null) return NotFound();
            return View(song);
        }

        // POST: AdminSong/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdminOrAbove()) return Unauthorized();
            var song = _context.Songs.Find(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminSong/Import
        public IActionResult Import()
        {
            if (!IsAdminOrAbove()) return Unauthorized();
            return View();
        }

        // POST: AdminSong/Import
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile excelFile)
        {
            if (!IsAdminOrAbove()) return Unauthorized();

            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select a valid Excel file.");
                return View();
            }

            ExcelPackage.License.SetNonCommercialPersonal("EUSong Admin");

            var errors = new List<string>();
            var imported = 0;

            using (var stream = excelFile.OpenReadStream())
            using (var pkg = new ExcelPackage(stream))
            {
                var ws = pkg.Workbook.Worksheets.FirstOrDefault();
                if (ws == null)
                {
                    ModelState.AddModelError("", "No worksheet found in file.");
                    return View();
                }

                for (int row = 2; row <= ws.Dimension.End.Row; row++)
                {
                    try
                    {
                        var title = ws.Cells[row, 1].Text?.Trim();
                        var singer = ws.Cells[row, 2].Text?.Trim();
                        var country = ws.Cells[row, 3].Text?.Trim();
                        var yearText = ws.Cells[row, 4].Text?.Trim();
                        var description = ws.Cells[row, 5].Text?.Trim();

                        if (string.IsNullOrEmpty(title) ||
                            string.IsNullOrEmpty(singer) ||
                            string.IsNullOrEmpty(country) ||
                            !int.TryParse(yearText, out var year))
                        {
                            errors.Add($"Row {row}: invalid or missing data.");
                            continue;
                        }
                        if (year < 1900 || year > 2100)
                        {
                            errors.Add($"Row {row}: Year {year} is out of range.");
                            continue;
                        }

                        _context.Songs.Add(new Song
                        {
                            Title = title,
                            Singer = singer,
                            Country = country,
                            Year = year,
                            Description = description
                        });
                        imported++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();
            }

            ViewBag.ImportedCount = imported;
            ViewBag.Errors = errors;
            return View("ImportResult");
        }
    }
}
