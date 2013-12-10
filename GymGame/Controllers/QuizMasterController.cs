using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GymGame.Models;
using GymGame.Classes;

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


        public ActionResult Edit(String quiz_Id)
        {
            // alle velden van een bepaalde quiz opvragen.
            // en de velden hiermee vullen.
           
            //**** TO DO //
            // hard coded omdat hij de waarde niet uit de url krijg
            //PlayableQuiz plquiz = new PlayableQuiz(int.Parse(quiz_Id));
           
            var hardId = 1;
            PlayableQuiz plquiz = new PlayableQuiz(hardId);

            ViewBag.name = plquiz.quiz.name;
            ViewBag.countRounds = plquiz.quiz.Rounds.Count;
            

            return View(plquiz);
        }

        [HttpPost]
        public ActionResult Save()
        {
            // al de data uit het form halen en indien nodig updaten in db.
            //in eerste fase alle data updaten in db.
            var form = Request.Form.AllKeys;
             
            
            foreach (var key in Request.Form.Keys)
            {
                
            }

            
            return View();
        }



        // later uitwerken.
        public String Delete(String quiz_Id)
        {
            // uit datadank verwijderen, maar hoe? nog te voorzien. Op inactief zetten.
           
            return quiz_Id + " uit de databank verwijderd" + "<a href='/Quizmaster/manage'> terug </a>";
        }


        public ActionResult Start(String id)
        {
            List<Round> rounds = new List<Round>();

            if (id != null)
            {
                GameModel gm = new GameModel();
                Quiz quiz = selectQuiz(id);
                rounds = gm.getAllRounds(quiz);

                // ronden in sessie zetten, zodat een gebruiker niet via javascript (jquery call) een andere quiz kan starten
                Session["roundsManaging"] = rounds;
                ViewBag.quizName = quiz.name;
                ViewBag.status = "success";
            }
            else { ViewBag.status = "error"; }

            return View(rounds);
        }

        [HttpPost]
        public ActionResult Start(FormCollection f)
        {
            QuizMasterModel qmm = new QuizMasterModel();
            Round round = selectRound(f["roundId"]);

            List<Round> rounds = (List<Round>)Session["roundsManaging"];
            Session.Remove("roundsManaging");

            // return values
            var status = new Dictionary<string, string>{};
            
            try
            {
                //hier testen we al of user effectief rondes mag starten
                Boolean editPermission = false;
                foreach (Round r in rounds)
                {
                    if(r.Round_Id == round.Round_Id) { editPermission = true; }
                }

                if(editPermission)
                {
                    try
                    {
                        //start round (current time is set there)
                        if (f["action"] == "start")
                        {
                            int duration = int.Parse(f["duration"]) * 60;
                            round.Max_Time = int.Parse(f["duration"]) * 60;
                            round = qmm.startRound(round);
                        }
                        else
                        {
                            round = qmm.stopRound(round);
                        }
                        status.Add("status","success");
                    }
                    catch (Exception e)
                    {
                        status.Add("status", "error");
                    }

                    //set additional return values
                    if (f["action"] == "start")
                    {
                        status.Add("current", "Gestart");
                    }
                    else
                    {
                        status.Add("current", "Gestopt");
                    }

                    DateTime maxTime = new DateTime();
                    if (round.Round_started != null) { maxTime = (DateTime)round.Round_started; }
                    if (round.Max_Time != null) { maxTime = maxTime.AddSeconds((double)round.Max_Time); }

                    String timeRemaining = "0";

                    if (round.Round_started <= DateTime.Now && DateTime.Now < maxTime)
                    {
                        TimeSpan timeRem = (TimeSpan)(maxTime - DateTime.Now);
                        timeRemaining = Math.Round(timeRem.TotalSeconds).ToString();
                    }
                    else { timeRemaining = round.Max_Time.ToString(); }

                    status.Add("timeRemaining", timeRemaining);
                }
                else { throw new Exception("Je bent niet gemachtigd om deze quiz te starten..."); }
            }
            catch (Exception e)
            {
                status.Add("status", "error");
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //functies om quizzen of rondes uit db te krijgen
        private Quiz selectQuiz(String id)
        {
            GameModel gm = new GameModel();
            Quiz quiz = new Quiz();
            //kijk of de quiz gegeven is, anders gaan we de status aanpassen naar "no quiz given"
            int quizId;
            String status;
            try
            {
                quizId = int.Parse(id);
                try
                {
                    quiz.Quiz_Id = quizId;
                    quiz = gm.getQuiz(quiz);
                    status = "success";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    status = "no quiz given";
                }
            }
            catch (Exception e)
            {
                //als we hier terechtkomen is de id een textuele string (bv. "abc")
                try
                {
                    quiz.code = id;
                    quiz = gm.getQuizByCode(quiz);
                    status = "success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //probeer op naam te zoeken (kleine kans, maar vergroot UX)
                    try
                    {
                        quiz.name = id;
                        quiz = gm.getQuizByName(quiz);
                        status = "success";
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                        status = "error";
                    }
                }
            }

            if (status != "success")
            {
                return new Quiz();
            }
            else
            {
                return quiz;
            }
        }

        private Round selectRound(String id)
        {
            GameModel gm = new GameModel();
            Round round = new Round();
            //kijk of de quiz gegeven is, anders gaan we de status aanpassen naar "no quiz given"
            int roundId;
            String status;
            try
            {
                roundId = int.Parse(id);
                try
                {
                    round.Round_Id = roundId;
                    round = gm.getRound(round);
                    status = "success";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    status = "no round given";
                }
            }
            catch (Exception e)
            {
                //als we hier terechtkomen is de id een textuele string (bv. "abc")
                try
                {
                    round.Round_name = id;
                    round = gm.getRoundByName(round);
                    status = "success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    status = "no round given";
                }
            }

            if (status != "success")
            {
                return new Round();
            }
            else
            {
                return round;
            }
        }
    }
}
