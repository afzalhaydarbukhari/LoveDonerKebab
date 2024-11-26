using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Client")]
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Clientid { get; set; }

        [StringLength(500)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? PhoneNo { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime SupplierCreationDate { get; set; } = DateTime.Now;

    }
}


