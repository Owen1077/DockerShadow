using DockerShadow.Domain.Entities;
using DockerShadow.Domain.Entities.Base;
using DockerShadow.Domain.Enum;
using DockerShadow.Domain.Settings;
using DockerShadow.Persistence.Seeds;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using System.Data;

namespace DockerShadow.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>, IApplicationDbContext
    {
        private IDbContextTransaction? _currentTransaction;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DatabaseOptions _databaseOptions;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor,
            IOptions<DatabaseOptions> databaseOptions) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _databaseOptions = databaseOptions.Value;
        }

        public DbSet<BankToBrokerFundWalletLog> BankToBrokerFundWalletLogs { get; set; }

        public DbSet<BankToBrokerLog> BankToBrokerLogs { get; set; }

        public DbSet<BankToBrokerWithdrawalLog> BankToBrokerWithdrawalLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(_databaseOptions.SchemaName);

            #region Identity Entities
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(name: "USER");
                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");
                entity.Property(x => x.AccessFailedCount)
                    .HasColumnName("ACCESS_FAILED_COUNT");
                entity.Property(x => x.ConcurrencyStamp)
                    .HasColumnName("CONCURRENCY_STAMP");
                entity.Property(x => x.CreatedAt)
                    .HasColumnName("CREATED_AT");
                entity.Property(x => x.UpdatedAt)
                    .HasColumnName("UPDATED_AT");
                entity.Property(x => x.Email)
                    .HasColumnName("EMAIL");
                entity.Property(x => x.EmailConfirmed)
                    .HasColumnName("EMAIL_CONFIRMED");
                entity.Property(x => x.Name)
                    .HasColumnName("NAME");
                entity.Property(x => x.Status)
                    .HasConversion(new EnumToStringConverter<UserStatus>())
                    .HasColumnName("STATUS");
                entity.Property(x => x.IsLoggedIn)
                    .HasColumnName("IS_LOGGED_IN");
                entity.Property(x => x.LastLoginTime)
                    .HasColumnName("LAST_LOGIN_TIME");
                entity.Property(x => x.LockoutEnabled)
                    .HasColumnName("LOCKOUT_ENABLED");
                entity.Property(x => x.LockoutEnd)
                    .HasColumnName("LOCKOUT_END");
                entity.Property(x => x.NormalizedEmail)
                    .HasColumnName("NORMALIZED_EMAIL");
                entity.Property(x => x.NormalizedUserName)
                    .HasColumnName("NORMALIZED_USER_NAME");
                entity.Property(x => x.PasswordHash)
                    .HasColumnName("PASSWORD_HASH");
                entity.Property(x => x.PhoneNumber)
                    .HasColumnName("PHONE_NUMBER");
                entity.Property(x => x.PhoneNumberConfirmed)
                    .HasColumnName("PHONE_NUMBER_CONFIRMED");
                entity.Property(x => x.SecurityStamp)
                    .HasColumnName("SECURITY_STAMP");
                entity.Property(x => x.TwoFactorEnabled)
                    .HasColumnName("TWO_FACTOR_ENABLED");
                entity.Property(x => x.UserName)
                    .HasColumnName("USER_NAME");

                // Each User can have many UserClaims
                entity.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                entity.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                entity.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                entity.HasMany(e => e.UserRoles)
                    .WithOne(ur => ur.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            }).Model.SetMaxIdentifierLength(30);

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable(name: "ROLE");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id)
                    .HasColumnName("ID");
                entity.Property(x => x.ConcurrencyStamp)
                    .HasColumnName("CONCURRENCY_STAMP");
                entity.Property(x => x.Name)
                    .HasColumnName("NAME");
                entity.Property(x => x.NormalizedName)
                    .HasColumnName("NORMALIZED_NAME");
                entity.HasMany(e => e.UserRoles)
                    .WithOne(ur => ur.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            }).Model.SetMaxIdentifierLength(30);

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.ToTable("USER_ROLES");
                entity.Property(x => x.RoleId)
                    .HasColumnName("ROLE_ID");
                entity.Property(x => x.UserId)
                    .HasColumnName("USER_ID");
            }).Model.SetMaxIdentifierLength(30);


            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("USER_CLAIMS");
                entity.Property(x => x.ClaimType)
                    .HasColumnName("CLAIM_TYPE");
                entity.Property(x => x.ClaimValue)
                    .HasColumnName("CLAIM_VALUE");
                entity.Property(x => x.Id)
                    .HasColumnName("ID");
                entity.Property(x => x.UserId)
                    .HasColumnName("USER_ID");
            }).Model.SetMaxIdentifierLength(30);

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("ROLE_CLAIMS");
                entity.Property(x => x.ClaimType)
                    .HasColumnName("CLAIM_TYPE");
                entity.Property(x => x.ClaimValue)
                    .HasColumnName("CLAIM_VALUE");
                entity.Property(x => x.Id)
                    .HasColumnName("ID");
                entity.Property(x => x.RoleId)
                    .HasColumnName("ROLE_ID");
            }).Model.SetMaxIdentifierLength(30);

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("USER_LOGINS");
                entity.Property(x => x.LoginProvider)
                    .HasColumnName("LOGIN_PROVIDER");
                entity.Property(x => x.ProviderDisplayName)
                    .HasColumnName("PROVIDER_DISPLAY_NAME");
                entity.Property(x => x.ProviderKey)
                    .HasColumnName("PROVIDER_KEY");
                entity.Property(x => x.UserId)
                    .HasColumnName("USER_ID");
            }).Model.SetMaxIdentifierLength(30);

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("USER_TOKENS");
                entity.Property(x => x.LoginProvider)
                    .HasColumnName("LOGIN_PROVIDER");
                entity.Property(x => x.Name)
                    .HasColumnName("NAME");
                entity.Property(x => x.Value)
                    .HasColumnName("VALUE");
                entity.Property(x => x.UserId)
                    .HasColumnName("USER_ID");
            }).Model.SetMaxIdentifierLength(30);
            #endregion Identity Entities

            #region Scaffolded Entities
            modelBuilder.Entity<BankToBrokerFundWalletLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("SYS_C0085666");

                entity.ToTable("BANK_TO_BROKER_FUND_WALLET_LOG", tb => tb.ExcludeFromMigrations());

                entity.HasIndex(e => e.TransactionReference, "UK_HBME2K5L418JJ4RNA4JBL3UJ8").IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Amount)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("AMOUNT");
                entity.Property(e => e.Attempts)
                    .HasPrecision(10)
                    .HasColumnName("ATTEMPTS");
                entity.Property(e => e.CreatedDate)
                    .HasPrecision(6)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ")
                    .HasColumnName("CREATED_DATE");
                entity.Property(e => e.DebitAccountNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("DEBIT_ACCOUNT_NUMBER");
                entity.Property(e => e.JobHostName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("JOB_HOST_NAME");
                entity.Property(e => e.LastModifiedDate)
                    .HasPrecision(6)
                    .HasColumnName("LAST_MODIFIED_DATE");
                entity.Property(e => e.OriginHostName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ORIGIN_HOST_NAME");
                entity.Property(e => e.ResponseCode)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("RESPONSE_CODE");
                entity.Property(e => e.ResponseMessage)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("RESPONSE_MESSAGE");
                entity.Property(e => e.RvslResponseCode)
                    .HasPrecision(10)
                    .HasColumnName("RVSL_RESPONSE_CODE");
                entity.Property(e => e.RvslResponseMessage)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("RVSL_RESPONSE_MESSAGE");
                entity.Property(e => e.Status)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");
                entity.Property(e => e.TransactionReference)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("TRANSACTION_REFERENCE");
                entity.Property(e => e.UserId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USER_ID");
            });

            modelBuilder.Entity<BankToBrokerLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("SYS_C0085569");

                entity.ToTable("BANK_TO_BROKER_LOG", tb => tb.ExcludeFromMigrations());

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ACCOUNT_NUMBER");
                entity.Property(e => e.CreatedDate)
                    .HasPrecision(6)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ")
                    .HasColumnName("CREATED_DATE");
                entity.Property(e => e.UserId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USER_ID");
            });

            modelBuilder.Entity<BankToBrokerWithdrawalLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("SYS_C0085728");

                entity.ToTable("BANK_TO_BROKER_WITHDRAWAL_LOG", tb => tb.ExcludeFromMigrations());

                entity.HasIndex(e => e.TransactionReference, "UK_FHJMOY6SLGTK2ISVAJQVKOU62").IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.Amount)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("AMOUNT");
                entity.Property(e => e.CreatedDate)
                    .HasPrecision(6)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ")
                    .HasColumnName("CREATED_DATE");
                entity.Property(e => e.CreditAccountNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("CREDIT_ACCOUNT_NUMBER");
                entity.Property(e => e.EsbResponse)
                    .HasColumnType("CLOB")
                    .HasColumnName("ESB_RESPONSE");
                entity.Property(e => e.OriginHostName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ORIGIN_HOST_NAME");
                entity.Property(e => e.ResponseCode)
                    .HasPrecision(10)
                    .HasColumnName("RESPONSE_CODE");
                entity.Property(e => e.ResponseMessage)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("RESPONSE_MESSAGE");
                entity.Property(e => e.Status)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");
                entity.Property(e => e.TransactionReference)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("TRANSACTION_REFERENCE");
                entity.Property(e => e.UserId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USER_ID");
            });
            #endregion Scaffolded Entities

            modelBuilder.Seed();
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditMeta();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(
            IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default
        )
        {
            _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                await _currentTransaction?.CommitAsync(cancellationToken)!;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _currentTransaction?.RollbackAsync(cancellationToken)!;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public Task RetryOnExceptionAsync(Func<Task> operation)
        {
            return Database.CreateExecutionStrategy().ExecuteAsync(operation);
        }

        public Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
        {
            return Database.CreateExecutionStrategy().ExecuteAsync(operation);
        }

        public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            var strategy = Database.CreateExecutionStrategy();
            return strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    await action();

                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
        {
            var strategy = Database.CreateExecutionStrategy();
            return strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var result = await action();

                    await transaction.CommitAsync(cancellationToken);

                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        private void AddTimestamp()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is EntityBase<IComparable> && x.State == EntityState.Added);

            foreach (var entity in entities)
            {
                var now = DateTime.Now; // current datetime

                ((EntityBase<IComparable>)entity.Entity).CreatedAt = now;
            }
        }

        private void AddAuditMeta()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is AuditableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "userId")?.Value ?? "internal";
                var now = DateTime.Now; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((AuditableEntity)entity.Entity).CreatedBy = userId;
                }
                if (entity.State == EntityState.Modified)
                {
                    ((AuditableEntity)entity.Entity).UpdatedAt = now;
                    ((AuditableEntity)entity.Entity).UpdatedBy = userId;
                }
            }
        }
    }
}