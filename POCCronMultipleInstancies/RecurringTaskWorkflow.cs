using Elsa.Activities.Console;
using Elsa.Activities.Temporal;
using Elsa.Builders;
using NodaTime;

namespace POCCronMultipleInstancies
{
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
}