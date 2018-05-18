using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FailingWebApi;
using MediatR;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using WhatIsTheTime.Behaviors;
using WhatIsTheTime.Controllers;
using WhatIsTheTime.Decorators;

namespace WhatIsTheTime
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RedisConfig.RegisterRedis();

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            var allAssemblies = new List<Assembly> { typeof(IMediator).GetTypeInfo().Assembly, typeof(HomeController).GetTypeInfo().Assembly };

            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), allAssemblies);
            container.Register(typeof(IRequestHandler<>), allAssemblies);
            container.Collection.Register(typeof(INotificationHandler<>), allAssemblies);

            container.RegisterInstance(new SingleInstanceFactory(container.GetInstance));
            container.RegisterInstance(new MultiInstanceFactory(container.GetAllInstances));

            container.Collection.Register(typeof(IPipelineBehavior<,>), new[] { typeof(DefaultNoOpPipelineBehavior<,>) });

            container.RegisterDecorator(typeof(IRequestHandler<,>), typeof(CacheRequestDecorator<,>));

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IMediator).GetTypeInfo().Assembly;
            yield return typeof(HomeController).GetTypeInfo().Assembly;
        }
    }
}
