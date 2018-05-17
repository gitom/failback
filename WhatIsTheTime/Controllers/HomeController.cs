using System;
using System.Net;
using System.Web.Mvc;
using FailingWebApi;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using StackExchange.Redis;

namespace WhatIsTheTime.Controllers
{
    public class HomeController : Controller
    {
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var restClient = new RestClient("http://failingwebapi.azurewebsites.net");
            var restRequest = new RestRequest("api/time");

            var currentTimeResponse = await restClient.ExecuteGetTaskAsync<CurrentTime>(restRequest);

            var redisDb = RedisConfig.Redis.GetDatabase();
            CurrentTime currentTime;

            if (currentTimeResponse.StatusCode == HttpStatusCode.OK)
            {
                currentTime = currentTimeResponse.Data;
                await redisDb.StringSetAsync("api", JsonConvert.SerializeObject(currentTime));
            }
            else
            {
                var cachedValue = await redisDb.StringGetAsync("api");
                currentTime = JsonConvert.DeserializeObject<CurrentTime>(cachedValue);
            }

            return View(currentTime);
        }
    }

    public class CurrentTime
    {
        public string Now { get; set; }
        public string Provider { get; set; }
    }
}