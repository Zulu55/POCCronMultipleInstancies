using Quartz;

namespace POCCronMultipleInstancies
{
    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Delay(1000);
            Console.WriteLine($"[{context.JobDetail.ToString()!.Substring(11, 73)}] RUN");
        }
    }
}