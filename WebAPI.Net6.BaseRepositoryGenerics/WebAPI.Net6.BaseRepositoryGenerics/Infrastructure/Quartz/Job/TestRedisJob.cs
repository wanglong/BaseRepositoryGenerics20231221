using Newtonsoft.Json;
using Quartz;

namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure.Quartz.Job
{
    public class TestRedisJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            int databaseNumber = 1;
            var redisHelper = new Redis.RedisHelper() { DatabaseNumber = databaseNumber, CustomKeyPrefix = "ERROR_CODE_MAPPING:" };

            redisHelper.ListRightPush("fx_code_1", JsonConvert.SerializeObject(new { code = "fx_code_1", messageCn = "阿斯蒂芬1", messageEn = "aewfa1" }));
            redisHelper.ListRightPush("fx_code_1", JsonConvert.SerializeObject(new { code = "fx_code_1", messageCn = "阿斯蒂芬2", messageEn = "aewfa2" }));
            redisHelper.ListRightPush("fx_code_1", JsonConvert.SerializeObject(new { code = "fx_code_1", messageCn = "阿斯蒂芬3", messageEn = "aewfa3" }));
            redisHelper.ListRightPush("fx_code_2", JsonConvert.SerializeObject(new { code = "fx_code_2", messageCn = "阿斯蒂芬", messageEn = "aewfa" }));

            //List<Task> tasks = new List<Task>();
            //for (int i = 0; i < 1000; i++)
            //{
            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {

            //        string key = Guid.NewGuid().ToString("N");
            //        string value = Guid.NewGuid().ToString("N");

            //        redisHelper.StringSet(key, value, TimeSpan.FromMinutes(10));
            //    }));
            //}
            //Task.WaitAll(tasks.ToArray());

            #region 重复 key

            //添加重复key时无需校验，第二次会自动覆盖第一次
            //redisHelper.StringSet("1",Guid.NewGuid().ToString("N"));
            //Thread.Sleep(1000*60);
            //redisHelper.StringSet("1", Guid.NewGuid().ToString("N"),TimeSpan.FromMinutes(10));

            #endregion

            //#region list

            //string listKey = "list";

            ////redisHelper.ListRightPush(listKey, Guid.NewGuid().ToString("N"));
            ////redisHelper.ListRightPush(listKey, Guid.NewGuid().ToString("N"));
            ////redisHelper.ListRightPush(listKey, Guid.NewGuid().ToString("N"));
            ////redisHelper.ListRightPush(listKey, Guid.NewGuid().ToString("N"));
            ////redisHelper.ListRightPush(listKey, Guid.NewGuid().ToString("N"));
            ////redisHelper.ListRightPush(listKey, Guid.NewGuid().ToString("N"));

            //string value = "";
            //value = redisHelper.ListLeftPop(listKey);
            //value = redisHelper.ListLeftPop(listKey);
            //value = redisHelper.ListLeftPop(listKey);
            //value = redisHelper.ListLeftPop(listKey);
            //value = redisHelper.ListLeftPop(listKey);
            //value = redisHelper.ListLeftPop(listKey);
            ////该取不出来了
            //value = redisHelper.ListLeftPop(listKey);

            //#endregion

            return Task.CompletedTask;
        }
    }
}
