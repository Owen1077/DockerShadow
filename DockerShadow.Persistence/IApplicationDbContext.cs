using DockerShadow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;

namespace DockerShadow.Persistence
{
    public interface IApplicationDbContext
    {
        public DbSet<BankToBrokerFundWalletLog> BankToBrokerFundWalletLogs { get; set; }
        public DbSet<BankToBrokerLog> BankToBrokerLogs { get; set; }
        public DbSet<BankToBrokerWithdrawalLog> BankToBrokerWithdrawalLogs { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DatabaseFacade Database { get; }
        Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default);
        Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
    }
}