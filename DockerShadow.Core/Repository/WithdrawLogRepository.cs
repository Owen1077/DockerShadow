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
    public class WithdrawLogRepository : RepositoryBase, IWithdrawLogRepository
    {
        private readonly ILogger<WithdrawLogRepository> _logger;
        public WithdrawLogRepository(IServiceScopeFactory serviceScopeFactory,
            ILogger<WithdrawLogRepository> logger)
            : base(serviceScopeFactory)
        {
            _logger = logger;
        }

        public async Task<BankToBrokerWithdrawalLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = GetDatabaseContext(scope);

            var response = await dbContext.BankToBrokerWithdrawalLogs
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return response;
        }

        public async Task<PagedList<BankToBrokerWithdrawalLog>> GetPagedByDateRange(GetPagedTxnsByDateReq request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = GetDatabaseContext(scope);

            IQueryable<BankToBrokerWithdrawalLog> query = dbContext.BankToBrokerWithdrawalLogs
                .Where(x => x.CreatedDate >= request.StartDate)
                .Where(x => x.CreatedDate <= request.EndDate);

            List<BankToBrokerWithdrawalLog> response = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            int totalRecords = await query.CountAsync(cancellationToken);

            return new PagedList<BankToBrokerWithdrawalLog>(response, totalRecords);
        }

        public async Task<List<BankToBrokerWithdrawalLog>> GetByDateRange(GetLogsByDateReq request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = GetDatabaseContext(scope);

            IQueryable<BankToBrokerWithdrawalLog> query = dbContext.BankToBrokerWithdrawalLogs
                .Where(x => x.CreatedDate >= request.StartDate)
                .Where(x => x.CreatedDate <= request.EndDate);

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
