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
            //GymGameModelDataContext dc = new GymGameModelDataContext();

            // Session aanmaken om te makkelijker te testen
            Session["userId"] = 1;


            // post alle velden uitlezen
            // for (var i = 0; i < aantal; i++)
            QuizMasterModel qmm = new QuizMasterModel();
            Quiz quiz = new Quiz();

            quiz.name = f["quizName"];
            // code afleiden van de naam. Afkappen op max 6 tekens.
            quiz.code = f["quizName"];
            quiz.Date = DateTime.Now;
            quiz.Location = f["gameLocation"];
            quiz.active = 1;
            quiz.FK_Users = (int)Session["userId"];
            var quizId = qmm.insertQuiz(quiz);

            // Ronde wordt voorlopig hardcoded aangemaakt.
            // // DOTO: nog Round veldenvoorzien in view.
            Round round = new Round();
            round.FK_Quiz = quizId;
            round.Max_Time = 15;
            round.Round_name = "Ronde 1";
            var roundId = qmm.insertRound(round);
            
            //vragen doorlopen.
            for (var i = 0; i < 1; i++)
            {
                Question question = new Question();
                question.Question_Text = f["question_" + i];
                question.FK_Round = roundId;
                qmm.insertQuestion(question);
            }





            



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
