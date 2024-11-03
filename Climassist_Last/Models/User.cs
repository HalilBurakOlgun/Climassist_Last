using System;
using System.ComponentModel.DataAnnotations;

namespace Climassist_Last.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        public string SurName { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; }

        public string UserType { get; set; } = "Customer"; // Varsayılan değer

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}