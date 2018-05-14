using System;
using System.Web.Http;

namespace FailingWebApi.Controllers
{
    public class TimeController : ApiController
    {
        public CurrentTime Get()
        {
            var currentTime = new CurrentTime
            {
                Now = DateTime.Now.ToLongTimeString(),
                Provider = "Public api (but sometimes I fail)"
            };
            return currentTime;
        }
    }

    public class CurrentTime
    {
        public string Now { get; set; }
        public string Provider { get; set; }
    }
}
