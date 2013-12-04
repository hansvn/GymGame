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

        // later uitwerken.
        public String Delete(String quiz_Id)
        {
            // uit datadank verwijderen, maar hoe? nog te voorzien.
           
            return quiz_Id + " uit de databank verwijderd" + "<a href='/Quizmaster/manage'> terug </a>";
        }


        public ActionResult Start(String id)
        {
            GameModel gm = new GameModel();
            Quiz quiz = selectQuiz(id);
            List<Round> rounds = gm.getAllRounds(quiz);

            ViewBag.quizName = quiz.name;

            return View(rounds);
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
