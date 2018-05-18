using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using WhatIsTheTime.Requests;

namespace WhatIsTheTime.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator mediator;

        public HomeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<ActionResult> Index()
        {
            var currentTime = await mediator.Send(new GetTimeRequest());

            return View(currentTime);
        }
    }
}