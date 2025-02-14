﻿using DockerShadow.Core.Contract.Repository;
using DockerShadow.Core.Repository.Base;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.Entities;
using DockerShadow.Domain.Entities.Base;
using DockerShadow.Domain.Enum;
using Microsoft.Extensions.DependencyInjection;

namespace DockerShadow.Core.Repository
{
    public class AuditLogRepository : Repository<AuditLog, string>, IAuditLogRepository
    {
        public AuditLogRepository(IServiceScopeFactory serviceScopeFactory)
            : base(serviceScopeFactory, (context) => context.Set<AuditLog>())
        {
        }

        public async Task InsertAuditLog<T>(T oldEntity, T newEntity, AuditEventType eventType, string userId) where T : AuditableEntity
        {
            string oldEntityJson = CoreHelpers.ClassToJsonData(oldEntity);
            string newEntityJson = CoreHelpers.ClassToJsonData(newEntity);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                var _entities = _getDbSet(dbContext);

                var auditLog = new AuditLog()
                {
                    UserId = userId,
                    Property = typeof(T).Name,
                    EventType = eventType,
                    OriginalValue = oldEntityJson,
                    CurrentValue = newEntityJson
                };

                await _entities.AddAsync(auditLog);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
