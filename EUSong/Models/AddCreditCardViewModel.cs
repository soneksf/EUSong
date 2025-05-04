// Models/AddCreditCardViewModel.cs
using System.ComponentModel.DataAnnotations;
namespace EUSong.Models
{
    public class AddCreditCardViewModel
    {
        [Required, CreditCard]
        public string CardNumber { get; set; }

        [Required]
        public string ExpiryDate { get; set; }

        [Required]
        public string CVV { get; set; }

        // hidden fields
        public string? ReturnTo { get; set; }
        public int? SongId { get; set; }
    }
}