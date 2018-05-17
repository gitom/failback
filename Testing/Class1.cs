using System;
using System.Threading.Tasks;
using FailingWebApi;
using FluentAssertions;
using RestSharp;
using StackExchange.Redis;
using WhatIsTheTime.Controllers;
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

        [Fact]
        public async Task TestRestClientSerialization()
        {
            var restClient = new RestClient("http://failingwebapi.azurewebsites.net");
            var restRequest = new RestRequest("api/time");
            var serializer = restRequest.JsonSerializer;
            IRestResponse<CurrentTime> currentTimeResponse = await restClient.ExecuteGetTaskAsync<CurrentTime>(restRequest);

            var redis = ConnectionMultiplexer.Connect("failback.redis.cache.windows.net:6380,password=eSbmdpMgw2QBU7lwymhPoep6x9zkT4DjZn+13c/KMHM=,ssl=True,abortConnect=False");
            var redisDb = redis.GetDatabase();
            var redisKey = "api";
            var redisValue = serializer.Serialize(currentTimeResponse.Data);
            await redisDb.StringSetAsync(redisKey, redisValue, TimeSpan.FromSeconds(10));

            var response = await redisDb.StringGetAsync("api");
            response.Should().Be(redisValue);
        }
    }
}
