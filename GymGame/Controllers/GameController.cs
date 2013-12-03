using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GymGame.Classes;
using GymGame.Models;

namespace GymGame.Controllers
{
    public class GameController : Controller
    {
       
        // GET: /Game/

        public ActionResult Index(String id)
        {
            

            // Op basis van wat er werd ingegeven achter Game/... wordt er een bepaalde quiz geselecteerd.
            // checken of de gebruiker al is ingelogd. Indien niet -> /login -> 
            if (Session["userId"] == null)
            {
                //gebruiker is niet ingelogd: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }
            ViewBag.id = id;
            // is er een user sessie? En wie is de gebruiker?
            ViewBag.username = "Willy"; // test Willy hardcoded.
            //test for playable quizzes... ----**!
            GameModel gm = new GameModel();
            PlayableQuiz playQuiz = new PlayableQuiz();
            //kijk of de quiz gegeven is, anders gaan we de status aanpassen naar "no quiz given"
            int quizId;
            try
            {
                quizId = int.Parse(id);
                try
                {
                    playQuiz = gm.getPlayableQuiz(quizId);
                    ViewBag.status = "success";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ViewBag.status = "no quiz given";
                }
            }
            catch (Exception e)
            {
                //als we hier terechtkomen is de id een textuele string (bv. "abc")
                try
                {
                    playQuiz = gm.getPlayableQuizByName(id);
                    ViewBag.status = "success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    ViewBag.status = "no quiz given";
                } 
            }

            return View(playQuiz);
        }


        public void StartGame() 
        {
            
        }

        [HttpGet]
        public ActionResult Search(String quizCode)
        {
            GameModel gm = new GameModel();
            Quiz result = new Quiz();
            result.name = quizCode;

            result = gm.getQuizByName(result);

            if (result != null)
            {
                ViewBag.foundQuiz = result.name;
                ViewBag.quizUrl = result.name;
            }
            else
            {
                ViewBag.foundQuiz = "We Couldn't find that quiz :(";
                ViewBag.quizUrl = "#";
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveAnswer(FormCollection f)
        {
            GameModel gm = new GameModel();
            Result result = new Result();

            String status = "error";

            try
            {
                if (f["quizId"] != null)
                {
                    result.FK_Quiz = int.Parse(f["quizId"]);
                }
                else
                {
                    throw new Exception("quizId is nog given");
                }
                if (f["questionId"] != null)
                {
                    result.FK_Question = int.Parse(f["questionId"]);
                }
                else
                {
                    throw new Exception("questionId is nog given");
                }
                if (f["answerId"] != null)
                {
                    result.FK_Answer = int.Parse(f["answerId"]);
                }
                else
                {
                    throw new Exception("answerId is nog given");
                }

                if (Session["userId"] != null)
                {
                    result.FK_User = (int)Session["userId"];
                    //result.FK_User = int.Parse((string)Session["userId"]);
                }
                else
                {
                    //gebruiker is niet ingelogd: doorverwijzen naar account/login
                    Response.Redirect("~/account/login");
                }

                int resultId = gm.InsertResult(result);
                if (resultId > 0)
                {
                    //als we een id terug krijgen, is de post gelukt :)
                    status = "success";
                }
            }
            catch (Exception e)
            {
                status = e.Message;
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

    

    }
}
