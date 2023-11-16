using Elsa;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POCCronMultipleInstancies;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IDistributedLockManager, DistributedLockManager>();

                services.AddElsa(elsa => elsa
                        .AddConsoleActivities()
                        .AddQuartzTemporalActivities()
                        .AddWorkflow<RecurringTaskWorkflow>());
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