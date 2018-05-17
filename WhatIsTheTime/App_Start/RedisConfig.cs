using StackExchange.Redis;

namespace FailingWebApi
{
    public class RedisConfig
    {
        public static ConnectionMultiplexer Redis;

        public static void RegisterRedis()
        {
            Redis = ConnectionMultiplexer.Connect("failback.redis.cache.windows.net:6380,password=eSbmdpMgw2QBU7lwymhPoep6x9zkT4DjZn+13c/KMHM=,ssl=True,abortConnect=False");
        }
    }
}