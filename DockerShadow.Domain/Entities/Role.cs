using Microsoft.AspNetCore.Identity;

namespace DockerShadow.Domain.Entities
{
    public class Role : IdentityRole
    {
        public virtual IList<UserRole>? UserRoles { get; set; }
    }
}