using System;
using System.Web.Http;
using LaunchDarkly.Client;

namespace FailingWebApi.Controllers
{
    public class TimeController : ApiController
    {
        private readonly LdClient ldClient = new LdClient("sdk-6337c39c-9bd1-4365-9b70-38ce60632d99");

        public CurrentTime Get()
        {
            var user = LaunchDarkly.Client.User.WithKey("bob@example.com");

            bool showFeature = ldClient.BoolVariation("break-dependency", user, false);

            if (showFeature)
            {
                throw new Exception("This api is just bad...");
            }
            else
            {
                return new CurrentTime
                {
                    Now = DateTime.Now.ToLongTimeString(),
                    Provider = "Public api (but sometimes I fail)"
                };
            }
        }
    }

    public class CurrentTime
    {
        public string Now { get; set; }
        public string Provider { get; set; }
    }
}