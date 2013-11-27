using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GymGame.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View();
        }

        public string Welcome(String name, int aantal = 0)
        {
            return HttpUtility.HtmlEncode("Hello " + name + ", Je was hier " + aantal + " keer");
        }

    }
}
