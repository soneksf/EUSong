// Models/CreditCard.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EUSong.Models
{
    public class CreditCard : IValidatableObject
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$",
            ErrorMessage = "Expiry date must be in MM/YY format")]
        public string ExpiryDate { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$",
            ErrorMessage = "CVV must be exactly 3 digits")]
        public string CVV { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // ensure the expiry isn't already past
            if (DateTime.TryParseExact(
                    ExpiryDate,
                    "MM/yy",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out var exp))
            {
                // move to end of month
                var lastOfMonth = new DateTime(exp.Year, exp.Month,
                    DateTime.DaysInMonth(exp.Year, exp.Month));
                if (lastOfMonth < DateTime.UtcNow.Date)
                {
                    yield return new ValidationResult(
                        "Credit card has expired",
                        new[] { nameof(ExpiryDate) });
                }
            }
            else
            {
                // Regex should have caught format errors already
            }
        }
    }
}
