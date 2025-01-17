using DockerShadow.Core.DTO.Request;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.Entities;

namespace DockerShadow.Core.Contract.Repository
{
    public interface IWithdrawLogRepository
    {
        Task<List<BankToBrokerWithdrawalLog>> GetByDateRange(GetLogsByDateReq request, CancellationToken cancellationToken = default);
        Task<BankToBrokerWithdrawalLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<PagedList<BankToBrokerWithdrawalLog>> GetPagedByDateRange(GetPagedTxnsByDateReq request, CancellationToken cancellationToken = default);
    }
}
