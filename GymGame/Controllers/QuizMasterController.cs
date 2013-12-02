using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GymGame.Models;

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

        [HttpPost]
        public ActionResult New(FormCollection f)
        {
            GymGameModelDataContext dc = new GymGameModelDataContext();
            // uit post halen: gameName, question, a1, a2, a3
            var gamename = f["gameName"];
            var question = f["question"];
            var answer1 = f["a1"];
            var answer2 = f["a2"];
            var answer3 = f["a3"];
            // wegschrijven



            //  Quiz q = new Quiz();
            User u = new User();
            return View();
        }

        public ActionResult Manage()
        {
            GymGameModelDataContext dc = new GymGameModelDataContext();
            // user komt normaal gezien uit de sessievar.
            User u = new User();
            u.User_Id = 1;
            QuizMasterModel qmm = new QuizMasterModel();
            List<Quiz> Allquizzes = qmm.getAllQuizzes(u);
           
            ViewBag.quizzes = Allquizzes;
            return View();
        }


        

        [HttpPost]
        public ActionResult Edit(String quiz_Id)
        {
            // alle velden van een bepaalde quiz opvragen.
            // en de velden hiermee vullen.
            var Quiz = new Quiz { Location = "IMDEVENT" + quiz_Id };
            return View(Quiz);
        }

        public String Delete(String quiz_Id)
        {
            // uit datadank verwijderen, maar hoe?
            return quiz_Id + " uit de databank verwijderd" + "<a href='/Quizmaster/manage'> terug </a>";
        }


        public ActionResult Start()
        {
            // een quiz starten. Timer starten.. 

            return View();
        }
    }
}
