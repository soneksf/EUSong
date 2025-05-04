using EUSong.Models;
using System.ComponentModel.DataAnnotations;
namespace EUSong.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required] public string Username { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Country { get; set; }

        // роль: "User", "Admin", "SuperAdmin"
        public string Role { get; set; } = "User";

        // для сумісності старого коду
        public bool IsAdmin
        {
            get => Role == "Admin" || Role == "SuperAdmin";
            set { /* можна лишити пустим */ }
        }

        public ICollection<Vote> Votes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<CreditCard> CreditCards { get; set; }
    }
}