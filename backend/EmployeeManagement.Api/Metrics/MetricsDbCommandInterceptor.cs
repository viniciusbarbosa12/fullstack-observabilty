using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;


namespace EmployeeManagement.Api.Metrics
{
    public class MetricsDbCommandInterceptor : DbCommandInterceptor
    {
        public override ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            EfCoreMetrics.RecordDbQuery(eventData.Duration.TotalMilliseconds);
            return new ValueTask<DbDataReader>(result);
        }

        public override ValueTask<int> NonQueryExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[EFCORE INTERCEPTOR] Query executada: {command.CommandText}");

            EfCoreMetrics.RecordDbQuery(eventData.Duration.TotalMilliseconds);
            return new ValueTask<int>(result);
        }

        public override ValueTask<object> ScalarExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            object result,
            CancellationToken cancellationToken = default)
        {
            EfCoreMetrics.RecordDbQuery(eventData.Duration.TotalMilliseconds);
            return new ValueTask<object>(result);
        }
    }
}