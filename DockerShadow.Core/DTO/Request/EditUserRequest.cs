using System.ComponentModel.DataAnnotations;

namespace DockerShadow.Core.DTO.Request
{
    public class EditUserRequest
    {
        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        public string? Role { get; set; }
    }
}
