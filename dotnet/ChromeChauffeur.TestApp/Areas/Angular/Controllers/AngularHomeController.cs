using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using ChromeChauffeur.TestApp.Areas.Angular.Models;

namespace ChromeChauffeur.TestApp.Areas.Angular.Controllers
{
    public class AngularHomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}