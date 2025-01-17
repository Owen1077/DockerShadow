namespace DockerShadow.Core.DTO.Response
{
    public class AccountLogResponse
    {
        public long Id { get; set; }

        public string? AccountNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? UserId { get; set; }
    }
}
