using System;
using System.Threading;
using System.Threading.Tasks;
using FailingWebApi;
using MediatR;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using StackExchange.Redis;
using WhatIsTheTime.Hubs;
using WhatIsTheTime.Models;

namespace WhatIsTheTime.Decorators
{
    public class CacheRequestDecorator<TIn, TOut> : IRequestHandler<TIn, TOut> where TIn : IRequest<TOut> where TOut : IProvider
    {
        private readonly IRequestHandler<TIn, TOut> decoratee;

        public CacheRequestDecorator(IRequestHandler<TIn, TOut> decoratee)
        {
            this.decoratee = decoratee;
        }

        public Task<TOut> Handle(TIn request, CancellationToken cancellationToken)
        {
            var fallbackPolicy = Policy<TOut>
                .Handle<Exception>()
                .FallbackAsync(GetFromCache);

            var fallbackAfterCircuitbreaker = fallbackPolicy.WrapAsync(PolicyFactory.CircuitbreakerPolicy());

            return fallbackAfterCircuitbreaker.ExecuteAsync(() => decoratee.Handle(request, cancellationToken));
        }

        static class PolicyFactory
        {
            private static CircuitBreakerPolicy<TOut> circuitBreakerPolicy;

            public static CircuitBreakerPolicy<TOut> CircuitbreakerPolicy()
            {
                return circuitBreakerPolicy ?? (circuitBreakerPolicy = Policy<TOut>
                           .Handle<Exception>()
                           .CircuitBreakerAsync<TOut>(2, TimeSpan.FromSeconds(15), onbreak2, onreset, onHalfOpen));
            }

            private static void onHalfOpen()
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<CircuitbreakerHub>();
                context.Clients.All.circuitstatechange("api circuitbreaker", "halfopen");
            }

            private static void onreset(Context obj)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<CircuitbreakerHub>();
                context.Clients.All.circuitstatechange("api circuitbreaker", "closed");
            }

            private static void onbreak2(DelegateResult<TOut> arg1, TimeSpan arg2, Context arg3)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<CircuitbreakerHub>();
                context.Clients.All.circuitstatechange("api circuitbreaker", "open");
            }
        }

        private async Task<TOut> GetFromCache(CancellationToken arg)
        {
            var redisDb = RedisConfig.Redis.GetDatabase();
            var cachedValue = await redisDb.StringGetAsync("api");
            var currentTime = JsonConvert.DeserializeObject<TOut>(cachedValue);
            currentTime.Provider = "Cache provider";
            return currentTime;
        }
    }
}