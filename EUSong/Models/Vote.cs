using EUSong.Models;
using System.ComponentModel.DataAnnotations;

namespace EUSong.Models
{
    public class Vote
    {
        public int Id { get; set; }

        public int Value { get; set; }

        [Required]
        public string Type { get; set; } // "judge" або "listener"

        public int UserId { get; set; }
        public User User { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }
    }
}
