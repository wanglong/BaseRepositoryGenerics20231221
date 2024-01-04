using Newtonsoft.Json;
using Quartz;
using static Azure.Core.HttpHeader;

namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure.Quartz
{

    public class TriggerListener : ITriggerListener
    {
        public string Name => "MyTriggerListener";

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            IJobDetail jobDetail = context.JobDetail;
            LoggerHelper._logger.Debug(jobDetail.Key.Name);
            LoggerHelper._logger.Debug(context.JobRunTime.ToString(@"hh\:mm\:ss"));

            if (context.Result != null && "refire".Equals(context.Result.ToString().ToLower()))
            {
                ITrigger refireTrigger = TriggerBuilder.Create().WithIdentity("Refire " + trigger.Key.Name, trigger.Key.Group)
                .StartAt(DateBuilder.FutureDate(5, IntervalUnit.Minute))
                .ForJob(context.JobDetail)
                .UsingJobData(trigger.JobDataMap)
                .Build();
                context.Scheduler.ScheduleJob(refireTrigger);
            }
            return Task.CompletedTask;
        }

        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            IJobDetail jobDetail = context.JobDetail;
            LoggerHelper._logger.Debug(jobDetail.Key.Name);
            LoggerHelper._logger.Debug(string.Format("DataMap:{0}", JsonConvert.SerializeObject(context.Trigger.JobDataMap)));
            return Task.CompletedTask;
        }

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            LoggerHelper._logger.Debug(trigger.Key.Name);
            return Task.CompletedTask;
        }

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}
