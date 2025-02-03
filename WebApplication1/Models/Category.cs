using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Primary Key

        [MaxLength(500)]
        public string? CategoryName { get; set; } // Nullable

        public bool? IsChild { get; set; } // Nullable

        public int? Root { get; set; } // Nullable (for parent-child relationship)

        public bool? UBLDiscount { get; set; } // Nullable

        public bool? HBLDiscount { get; set; } // Nullable
        public ICollection<Inv_Items>? InvItems { get; set; } // Navigation Property

    }

}


