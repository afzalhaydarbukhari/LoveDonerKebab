using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Inv_Items")]
    public class Inv_Items
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ItemId { get; set; }

        //public int Clientid { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Item name must not exceed 50 characters")]
        public string? ItemName { get; set; }
        public int? CategoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
        public decimal RecentUnitPrice { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public int? Discount { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Remarks must not exceed 200 characters")]
        public string? Remarks { get; set; }

        [DisplayName("Choose Image")]
        [Required]
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? ImagePath { get; set; }
        public Category Category { get; set; }
        public List<Category>? categorylist { get; set; } 

    }
}
