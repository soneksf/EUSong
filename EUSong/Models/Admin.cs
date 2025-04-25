using System.ComponentModel.DataAnnotations;

namespace EUSong.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        public string AdminName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Country { get; set; }
    }
}
