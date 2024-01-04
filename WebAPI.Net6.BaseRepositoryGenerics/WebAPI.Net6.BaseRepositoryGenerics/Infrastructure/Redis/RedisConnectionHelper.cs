using StackExchange.Redis;
using System.Collections.Concurrent;

namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure.Redis
{
    public class RedisConnectionHelper
    {
        // 使用Lazy实现线程安全的单例模式
        private static readonly Lazy<ConnectionMultiplexer> LazyInstance = new Lazy<ConnectionMultiplexer>(GetManager);

        // 获取Redis连接字符串配置
        private static string RedisConnectionString { get; } = ConfigHelper.GetSectionValue("MyCSRedisConnString");

        // 缓存连接多路复用器
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        // 获取单例连接多路复用器
        public static ConnectionMultiplexer Instance => LazyInstance.Value;

        // 获取或创建具有指定连接字符串的连接多路复用器并将其添加到缓存中
        public static ConnectionMultiplexer GetConnectionMultiplexer(string connectionString)
        {
            // 检查连接字符串是否为空
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            // 缓存中存在连接多路复用器则返回它，否则创建一个新的连接多路复用器并将其添加到缓存中
            return ConnectionCache.GetOrAdd(connectionString, CreateConnectionMultiplexer);
        }

        // 从缓存中移除所有连接多路复用器，或者移除指定连接字符串的连接多路复用器
        public static void ClearCache(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                ConnectionCache.Clear();
            }
            else
            {
                ConnectionMultiplexer multiplexer;
                ConnectionCache.TryRemove(connectionString, out multiplexer);
                multiplexer?.Close();
            }
        }

        // 获取一个连接多路复用器
        private static ConnectionMultiplexer GetManager() => ConnectionMultiplexer.Connect(RedisConnectionString);

        // 创建连接多路复用器并注册事件
        private static ConnectionMultiplexer CreateConnectionMultiplexer(string connectionString)
        {
            var connect = ConnectionMultiplexer.Connect(connectionString);

            // 注册连接状态及错误相关的事件处理程序
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;

            return connect;
        }

        #region 事件处理程序

        // 配置更改时
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.WriteLine("Configuration changed: " + e.EndPoint);
        }

        // 发生错误时
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine("ErrorMessage: " + e.Message);
        }

        // 重新建立连接之前的错误
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("ConnectionRestored: " + e.EndPoint);
        }

        // 连接失败 ， 如果重新连接成功你将不会收到这个通知
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("Reconnecting: Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));

            var multiplexer = (ConnectionMultiplexer)sender;
            multiplexer.Dispose();
            ConnectionCache.TryRemove(multiplexer.Configuration, out multiplexer);
            CreateConnectionMultiplexer(multiplexer.Configuration);
        }

        // 更改集群
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        // redis类库错误
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine("InternalError:Message" + e.Exception.Message);
        }

        #endregion
    }
}
