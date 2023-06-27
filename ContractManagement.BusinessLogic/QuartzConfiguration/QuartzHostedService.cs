using ContractManagement.BusinessLogic.ClasseQuartz;
using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractManagement.BusinessLogic.QuartzConfiguration
{
    public  class QuartzHostedService : IHostedService
    {
        private readonly IScheduler _scheduler;
        private readonly IEnumerable<JobSchedule> _jobSchedules;
        public QuartzHostedService(IScheduler scheduler , IEnumerable<JobSchedule> jobSchedules)
        {
            _scheduler = scheduler;
            _jobSchedules = jobSchedules;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var jobSchedule in _jobSchedules)
            {
                var job = JobBuilder.Create(jobSchedule._JobType)
                    .WithIdentity(jobSchedule._JobType.FullName)
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"{jobSchedule._JobType.FullName}.trigger")
                    .WithCronSchedule(jobSchedule._CronExpression) .Build();
                _scheduler.ScheduleJob(job, trigger);

            }
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _scheduler.Shutdown();
        }
    }
   
}
