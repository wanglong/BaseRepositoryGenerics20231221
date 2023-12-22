namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure
{
    public class ConfigHelper
    {
        //public static readonly IConfiguration _configuration;

        public static IConfiguration configuration { get; set; }

        static ConfigHelper()
        {

        }

        /// <summary>
        /// 获取Section的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSectionValue(string key)
        {
            return configuration.GetSection(key).Value;
        }

        /// <summary>
        /// 获取ConnectionStrings下的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnectionString(string key)
        {
            return configuration.GetConnectionString(key);
        }
    }
}
