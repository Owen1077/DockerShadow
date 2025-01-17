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
    public class AccountLogRepository : RepositoryBase, IAccountLogRepository
    {
        private readonly ILogger<AccountLogRepository> _logger;
        public AccountLogRepository(IServiceScopeFactory serviceScopeFactory,
            ILogger<AccountLogRepository> logger)
            : base(serviceScopeFactory)
        {
            _logger = logger;
        }

        public async Task<BankToBrokerLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = GetDatabaseContext(scope);

            var response = await dbContext.BankToBrokerLogs
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return response;
        }

        public async Task<PagedList<BankToBrokerLog>> GetPagedByDateRange(GetPagedTxnsByDateReq request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = GetDatabaseContext(scope);

            IQueryable<BankToBrokerLog> query = dbContext.BankToBrokerLogs
                .Where(x => x.CreatedDate >= request.StartDate)
                .Where(x => x.CreatedDate <= request.EndDate);

            List<BankToBrokerLog> response = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            int totalRecords = await query.CountAsync(cancellationToken);

            return new PagedList<BankToBrokerLog>(response, totalRecords);
        }

        public async Task<List<BankToBrokerLog>> GetByDateRange(GetLogsByDateReq request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = GetDatabaseContext(scope);

            IQueryable<BankToBrokerLog> query = dbContext.BankToBrokerLogs
                .Where(x => x.CreatedDate >= request.StartDate)
                .Where(x => x.CreatedDate <= request.EndDate);

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
