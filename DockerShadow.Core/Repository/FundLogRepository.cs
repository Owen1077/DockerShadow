using DockerShadow.Core.Contract.Repository;
using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.Repository.Base;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DockerShadow.Core.Repository
{
    public class FundLogRepository : RepositoryBase, IFundLogRepository
    {
        private readonly ILogger<FundLogRepository> _logger;
        public FundLogRepository(IServiceScopeFactory serviceScopeFactory,
            ILogger<FundLogRepository> logger)
            : base(serviceScopeFactory)
        {
            _logger = logger;
        }

        public async Task<BankToBrokerFundWalletLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = GetDatabaseContext(scope);

            var response = await dbContext.BankToBrokerFundWalletLogs
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return response;
        }

        public async Task<PagedList<BankToBrokerFundWalletLog>> GetPagedByDateRange(GetPagedTxnsByDateReq request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = GetDatabaseContext(scope);

            IQueryable<BankToBrokerFundWalletLog> query = dbContext.BankToBrokerFundWalletLogs
                .Where(x => x.CreatedDate >= request.StartDate)
                .Where(x => x.CreatedDate <= request.EndDate);

            List<BankToBrokerFundWalletLog> response = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            int totalRecords = await query.CountAsync(cancellationToken);

            return new PagedList<BankToBrokerFundWalletLog>(response, totalRecords);
        }

        public async Task<List<BankToBrokerFundWalletLog>> GetByDateRange(GetLogsByDateReq request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = GetDatabaseContext(scope);

            IQueryable<BankToBrokerFundWalletLog> query = dbContext.BankToBrokerFundWalletLogs
                .Where(x => x.CreatedDate >= request.StartDate)
                .Where(x => x.CreatedDate <= request.EndDate);

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
