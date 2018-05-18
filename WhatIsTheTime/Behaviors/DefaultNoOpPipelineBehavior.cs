using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace WhatIsTheTime.Behaviors
{
    public class DefaultNoOpPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            return next();
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            return next();
        }
    }
}
