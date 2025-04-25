using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EUSong.Models
{
   

    public class Song
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }

        [Required]
        public string Singer { get; set; }

        [Required]
        public string Country { get; set; }

        public ICollection<Vote> Votes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

}
