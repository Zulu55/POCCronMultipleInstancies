using Elsa;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POCCronMultipleInstancies;

internal class Program
{
    private static string _task = null!;

    private static async Task Main(string[] args)
    {
        _task = "TestTask";
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

        Console.CancelKeyPress += new ConsoleCancelEventHandler(CtrlCPressed);
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

        using (host)
        {
            await host.StartAsync();
            await host.WaitForShutdownAsync();
        }
    }

    private static void OnProcessExit(object? sender, EventArgs e)
    {
        CleanLockAsync().Wait();
    }

    private static void CtrlCPressed(object? sender, ConsoleCancelEventArgs e)
    {
        CleanLockAsync().Wait();
    }

    private static async Task CleanLockAsync()
    {
        var distributedLockManager = new DistributedLockManager();
        await distributedLockManager.ReleaseLockAsync(_task);
    }
}