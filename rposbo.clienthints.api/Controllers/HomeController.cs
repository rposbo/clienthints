using System.Web.Mvc;

namespace rposbo.clienthints.api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}