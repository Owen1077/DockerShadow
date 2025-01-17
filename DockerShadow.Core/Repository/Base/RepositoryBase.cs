using DockerShadow.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DockerShadow.Core.Repository.Base
{
    public class RepositoryBase
    {
        public RepositoryBase(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected IServiceScopeFactory _serviceScopeFactory;

        public ApplicationDbContext GetDatabaseContext(IServiceScope serviceScope)
        {
            return serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public void ClearChangeTracking()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                dbContext.ChangeTracker.Clear();
            }
        }
    }
}
