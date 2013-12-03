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

        public ActionResult Index(String id)
        {

            // Op basis van wat er werd ingegeven achter Game/... wordt er een bepaalde quiz geselecteerd.
            // checken of de gebruiker al is ingelogd. Indien niet -> /login ->
            if (Session["userId"] == null)
            {
                //gebruiker is niet ingelogd: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }

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
                    playQuiz = gm.getPlayableQuizByCode(id);
                    ViewBag.status = "success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //probeer op naam te zoeken (kleine kans, maar vergroot UX)
                    try
                    {
                        playQuiz = gm.getPlayableQuizByName(id);
                        ViewBag.status = "success";
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                        ViewBag.status = "no quiz given";
                    }
                }
            }

            //check if the user already played the quiz
            User u = new User();
            u.User_Id = (int)Session["userId"];
            List<Result> resultsByUser = gm.getResultsByUser(u);
            foreach (Result r in resultsByUser)
            {
                if (r.FK_Quiz == playQuiz.quiz.Quiz_Id)
                {
                    playQuiz.userPlayed = true;
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
                ViewBag.quizUrl = result.code;
            }
            else
            {
                //we konden de quiz niet op de naam vinden, probeer het op code
                result = new Quiz();
                result.code = quizCode;
                result = gm.getQuizByCode(result);

                if (result != null)
                {
                    ViewBag.foundQuiz = result.name;
                    ViewBag.quizUrl = result.code;
                }
                else
                {
                    ViewBag.foundQuiz = "Die quiz hebben we niet gevonden :(";
                    ViewBag.quizUrl = "#";
                }
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

        public ActionResult Results(String id)
        {
            // checken of de gebruiker al is ingelogd. Indien niet -> /login ->
            if (Session["userId"] == null)
            {
                //gebruiker is niet ingelogd: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }

            GameModel gm = new GameModel();
            List<CompleteResult> results = new List<CompleteResult>();
            
            //kijk of de quiz gegeven is, anders gaan we de status aanpassen naar "no quiz given"
            User u = new User();
            u.User_Id = (int)Session["userId"];

            try
            {
                int quizId = int.Parse(id);
                try
                {
                    //haal resultaten op
                    Quiz q = new Quiz();
                    q.Quiz_Id = quizId;
                    List<Result> res = gm.getResultsByUserAndQuiz(u, q);
                    foreach (Result r in res)
                    {
                        results.Add(new CompleteResult(r));
                    }
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
                    //haal resultaten op
                    Quiz q = new Quiz();
                    q.code = id;
                    q = gm.getQuizByCode(q);
                    List<Result> res = gm.getResultsByUserAndQuiz(u, q);
                    foreach (Result r in res)
                    {
                        results.Add(new CompleteResult(r));
                    }
                    
                    ViewBag.status = "success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //probeer op naam te zoeken (kleine kans, maar vergroot UX)
                    try
                    {
                        //haal resultaten op
                        Quiz q = new Quiz();
                        q.name = id;
                        q = gm.getQuizByName(q);
                        List<Result> res = gm.getResultsByUserAndQuiz(u, q);
                        foreach (Result r in res)
                        {
                            results.Add(new CompleteResult(r));
                        }

                        ViewBag.status = "success";
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);

                        ViewBag.status = "no quiz given";
                    }
                }
            }

            //viewbag data:
            //the number of questions
            int questions = results.Count();
            ViewBag.questions = questions;

            //count the right answers
            int rightAnswers = 0;
            foreach (CompleteResult cr in results)
            {
                if (cr.answer.Answer_value == 1) rightAnswers++;
            }
            ViewBag.rightAnswers = rightAnswers;
            ViewBag.percentage = Math.Round(((double)rightAnswers / questions) * 100);

            return View(results);
        }
    

    }
}
