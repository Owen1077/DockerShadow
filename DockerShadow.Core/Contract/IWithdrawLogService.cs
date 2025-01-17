using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.QueryParameters;

namespace DockerShadow.Core.Contract
{
    public interface IWithdrawLogService
    {
        Task<byte[]> DownloadLogs(DownloadLogsRequest request, CancellationToken cancellationToken);
        Task<PagedResponse<List<WithdrawLogResponse>>> GetLogs(LogQueryParameters queryParameters, CancellationToken cancellationToken);
        Task<Response<WithdrawLogResponse>> GetLogById(long id, CancellationToken cancellationToken);
    }
}
