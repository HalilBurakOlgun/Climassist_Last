using System;
using System.ComponentModel.DataAnnotations;

namespace Climassist_Last.Models
{
    public class Requests
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UserSurname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string ClientType { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string RequestType { get; set; } = string.Empty;

        public string SparePartType { get; set; } = string.Empty;

        public string RecoveryTime { get; set; } = string.Empty;

        public string UnitType { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}