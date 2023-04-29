using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryhouseService.Models
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        public int? AdId { get; set; }
        [ForeignKey("AdId")]
        public Ad? Ad { get; set; }  
        [Required]
        public string Status { get; set; }
        [MaxLength(480)]
        public string? Comment { get; set; }

        public string? WorkerId { get; set; }
        [ForeignKey("WorkerId")]
        public User? Worker { get; set; }
    }
}
