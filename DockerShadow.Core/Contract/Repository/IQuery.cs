using DockerShadow.Persistence;

namespace DockerShadow.Core.Contract.Repository
{
    public interface IQuery<TOut>
    {
        IQueryable<TOut> Run(ApplicationDbContext dbContext);
    }
}
