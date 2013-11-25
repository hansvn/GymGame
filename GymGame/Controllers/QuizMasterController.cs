using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GymGame.Controllers
{
    public class QuizMasterController : Controller
    {
        // This controller will handle backend features for administrators.
        // New quizes can be created and managed.
        // GET: /QuizMaster/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }
    }
}
