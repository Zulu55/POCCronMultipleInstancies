using Elsa;
using Elsa.Activities.Console;
using Elsa.Activities.Temporal;
using Elsa.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;

internal class Program
{
    private static async Task Main(string[] args)
    {
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
    }
}

public class RecurringTaskWorkflow : IWorkflow
{
    private readonly IClock _clock;
    private readonly ILockService _lockService; // Servicio de bloqueo personalizado

    private readonly Guid _process = Guid.NewGuid();

    public RecurringTaskWorkflow(IClock clock, ILockService lockService)
    {
        _clock = clock;
        _lockService = lockService;
    }

    public string Process => _process.ToString().Substring(30, 6);

    public void Build(IWorkflowBuilder builder) =>
        builder
            .Timer(Duration.FromSeconds(1))
            .Then(() => TryExecuteProcess());

    private void TryExecuteProcess()
    {
        if (_lockService.TryAcquireLock(Process))
        {
            Console.WriteLine($"Process [{Process}] Time [{_clock.GetCurrentInstant()}]");
            // Ejecutar la lógica del proceso aquí
            _lockService.ReleaseLock(Process);
        }
        else
        {
            Console.WriteLine("El proceso ya está siendo ejecutado por otra instancia.");
        }
    }
}