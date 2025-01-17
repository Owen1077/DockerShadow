using System.ComponentModel.DataAnnotations;

namespace DockerShadow.Core.DTO.Request
{
    public class DownloadLogsRequest
    {
        [DataType(DataType.Date)]
        public string StartDate { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public string EndDate { get; set; } = string.Empty;
    }
}
