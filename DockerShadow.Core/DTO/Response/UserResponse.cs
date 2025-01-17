using DockerShadow.Domain.Enum;

namespace DockerShadow.Core.DTO.Response
{
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public string StatusMeaning => Status.ToString();
        public string Email { get; set; } = string.Empty;
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
