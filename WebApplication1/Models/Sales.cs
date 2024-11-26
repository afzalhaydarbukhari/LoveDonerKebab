using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Inv_Sale")]
    public class Sales
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal SaleId { get; set; }

        public decimal ClientId { get; set; }  // Foreign key for the Client

        public DateTime? SaleDate { get; set; }
        public DateTime? LastModified { get; set; }
        public decimal? Payment { get; set; }
        public string? Status { get; set; }
        public decimal? Cash_Received { get; set; }
        public decimal? Paid_Back { get; set; }
        public string? Modifier { get; set; }
        public int? TokenNumber { get; set; }
        public string? Serving { get; set; }

        [ForeignKey("ClientId")]
        public Client clients { get; set; }
    }
}

