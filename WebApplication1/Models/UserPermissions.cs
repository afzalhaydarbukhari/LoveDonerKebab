using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("UserPermission")]
	public class UserPermissions
	{
        [Key]
        public int? PermissionId { get; set; }
        public int UserCode { get; set; }
        public int? MethodId { get; set; }
        public string? MethodName { get; set; }
        public bool View { get; set; }
    }
}
