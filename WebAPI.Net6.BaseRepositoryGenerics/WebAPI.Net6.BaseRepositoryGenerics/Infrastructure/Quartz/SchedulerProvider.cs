using Quartz.Impl;
using Quartz;
using static Azure.Core.HttpHeader;
using System.Collections.Specialized;
using WebAPI.Net6.BaseRepositoryGenerics.Infrastructure.Quartz.Job;

namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure.Quartz
{
    public class SchedulerProvider
    {
        public object CreateScheduler()
        {

            var properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "RemoteServerSchedulerClient";

            // set thread pool info
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "20";
            properties["quartz.threadPool.threadPriority"] = "Normal";

            //// set remoting expoter
            //properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
            //properties["quartz.scheduler.exporter.port"] = "555";
            //properties["quartz.scheduler.exporter.bindName"] = "QuartzScheduler";
            //properties["quartz.scheduler.exporter.channelType"] = "tcp";
            //properties["quartz.scheduler.exporter.rejectRemoteRequests"] = "true";

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
            Task<IScheduler> schedulerTask = schedulerFactory.GetScheduler();
            schedulerTask.Wait();
            IScheduler scheduler = schedulerTask.Result;
            scheduler.Start();
            #region [trigger]
            var triggerOneTime = TriggerBuilder.Create()
                .WithIdentity("只触发一次 Trigger", "同步数据")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithRepeatCount(1)
                    .WithInterval(TimeSpan.FromMinutes(1)))
                .Build();


            var trigger5 = TriggerBuilder.Create()
                .WithIdentity("每5分钟触发一次 Trigger", "同步数据")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever())
                .Build();


            var trigger10 = TriggerBuilder.Create()
                .WithIdentity("每10分钟触发一次 Trigger", "同步数据")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(10)
                    .RepeatForever())
                .Build();

            var trigger30 = TriggerBuilder.Create()
                .WithIdentity("每30分钟触发一次 Trigger", "同步数据")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(30)
                    .RepeatForever())
                .Build();

            var trigger60 = TriggerBuilder.Create()
                .WithIdentity("每60分钟触发一次 Trigger", "同步数据")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever())
                .Build();

            var triggerOneDay = TriggerBuilder.Create()
                .WithIdentity("每天触发一次 Trigger", "同步数据")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            var triggerSevenDay = TriggerBuilder.Create()
                .WithIdentity("自动同步存到redis", "同步数据")
                .StartNow()
                 .WithCronSchedule("0 0 23 ? * 1/7") //每天晚上11点开始执行 每7天重复执行一次
                .Build();

            //https://www.freeformatter.com/cron-expression-generator-quartz.html
            var triggerCorn = TriggerBuilder.Create()
                .WithIdentity("测试用", "同步数据")
                .StartNow()
                .WithCronSchedule("0 33 11 * * ?")
                //.WithCronSchedule("5-30 * * ? * * *")
                .Build();

            var triggerCornAt0 = TriggerBuilder.Create()
                .WithIdentity("每天晚上凌晨触发", "test")
                .StartNow()
                .WithCronSchedule("0 0 0 * * ? *")
                .Build();

            var triggerCornAt2 = TriggerBuilder.Create()
                .WithIdentity("每周二四六的凌晨两点触发", "同步数据")
                .StartNow()
                .WithCronSchedule("0 0 2 ? * TUE,THU,SAT *")
                .Build();

            var triggerCornAt25 = TriggerBuilder.Create()
                .WithIdentity("At 02:30:00am every day", "校验")
                .StartNow()
                .WithCronSchedule("0 30 2 ? * * *")
                .Build();

            var triggerCorn2 = TriggerBuilder.Create()
                .WithIdentity("测试用", "同步数据")
                .StartNow()
                .WithCronSchedule("40 01 15 * * ?")
                //.WithCronSchedule("5-30 * * ? * * *")
                .Build();

            var triggerCornAt3 = TriggerBuilder.Create()
                .WithIdentity("在第15分和第45分时触发", "同步数据")
                .StartNow()
                .WithCronSchedule("0 15,45 0 ? * * *")
                .Build();

            var triggerAt822 = TriggerBuilder.Create()
                .WithIdentity("每10分钟触发一次 Trigger", "发送通知")
                .StartNow()
                .WithCronSchedule("0 0/10 8-22 ? * *") // 每隔10分钟触发一次，从早上8点到晚上10点
                .Build();

            var triggerAt226 = TriggerBuilder.Create()
                .WithIdentity("同步redis中的Trigger", "同步数据")
                .StartNow()
                .WithCronSchedule("0 0 22-23,0-6 * * ?") //每天晚上10点开始执行 每天早晨6点结束执行
                .Build();

            var triggerOneTimeStop = TriggerBuilder.Create()
                .WithIdentity("同步映射数据Trigger", "同步数据")
                .StartNow()
                .WithCronSchedule("0 20 22 27 12 ? 2023")
                .Build();

            var triggerOneTimeStops = TriggerBuilder.Create()
                .WithIdentity("同步静态映射数据Trigger", "同步数据")
                .StartNow()
                .WithCronSchedule("0 30 22 27 12 ? 2023")
                .Build();
            #endregion

            #region [Job]
            var testRedisJob = JobBuilder.Create<TestRedisJob>()
                .WithIdentity("Test Redis Job", "sync Redis data")
                .WithDescription("Test sync Redis data。")
                .Build();
            #endregion

            #region [scheduler]
            scheduler.ScheduleJob(testRedisJob, triggerCorn);//测试-redis
            #endregion


            var triggerListener = new TriggerListener();
            scheduler.ListenerManager.AddTriggerListener(triggerListener);

            var schedulerListener = new SchedulerListener();
            scheduler.ListenerManager.AddSchedulerListener(schedulerListener);

            scheduler.StartDelayed(TimeSpan.FromSeconds(30));

            LoggerHelper._logger.Information("Quartz 启动");

            return scheduler;
        }

        /*
		 * Here are just a few overloads to support both v2 and v3 versions on Quartz.
		 * Fill free to avoid these methods.
		 */
        private IScheduler GetScheduler(IScheduler scheduler)
        {
            return scheduler;
        }

        private IScheduler GetScheduler(Task<IScheduler> scheduler)
        {
            return scheduler.Result;
        }
    }
}
