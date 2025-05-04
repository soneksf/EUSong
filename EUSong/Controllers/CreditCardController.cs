using Microsoft.AspNetCore.Mvc;
using EUSong.Data;
using EUSong.Models;
using System.Linq;

namespace EUSong.Controllers
{
    public class CreditCardController : Controller
    {
        private readonly AppDbContext _context;
        public CreditCardController(AppDbContext context) => _context = context;

        // GET: /CreditCard/Add?returnTo=SongIndex&songId=123
        [HttpGet]
        public IActionResult Add(string? returnTo = null, int? songId = null)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var vm = new AddCreditCardViewModel
            {
                ReturnTo = returnTo,
                SongId = songId
            };
            return View(vm);
        }

        // POST: /CreditCard/Add
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(AddCreditCardViewModel vm)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(vm);

            var card = new CreditCard
            {
                UserId = userId.Value,
                CardNumber = vm.CardNumber,
                ExpiryDate = vm.ExpiryDate,
                CVV = vm.CVV
            };

            _context.CreditCards.Add(card);
            _context.SaveChanges();

            if (vm.SongId.HasValue)
                return RedirectToAction("Create", "Vote", new { songId = vm.SongId.Value });

            if (!string.IsNullOrWhiteSpace(vm.ReturnTo))
                return Redirect(vm.ReturnTo!);

            return RedirectToAction("Index", "Song");
        }

        // (Optional) list saved cards
        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cards = _context.CreditCards
                                .Where(c => c.UserId == userId.Value)
                                .ToList();
            return View(cards);
        }
    }
}
