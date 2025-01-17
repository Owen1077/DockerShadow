namespace DockerShadow.Core.DTO.Request
{
    public class GetPagedTxnsByDateReq
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}
