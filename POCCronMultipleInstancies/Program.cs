using Elsa;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POCCronMultipleInstancies;

var host = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services
    .AddElsa(elsa => elsa
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