using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WhatIsTheTime.StartUp))]

namespace WhatIsTheTime
{
    public class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}