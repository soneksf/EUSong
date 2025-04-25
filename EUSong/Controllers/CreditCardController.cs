using EUSong.Data;
using EUSong.Models;
using Microsoft.AspNetCore.Mvc;

namespace EUSong.Controllers
{
    public class CreditCardController : Controller
    {
        private readonly AppDbContext _context;

        public CreditCardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Add(int? returnToVote)
        {
            ViewBag.ReturnToVote = returnToVote;
            return View();
        }

        [HttpPost]
        public IActionResult Add(string cardNumber, string expiryDate, string cvv, int? returnToVote)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var card = new CreditCard
            {
                UserId = userId.Value,
                CardNumber = cardNumber,
                ExpiryDate = expiryDate,
                CVV = cvv
            };

            _context.CreditCards.Add(card);
            _context.SaveChanges();

            if (returnToVote != null)
                return RedirectToAction("Create", "Vote", new { songId = returnToVote.Value });

            return RedirectToAction("Index", "Song");
        }
    }

}
