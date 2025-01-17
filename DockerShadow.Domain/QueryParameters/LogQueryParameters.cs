using System.ComponentModel.DataAnnotations;

namespace DockerShadow.Domain.QueryParameters
{
    public class LogQueryParameters : UrlQueryParameters
    {
        [DataType(DataType.Date)]
        public string? StartDate { get; set; }
        [DataType(DataType.Date)]
        public string? EndDate { get; set; }
    }
}
