using System.Web.Mvc;

namespace ChromeChauffeur.TestApp.Areas.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Message(string message)
        {
            ViewBag.Message = message;

            return View();
        }

        public ActionResult FileUpload()
        {
            var message = "ERROR";

            if (Request.Files.Count > 0 && Request.Files[0] != null && Request.Files[0].ContentLength > 0)
            {
                message = "File upload success";
            }

            return RedirectToAction("Message", new { message });
        }
    }
}