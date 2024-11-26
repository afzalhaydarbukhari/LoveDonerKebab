using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Inv_SaledItems")]
    public class SoldItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal EntryId { get; set; }  // Primary Key

        // Foreign Key fields with matching database type `numeric(18, 0)` as `decimal` in C#
        [Column(TypeName = "decimal(18, 0)")]
        public decimal SaleId { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        public decimal ItemId { get; set; }

        // Optional item name for display purposes
        public string? ItemName { get; set; }

        // Quantity, expected to be an integer
        public decimal Qty { get; set; }

        // Unit price per item, numeric(18, 1) in the database
        [Column(TypeName = "decimal(18, 1)")]
        public decimal UnitPrice { get; set; }

        // Calculated or net price after any discounts, numeric(18, 0) in the database
        [Column(TypeName = "decimal(18, 0)")]
        public decimal NetPrice { get; set; }

        // Uncomment and define additional fields if needed in the future
        // [Column(TypeName = "decimal(18, 1)")]
        // public decimal? UnitSalePrice { get; set; }

        // [Column(TypeName = "decimal(18, 1)")]
        // public decimal? Discount { get; set; }

        // Optional sale type (e.g., wholesale or retail)
        // public string? SaleType { get; set; }

        // Other optional financial fields for detailed tracking (uncomment if required)
        // [Column(TypeName = "decimal(18, 0)")]
        // public decimal? NetPriceWOD { get; set; }

        // [Column(TypeName = "decimal(18, 1)")]
        // public decimal? PurchasePrice { get; set; }

        // [Column(TypeName = "decimal(18, 1)")]
        // public decimal? Packing_Cost { get; set; }
    }
}
