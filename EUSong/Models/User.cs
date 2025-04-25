using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EUSong.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Country { get; set; }
        public string Role { get; set; } = "User"; 

        public ICollection<Vote> Votes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<CreditCard> CreditCards { get; set; }
    }
}
