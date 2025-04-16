using System.Diagnostics.Metrics;

namespace EmployeeManagement.Api.Metrics
{
    public static class EfCoreMetrics
    {
        private static readonly Meter Meter = new("EmployeeManagementApi.Metrics", "1.0.0");

        public static readonly Histogram<double> DbQueryDuration =
            Meter.CreateHistogram<double>("db_query_duration_ms", "ms", "Duração das queries EF Core");

        public static void RecordDbQuery(double duration)
        {
            Console.WriteLine($"[METRICS] Registrando query: {duration}ms");
            DbQueryDuration.Record(duration);
        }

        public static readonly Counter<int> DbQueryCount =
        Meter.CreateCounter<int>("db_query_count", "queries", "Contagem de queries executadas");

        public static void IncrementQueryCount()
        {
            DbQueryCount.Add(1);
        }

    }

}
