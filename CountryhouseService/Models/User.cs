using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryhouseService.Models
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [MaxLength(256, ErrorMessage = "First name should be less than 256 characters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(256, ErrorMessage = "Last name should be less than 256 characters")]
        public string LastName { get; set; }
        
        public string? AvatarSource { get; set; }
    }
}