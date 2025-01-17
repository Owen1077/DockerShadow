using DockerShadow.Domain.Enum;

namespace DockerShadow.Core.DTO.Response
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
        public UserStatus Status { get; set; }
        public string StatusMeaning => Status.ToString();
        public bool IsLoggedIn { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string JWToken { get; set; }
        public double ExpiresIn { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
