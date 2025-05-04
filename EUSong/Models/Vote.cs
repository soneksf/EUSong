using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EUSong.Models
{
    public class Vote
    {
        public int Id { get; set; }

        [Range(1, 12)]
        public int Value { get; set; }

        [Required]
        public string Type { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }

        
        public DateTime Timestamp { get; set; }
    }
}
