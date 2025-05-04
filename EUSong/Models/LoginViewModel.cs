﻿// Models/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace EUSong.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
