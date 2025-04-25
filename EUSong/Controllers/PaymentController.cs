using Microsoft.AspNetCore.Mvc;
//using EurovisionSongApp.Data;
using EUSong.Models;
using EUSong.Data;


namespace EUSong.Controllers
{
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Payment/AddCard
        public IActionResult AddCard()
        {
            return View();
        }

        // POST: Payment/AddCard
        [HttpPost]
        public IActionResult AddCard(string cardNumber, string expiryDate, string cvv)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var card = new CreditCard
            {
                UserId = userId.Value,
                CardNumber = cardNumber,
                ExpiryDate = expiryDate,
                CVV = cvv
            };

            _context.CreditCards.Add(card);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Credit card added successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}
