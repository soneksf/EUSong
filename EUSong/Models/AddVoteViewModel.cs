// Models/AddVoteViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EUSong.Models
{
    public class AddVoteViewModel : IValidatableObject
    {
        [Required]
        public int SongId { get; set; }

        [Required]
        [Display(Name = "Points (1–8,10,12)")]
        public int Value { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var allowed = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12 };
            if (!allowed.Contains(Value))
            {
                yield return new ValidationResult(
                    "Please select a valid score (1–8, 10 or 12).",
                    new[] { nameof(Value) }
                );
            }
        }
    }
}
