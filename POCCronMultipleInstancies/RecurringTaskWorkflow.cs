using Elsa.Activities.Temporal;
using Elsa.Builders;
using NodaTime;

namespace POCCronMultipleInstancies
{
    public class RecurringTaskWorkflow : IWorkflow
    {
        private readonly IClock _clock;
        private readonly IDistributedLockManager _distributedLockManager;
        private readonly Guid _process = Guid.NewGuid();
        private readonly string _task;

        public RecurringTaskWorkflow(IClock clock, IDistributedLockManager distributedLockManager)
        {
            _task = "TestTask";
            _clock = clock;
            _distributedLockManager = distributedLockManager;
            _distributedLockManager.ReleaseLockAsync(_task).Wait();
        }

        public string Process => _process.ToString().Substring(30, 6);

        public void Build(IWorkflowBuilder builder) =>
            builder
                .Timer(Duration.FromSeconds(1))
                .Then(() => TryExecuteProcessAsync());

        private async void TryExecuteProcessAsync()
        {
            var locked = await _distributedLockManager.AcquireLockAsync(_task, TimeSpan.FromMinutes(1));
            if (locked)
            {
                Console.WriteLine($"Process [{Process}] Time [{_clock.GetCurrentInstant()}]");
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                await _distributedLockManager.ReleaseLockAsync(_task);
            }
            else
            {
                Console.WriteLine("Process running in another instance...");
            }
        }
    }
}