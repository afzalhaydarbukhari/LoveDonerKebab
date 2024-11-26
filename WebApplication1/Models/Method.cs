using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Methods")]
	public class Method
	{
        [Key]
        public int MethodId { get; set; }
        public string? MethodName { get; set; }
    }
}
