using EUSong.Models;
using System.ComponentModel.DataAnnotations;

namespace EUSong.Models
{
    public class CreditCard
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [CreditCard]
        public string CardNumber { get; set; }

        public string ExpiryDate { get; set; }

        public string CVV { get; set; }
    }
}
