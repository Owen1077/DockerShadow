namespace DockerShadow.Core.DTO.Request
{
    public class SendMailRequest
    {
        public string From { get; set; } = "noreply@accessbankplc.com";
        public string Recipient { get; set; } = string.Empty;
        public string CopyAddress { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}