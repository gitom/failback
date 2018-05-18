using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using FailingWebApi;
using MediatR;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using RestSharp;
using WhatIsTheTime.Requests;

namespace WhatIsTheTime.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator mediator;

        public HomeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<ActionResult> Index()
        {
            var currentTime = await mediator.Send(new GetTimeRequest());

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