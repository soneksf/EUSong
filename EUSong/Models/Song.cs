// Models/Song.cs
using EUSong.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace EUSong.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Singer { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [ValidateNever]
        public ICollection<Vote> Votes { get; set; }
        [ValidateNever]

        public ICollection<Comment> Comments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            // Disallow any year greater than the current calendar year
            if (Year > DateTime.UtcNow.Year)
            {
                yield return new ValidationResult(
                    "Year cannot be in the future.",
                    new[] { nameof(Year) }
                );
            }
        }
    }
}