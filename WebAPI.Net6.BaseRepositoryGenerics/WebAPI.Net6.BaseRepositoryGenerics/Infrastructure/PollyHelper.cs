using Polly;
using Polly.Timeout;
using Polly.Wrap;
using WebAPI.Net6.BaseRepositoryGenerics.Extensions;

namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure
{
    /// <summary>
    /// http连接基础类，负责底层的http通信.
    /// </summary>
    public class PollyHelper
    {
        private static ILogger _logger;

        public PollyHelper(ILogger<PollyHelper> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Polly初始化
        /// </summary>
        /// <typeparam name="T">返回对象.</typeparam>
        /// <param name="_policyWrap">Policy实例</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="WaitSeconds">重试等待分钟</param>
        /// <param name="breakCount">熔断次数</param>
        /// <param name="durationOfBreak">断开时间</param>
        public static AsyncPolicyWrap<T> Initialize<T>(AsyncPolicyWrap<T> _policyWrap, int retryCount, int WaitSeconds, int breakCount, int durationOfBreak, string serverName)
        {
            // 调用超时策略
            var timeout = Polly.Policy
            // 超过10秒钟，就设定超时
            .TimeoutAsync(60, TimeoutStrategy.Pessimistic, (context, ts, task) =>
            {
                _logger.LogInformation(serverName + "调用超时");
                return Task.CompletedTask;
            });


            // 调用重试策略
            var retry = Polly.Policy.Handle<Exception>().WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(WaitSeconds),
            // 处理异常、等待时间、重试第几回
            (exception, timespan, retryCount, context) =>
            {
                _logger.LogInformation(serverName + "重试");
            });


            // 调用熔断策略
            var circuitBreakerPolicy = Polly.Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                  exceptionsAllowedBeforeBreaking: breakCount,             // 连续5次异常
                  durationOfBreak: TimeSpan.FromMilliseconds(durationOfBreak),       // 断开1分钟
                  onBreak: (exception, breakDelay) =>
                  {
                      // 熔断以后，需要处理的动作；  记录日志；
                      _logger.LogInformation(serverName + "服务出现=========>熔断");
                  },
                  onReset: () => //// 熔断器关闭时
                  {
                      _logger.LogInformation(serverName + "服务熔断器关闭了");
                  },
                  onHalfOpen: () => // 熔断时间结束时，从断开状态进入半开状态
                  {
                      _logger.LogInformation(serverName + "服务熔断时间到，进入半开状态");
                  });

            _policyWrap = Policy<T>
                .Handle<Exception>()
                .FallbackAsync(AccountServiceFallback<T>(), (x) =>
                {
                    _logger.LogInformation(serverName + "进行了服务降级");
                    return Task.CompletedTask;
                })
                .WrapAsync(retry)
                .WrapAsync(circuitBreakerPolicy)
                .WrapAsync(timeout);
            return _policyWrap;
        }

        private static T AccountServiceFallback<T>()
        {
            return default;
        }
    }
}
