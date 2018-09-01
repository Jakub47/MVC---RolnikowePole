using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class LoginViewModel
    {
        [Required( ErrorMessage = "Musisz wprowadzic email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required( ErrorMessage = "Musisz wprowadzic hasło")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamietaj mnie")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} musi mieć co najmniej {2} znaków", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź Hasło")]
        [Compare("Password", ErrorMessage = "Hasło i potwierdzenie hasła nie pasuje do siebie")]
        public string ConfirmPassword { get; set; }
    }
}