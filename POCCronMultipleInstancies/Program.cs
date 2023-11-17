using Elsa;
using Elsa.Activities.Console;
using Elsa.Activities.Temporal;
using Elsa.Builders;
using Elsa.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var dbConnectionString = "Data Source=.;Initial Catalog=POC;User ID=sa;Password=Roger1974.;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        var redisConnectionString = "zulutest.redis.cache.windows.net:6380,password=OWuJzbESK5HFAE5jMPLxnVk1s9oxUKYQkAzCaAqTIao=,ssl=True,abortConnect=False";

        var host = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddRedis(redisConnectionString);
                services.AddElsa(elsa => elsa
                        .AddConsoleActivities()
                        .AddQuartzTemporalActivities()
                        .AddWorkflow<RecurringTaskWorkflow>()
                        .ConfigureDistributedLockProvider(options => options.UseSqlServerLockProvider(dbConnectionString))
                        .UseRedisCacheSignal());
            })
            .UseConsoleLifetime()
            .Build();

        using (host)
        {
            await host.StartAsync();
            await host.WaitForShutdownAsync();
        }
    }
}

public class RecurringTaskWorkflow : IWorkflow
{
    private readonly IClock _clock;

    private readonly Guid _process = Guid.NewGuid();

    public RecurringTaskWorkflow(IClock clock) => _clock = clock;

    public string Process => _process.ToString().Substring(30, 6);

    public void Build(IWorkflowBuilder builder) =>
        builder
            .Timer(Duration.FromSeconds(1))
            .WriteLine(() => $"Process [{Process}] Time [{_clock.GetCurrentInstant()}]");
}