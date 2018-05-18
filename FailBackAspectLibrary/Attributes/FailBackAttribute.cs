//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Polly;
//using Polly.CircuitBreaker;
//using PostSharp.Aspects;
//using PostSharp.Extensibility;

//namespace FailBackAspectLibrary.Attributes
//{
//    public static class PolicyFactory
//    {
//        private static CircuitBreakerPolicy<object> circuitBreakerPolicy;

//        public static CircuitBreakerPolicy<object> CircuitbreakerPolicy()
//        {
//            return circuitBreakerPolicy ?? (circuitBreakerPolicy = Policy<object>
//                       .Handle<Exception>()
//                       .CircuitBreakerAsync(2, TimeSpan.FromSeconds(60)));
//        }
//    }

//    public class FailBackAttribute : MethodInterceptionAspect
//    {
//        public override void OnInvoke(MethodInterceptionArgs args)
//        {
//            var tmp = args.Method as MethodInfo;
//            var a = tmp.ReturnType;

//            var fallbackPolicy = Policy//Needs a type at compile time
//                .Handle<Exception>()
//                .Fallback(blbla);

//            var fallbackAfterCircuitbreaker = fallbackPolicy.Wrap(PolicyFactory.CircuitbreakerPolicy());

//            fallbackAfterCircuitbreaker.Execute(args.Proceed());

//            //Only populated after args.Proceed();
//            var returnValue = args.ReturnValue;
            
//            //Save to cache
//        }

//        public override async Task OnInvokeAsync(MethodInterceptionArgs args)
//        {
//            var tmp = args.Method as MethodInfo;
//            var a = tmp.ReturnType;

//            var fallbackPolicy = Policy<object>
//                .Handle<Exception>()
//                .FallbackAsync(GetFromCacheAsync);

//            var fallbackAfterCircuitbreaker = fallbackPolicy.WrapAsync(PolicyFactory.CircuitbreakerPolicy());

//            await fallbackAfterCircuitbreaker.ExecuteAsync(args.Proceed());
//        }

//        private T GetFromCache<T>()
//        {
//            return default(T);
//        }

//        private Task<object> GetFromCacheAsync(CancellationToken arg)
//        {
//            return null;
//        }

//        [Serializable]
//        [MulticastAttributeUsage(MulticastTargets.Method)]
//        public class FlowControllerAttribute : MethodLevelAspect, IAspectProvider
//        {
//            public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
//            {
//                MethodInfo method = (MethodInfo)targetElement;

//                Type returnType = method.ReturnType == typeof(void)
//                    ? typeof(object)
//                    : method.ReturnType;

//                IAspect aspect = (IAspect)Activator.CreateInstance(typeof(FailBackAspect<>).MakeGenericType(returnType));

//                yield return new AspectInstance(targetElement, aspect);
//            }
//        }

//        [Serializable]
//        public class FailBackAspect<T> : IOnMethodBoundaryAspect
//        {
//            public void RuntimeInitialize(MethodBase method)
//            {
//            }

//            public void OnEntry(MethodExecutionArgs args)
//            {
//                var fallbackPolicy = Policy<T>
//                .Handle<Exception>()
//                .Fallback(blblblbl);

//                args.FlowBehavior = FlowBehavior.Return;

//                //var fallbackAfterCircuitbreaker = fallbackPolicy.Wrap(PolicyFactory.CircuitbreakerPolicy());

//                //fallbackAfterCircuitbreaker.Execute(args.Proceed());
               
//            }

//            private T blblblbl()
//            {
//                return default(T);
//            }

//            public void OnExit(MethodExecutionArgs args)
//            {
//            }

//            public void OnSuccess(MethodExecutionArgs args)
//            {
//            }

//            public void OnException(MethodExecutionArgs args)
//            {
//            }
//        }
//    }
//}