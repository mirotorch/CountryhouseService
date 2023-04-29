using System.ComponentModel.DataAnnotations;

namespace CountryhouseService.Models
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }
       [Required]
        public byte[] file { get; set; }
        public Ad Ad { get; set; }
    }
}