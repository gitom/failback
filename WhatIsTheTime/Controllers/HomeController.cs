using System.Web.Mvc;
using RestSharp;

namespace WhatIsTheTime.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var restClient = new RestClient("http://failingwebapi.azurewebsites.net");
            var currentTime = restClient.Execute<CurrentTime>(new RestRequest("api/time"));
            return View(currentTime.Data);
        }
    }

    public class CurrentTime
    {
        public string Now { get; set; }
        public string Provider { get; set; }
    }
}