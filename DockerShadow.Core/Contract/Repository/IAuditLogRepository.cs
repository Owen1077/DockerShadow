using DockerShadow.Domain.Entities;
using DockerShadow.Domain.Entities.Base;
using DockerShadow.Domain.Enum;

namespace DockerShadow.Core.Contract.Repository
{
    public interface IAuditLogRepository : IRepository<AuditLog, string>
    {
        Task InsertAuditLog<T>(T oldEntity, T newEntity, AuditEventType eventType, string userId) where T : AuditableEntity;
    }
}
