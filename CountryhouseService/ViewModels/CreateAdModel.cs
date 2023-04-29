using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CountryhouseService.Helpers;

namespace CountryhouseService.ViewModels
{
    public class CreateAdModel
    {

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
        [Range(0, int.MaxValue, ErrorMessage = "A budget cannot be negative")]
        public int Payment { get; set; }

        [MaxLength(25, ErrorMessage = "A contact number should be less than 25 characters")]
        [Required]
        [DisplayName("Contact number")]
        public string ContactNumber { get; set; }

        [DataType(DataType.Upload)]
        [TypeRequired("image/png", "image/jpeg", ErrorMessage = "Only .jpg .png .jfif or .jpeg files are allowed")]
        [DisplayName("Image")]
        public IFormFileCollection? Images { get; set; }

        [DisplayName("Start from date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Required]
        [DisplayName("End until date")]
        [DataType(DataType.Date)]
        public DateTime UntilDate { get; set; }

    }
}
