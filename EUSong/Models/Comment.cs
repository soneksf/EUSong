using EUSong.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace EUSong.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Text { get; set; }

        [Range(1, 10)]
        public int Rating { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
