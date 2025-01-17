using DockerShadow.Core.DTO.Request;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.Entities;

namespace DockerShadow.Core.Contract.Repository
{
    public interface IFundLogRepository
    {
        Task<List<BankToBrokerFundWalletLog>> GetByDateRange(GetLogsByDateReq request, CancellationToken cancellationToken = default);
        Task<BankToBrokerFundWalletLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<PagedList<BankToBrokerFundWalletLog>> GetPagedByDateRange(GetPagedTxnsByDateReq request, CancellationToken cancellationToken = default);
    }
}
