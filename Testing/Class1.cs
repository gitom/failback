using System.Threading.Tasks;
using FluentAssertions;
using StackExchange.Redis;
using Xunit;

namespace Testing
{
    public class Class1
    {
        [Fact]
        public async Task Test()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("failback.redis.cache.windows.net:6380,password=eSbmdpMgw2QBU7lwymhPoep6x9zkT4DjZn+13c/KMHM=,ssl=True,abortConnect=False");
            IDatabase db = redis.GetDatabase();

            var value = "abcdefg";
            await db.StringSetAsync("mykey", value);
            var newValue = await db.StringGetAsync("mykey");

            newValue.Should().Be(value);
        }
    }
}
