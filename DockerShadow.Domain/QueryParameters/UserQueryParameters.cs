using DockerShadow.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DockerShadow.Domain.QueryParameters
{
    public class UserQueryParameters : UrlQueryParameters
    {
        [DataType(DataType.Text)]
        public string? Query { get; set; }
        [DataType(DataType.Text)]
        public string? Role { get; set; }
        [DataType(DataType.Text)]
        public UserStatus? Status { get; set; }
    }
}
