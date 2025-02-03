using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CartItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? CartID { get; set; }

        public decimal? ItemID { get; set; }

        [StringLength(50)]
        public string? ItemName { get; set; }

        public decimal? Price { get; set; }

        public decimal? Qty { get; set; }

        public decimal? ClientID { get; set; }

        public string? ItemImage { get; set; }
        public int? Inv_ItemsItemId { get; set; }

        [StringLength(100)]
        public string? CartSessionId { get; set; }

        [StringLength(50)]
        public string? CartStatus { get; set; }

        public DateTime? Date { get; set; }

        public string? MacAddress { get; set; }

    }
}
