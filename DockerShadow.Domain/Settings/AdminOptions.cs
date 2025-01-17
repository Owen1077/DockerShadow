namespace DockerShadow.Domain.Settings
{
    public class AdminOptions
    {
        public bool EnableSelfAuthCheck { get; set; }
        public string BroadcastEmail { get; set; }
        public string[] AllowedHosts { get; set; }
    }
}