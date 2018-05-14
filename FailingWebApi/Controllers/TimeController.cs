using System;
using System.Web.Http;
using LaunchDarkly.Client;

namespace FailingWebApi.Controllers
{
    public class TimeController : ApiController
    {
        public CurrentTime Get()
        {
            //LdClient client = new LdClient("sdk-6337c39c-9bd1-4365-9b70-38ce60632d99");
            //User user = LaunchDarkly.Client.User.WithKey("bob@example.com")
            //    .AndFirstName("Bob")
            //    .AndLastName("Loblaw")
            //    .AndCustomAttribute("groups", "beta_testers");

            //var toggle = client.BoolVariation("test", user);
            var currentTime = new CurrentTime
            {
                Now = DateTime.Now.ToLongTimeString(),
                Provider = "Public api (but sometimes I fail)",
                //Toggle = toggle
            };
            return currentTime;
        }
    }

    public class CurrentTime
    {
        public string Now { get; set; }
        public string Provider { get; set; }
        public bool Toggle { get; set; }
    }
}
