using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using WhatIsTheTime.Hubs;

namespace WhatIsTheTime.Controllers
{
    public class PolyDashboardController : Controller
    {
        public async Task<ActionResult> Index()
        {
            return View();
        }

        public async Task<ActionResult> IndexTest()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<CircuitbreakerHub>();
            context.Clients.All.circuitstatechange("test", "test");

            return View("Index");
        }
    }
}