using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DockerShadow.Domain.Entities
{
    public class UserRole : IdentityUserRole<string>
    {
        [StringLength(450)]
        public override string UserId { get; set; } = string.Empty;

        public virtual User? User { get; set; }
        [StringLength(450)]

        public override string RoleId { get; set; } = string.Empty;

        public virtual Role? Role { get; set; }
    }
}