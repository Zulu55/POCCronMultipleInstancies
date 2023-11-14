using Quartz.Impl;
using Quartz;

namespace POCCronMultipleInstancies
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            ISchedulerFactory schedFactory = new StdSchedulerFactory();

            // Get a scheduler
            IScheduler scheduler = await schedFactory.GetScheduler();
            await scheduler.Start();

            // Define the job and tie it to our HelloJob class
            var jobName = Guid.NewGuid().ToString();
            var groupName = Guid.NewGuid().ToString();

            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity(jobName, groupName)
                .Build();

            // Trigger the job to run now, and then repeat every 5 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(jobName, groupName)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(5)
                    .RepeatForever())
                .Build();

            // Tell the scheduler to execute the job using our trigger
            await scheduler.ScheduleJob(job, trigger);

            var counter = 0;
            do
            {
                await Task.Delay(1000);
                Console.WriteLine($"[{groupName}.{jobName}] Second: {++counter}");
            } while (true);
        }
    }
}