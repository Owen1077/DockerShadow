using DockerShadow.Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace DockerShadow.Domain.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public bool IsLoggedIn { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = new List<IdentityUserClaim<string>>();
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; } = new List<IdentityUserLogin<string>>();
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; } = new List<IdentityUserToken<string>>();
    }
}