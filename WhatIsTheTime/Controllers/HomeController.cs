using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using FailingWebApi;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using RestSharp;

namespace WhatIsTheTime.Controllers
{
    public static class PolicyFactory
    {
        private static CircuitBreakerPolicy<CurrentTime> circuitBreakerPolicy;

        public static CircuitBreakerPolicy<CurrentTime> CircuitbreakerPolicy()
        {
            return circuitBreakerPolicy ?? (circuitBreakerPolicy = Policy<CurrentTime>
                       .Handle<Exception>()
                       .CircuitBreakerAsync(2, TimeSpan.FromSeconds(60)));
        }
    }
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            //var circuitBreakerPolicy = Policy<CurrentTime>
            //    .Handle<Exception>()
            //    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(60));

            var fallbackPolicy = Policy<CurrentTime>
                .Handle<Exception>()
                .FallbackAsync(GetCurrentTimeFromCache);

            var fallbackAfterCircuitbreaker = fallbackPolicy.WrapAsync(PolicyFactory.CircuitbreakerPolicy());

            var currentTime = await fallbackAfterCircuitbreaker.ExecuteAsync(GetCurrentTimeFromExternalService);
            currentTime.CircuitState = PolicyFactory.CircuitbreakerPolicy().CircuitState.ToString();
            return View(currentTime);
        }

        private async Task<CurrentTime> GetCurrentTimeFromExternalService()
        {
            var restClient = new RestClient("http://failingwebapi.azurewebsites.net");
            var restRequest = new RestRequest("api/time");

            var restResponse = await restClient.ExecuteGetTaskAsync<CurrentTime>(restRequest);

            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception();

            var currentTime = restResponse.Data;
            await RedisConfig.Redis.GetDatabase().StringSetAsync("api", JsonConvert.SerializeObject(currentTime));
            return currentTime;
        }


        private async Task<CurrentTime> GetCurrentTimeFromCache(CancellationToken arg)
        {
            var redisDb = RedisConfig.Redis.GetDatabase();
            var cachedValue = await redisDb.StringGetAsync("api");
            var currentTime = JsonConvert.DeserializeObject<CurrentTime>(cachedValue);
            currentTime.Provider = "Cache";
            return currentTime;
        }
    }

    public class CurrentTime
    {
        public string Now { get; set; }
        public string Provider { get; set; }
        public string CircuitState { get; set; }
    }
}