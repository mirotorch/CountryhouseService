using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryhouseService.Models
{
    public class Ad
    {
        [Key]
        public int AdId { get; set; }

        [MaxLength(100, ErrorMessage = "A title should be less than 100 characters")]
        [Required]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "A description should be less than 1000 characters")]
        [Required]
        [DisplayName("Description and list of tasks")]
        public string Description { get; set; }

        [MaxLength(100, ErrorMessage = "An address should be less than 100 characters")]
        [Required]
        public string Address { get; set; }
        [Required]
        public int Payment { get; set; }

        [MaxLength(25, ErrorMessage = "A contact number should be less than 25 characters")]
        [Required]
        [DisplayName("Contact number")]
        public string ContactNumber { get; set; }
        [Required]
        public System.DateTime created_at { get; set; } = DateTime.Now;
        [Required]
        public DateTime UpdatedOn { get; set; }

        [DisplayName("Start from date")]
        [Column(TypeName = "date")]
        public DateTime? FromDate { get; set; }

        [Required]
        [DisplayName("End until date")]
        [Column(TypeName = "date")]
        public DateTime UntilDate { get; set; }
        public string Status { get; set; } = "Published";
        public List<Image>? Images { get; set; }

        public string CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public User Creator { get; set; }
    }
}
