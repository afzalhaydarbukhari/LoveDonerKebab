using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Login")]
    public class Register
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserCode { get; set; }
        public string? ID { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Access { get; set; }
    }
}
