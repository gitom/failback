using System;
using System.Threading;
using System.Threading.Tasks;
using FailingWebApi;
using MediatR;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;

namespace WhatIsTheTime.Decorators
{
    public class CacheRequestDecorator<TIn, TOut> : IRequestHandler<TIn, TOut> where TIn : IRequest<TOut>
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

            return fallbackPolicy.ExecuteAsync(() => decoratee.Handle(request, cancellationToken));
        }

        static class PolicyFactory
        {
            private static CircuitBreakerPolicy<TOut> circuitBreakerPolicy;

            //TODO: Test this
            public static CircuitBreakerPolicy<TOut> CircuitbreakerPolicy()
            {
                return circuitBreakerPolicy ?? (circuitBreakerPolicy = Policy<TOut>
                           .Handle<Exception>()
                           .CircuitBreakerAsync(2, TimeSpan.FromSeconds(60)));
            }
        }

        private async Task<TOut> GetFromCache(CancellationToken arg)
        {
            var redisDb = RedisConfig.Redis.GetDatabase();
            var cachedValue = await redisDb.StringGetAsync("api");
            var currentTime = JsonConvert.DeserializeObject<TOut>(cachedValue);
            return currentTime;
        }
    }
}