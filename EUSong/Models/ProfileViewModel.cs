using System;
using System.Collections.Generic;

namespace EUSong.Models
{
    public class ProfileViewModel
    {
        public List<VoteDto> Votes { get; set; }
        public List<CommentDto> Comments { get; set; }
    }

    public class VoteDto
    {
        public string SongTitle { get; set; }
        public int Value { get; set; }
        // за потреби: public string Type { get; set; } 
        public DateTime Timestamp { get; set; }
    }

    public class CommentDto
    {
        public string SongTitle { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
