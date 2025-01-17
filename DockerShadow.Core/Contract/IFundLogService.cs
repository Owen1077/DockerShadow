using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.QueryParameters;

namespace DockerShadow.Core.Contract
{
    public interface IFundLogService
    {
        Task<byte[]> DownloadLogs(DownloadLogsRequest request, CancellationToken cancellationToken);
        Task<PagedResponse<List<FundLogResponse>>> GetLogs(LogQueryParameters queryParameters, CancellationToken cancellationToken);
        Task<Response<FundLogResponse>> GetLogById(long id, CancellationToken cancellationToken);
    }
}
