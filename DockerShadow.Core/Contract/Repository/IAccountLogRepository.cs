using DockerShadow.Core.DTO.Request;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.Entities;

namespace DockerShadow.Core.Contract.Repository
{
    public interface IAccountLogRepository
    {
        Task<List<BankToBrokerLog>> GetByDateRange(GetLogsByDateReq request, CancellationToken cancellationToken = default);
        Task<BankToBrokerLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<PagedList<BankToBrokerLog>> GetPagedByDateRange(GetPagedTxnsByDateReq request, CancellationToken cancellationToken = default);
    }
}
