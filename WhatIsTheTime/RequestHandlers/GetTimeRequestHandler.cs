using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FailingWebApi;
using MediatR;
using Newtonsoft.Json;
using RestSharp;
using WhatIsTheTime.Models;
using WhatIsTheTime.Requests;

namespace WhatIsTheTime.RequestHandlers
{
    public class GetTimeRequestHandler : IRequestHandler<GetTimeRequest, CurrentTime>
    {
        public Task<CurrentTime> Handle(GetTimeRequest request, CancellationToken cancellationToken)
        {
            return GetCurrentTimeFromExternalService();
        }

        private async Task<CurrentTime> GetCurrentTimeFromExternalService()
        {
            var restClient = new RestClient("http://failingwebapi.azurewebsites.net");
            var restRequest = new RestRequest("api/time");

            var restResponse = await restClient.ExecuteGetTaskAsync<CurrentTime>(restRequest);

            if (restResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception();

            var currentTime = restResponse.Data;
            //TODO: This should be done by the decorator
            await RedisConfig.Redis.GetDatabase().StringSetAsync("api", JsonConvert.SerializeObject(currentTime));
            return currentTime;
        }
    }
}