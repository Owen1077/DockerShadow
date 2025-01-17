using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace DockerShadow.Infrastructure.Interceptor
{
    public class DBLongQueryLogger : DbCommandInterceptor
    {
        private readonly ILogger<DBLongQueryLogger> _logger;
        public DBLongQueryLogger(ILogger<DBLongQueryLogger> logger)
        {
            _logger = logger;
        }
        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug($"DB Query: {command.CommandText}");

            if (eventData.Duration.TotalMilliseconds > 700)
            {
                LogLongQuery(command, eventData);
            }
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            _logger.LogDebug($"DB Query: {command.CommandText}");

            if (eventData.Duration.TotalMilliseconds > 700)
            {
                LogLongQuery(command, eventData);
            }
            return base.ReaderExecuted(command, eventData, result);
        }

        private void LogLongQuery(DbCommand command, CommandExecutedEventData eventData)
        {
            _logger.LogError($"Long query: {command.CommandText}. \nTotalMilliseconds: {eventData.Duration.TotalMilliseconds}ms");
        }
    }
}
