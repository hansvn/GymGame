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

        [HttpPost]
        public ActionResult New(FormCollection f)
        {
            string namegame = f["naam"];
            return View();
        }

        public ActionResult Manage()
        {
     
            return View();
        }

        public ActionResult Edit(int id)
        {

            ViewBag.questions = "";
            return View();
        }

        public ActionResult Start()
        {
            return View();
        }
    }
}
