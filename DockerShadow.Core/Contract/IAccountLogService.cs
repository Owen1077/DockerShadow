using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.QueryParameters;

namespace DockerShadow.Core.Contract
{
    public interface IAccountLogService
    {
        Task<byte[]> DownloadLogs(DownloadLogsRequest request, CancellationToken cancellationToken);
        Task<PagedResponse<List<AccountLogResponse>>> GetLogs(LogQueryParameters queryParameters, CancellationToken cancellationToken);
        Task<Response<AccountLogResponse>> GetLogById(long id, CancellationToken cancellationToken);
    }
}
