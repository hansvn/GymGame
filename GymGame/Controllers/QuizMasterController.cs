﻿using System;
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
            //eerst kijken of de gebruiker de juiste rechten heeft
            User u = new User();
            if (Session["userId"] != null || (int)Session["userLevel"] < 2)
            {
                u.User_Id = (int)Session["userId"];
            }
            else
            {
                //gebruiker is niet ingelogd: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }

            // post alle velden uitlezen
            QuizMasterModel qmm = new QuizMasterModel();
            Quiz quiz = new Quiz();

            quiz.name = f["quizName"];
            // code afleiden van de naam. Afkappen op max 6 tekens.
            quiz.code = f["quizName"].ToUpper();
            quiz.Date = DateTime.Now;
            quiz.Location = f["gameLocation"];
            quiz.active = 1;
            if (Session["userId"] != null)
            {
                quiz.FK_Users = (int)Session["userId"];
            }
            else
            {
                //gebruiker is niet ingelogd: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }
            var quizId = qmm.insertQuiz(quiz);

            // Ronde wordt voorlopig hardcoded aangemaakt.
            // // DOTO: nog Round veldenvoorzien in view.
            Round round = new Round();
            round.FK_Quiz = quizId;
            round.Max_Time = 15;
            round.Round_name = "Ronde 1";
            var roundId = qmm.insertRound(round);
            
            //vragen doorlopen
            Boolean keepRunning = true;
            int q = 1;
            while (keepRunning)
            {
                try
                {
                    String q_text = f["question_" + q];
                    Console.WriteLine("question text: " + q_text);
                    if (q_text.Equals(null))
                    {
                        throw new Exception("Er zijn geen vragen meer om opt te slaan");
                    }
                }
                catch (Exception e)
                {
                    //dit betekend dat we de laatste vraag hebben gehad...
                    keepRunning = false;
                    break;
                }
                try
                {
                    Question question = new Question();
                    question.Question_Text = f["question_" + q];
                    question.FK_Round = roundId;
                    int q_id = qmm.insertQuestion(question);

                    //over antwoorden loopen (q = vraag waar we momenteel aan bezig zijn
                    for (int i = (((q - 1) * 3) + 1); i < ((((q - 1) * 3) + 1) + 3); i++)
                    {
                        Answer answer = new Answer();
                        answer.Answer_Text = f["answer_" + i];
                        answer.FK_Question = q_id;
                        String answerValue = f["correctAnswer_" + i];
                        if (answerValue != null && answerValue.Equals("on"))
                        {
                            answer.Answer_value = 1; 
                        }
                        else
                        { 
                            answer.Answer_value = 0; 
                        }
                        int checkInt = qmm.insertAnswer(answer);
                    }
                    ViewBag.status = "success";
                }
                catch (Exception e)
                {
                    //nu hebben we echt een fout...
                    ViewBag.status = "er liep helaas iets fout...";
                }

                //count
                q++;
                
            }

            return View();
        }

        public ActionResult Manage()
        {
            // user komt normaal gezien uit de sessievar.
            User u = new User();
            if (Session["userId"] != null || (int)Session["userLevel"] < 2)
            {
                u.User_Id = (int)Session["userId"];
            }
            else
            {
                //gebruiker is niet ingelogd: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }
            QuizMasterModel qmm = new QuizMasterModel();
            List<Quiz> Allquizzes = qmm.getAllQuizzes(u);
           
            return View(Allquizzes);
        }

        public ActionResult Edit(String id)
        {
            // alle velden van een bepaalde quiz opvragen.
            // en de velden hiermee vullen.
            if (Session["userId"] == null || (int)Session["userLevel"] < 2)
            {
                //gebruiker is niet ingelogd of heeft geen rechten: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }

            PlayableQuiz plQuiz = new PlayableQuiz();
            //kijk of de quiz gegeven is, anders gaan we de status aanpassen naar "no quiz given"
            int quizId;
            try
            {
                quizId = int.Parse(id);
                try
                {
                    plQuiz = new PlayableQuiz(quizId);
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
                //als we hier terechtkomen is de id een code (bv. "abc")
                try
                {
                    plQuiz = new PlayableQuiz(id);
                    ViewBag.status = "success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    ViewBag.status = "no quiz given";
                }
            }

            ViewBag.name = plQuiz.quiz.name;
            ViewBag.countRounds = plQuiz.quiz.Rounds.Count;
            
            return View(plQuiz);
        }

        [HttpPost]
        public ActionResult Save()
        {
            //kijk of gebruiker de juiste rechten heeft
            User u = new User();
            if (Session["userId"] != null || (int)Session["userLevel"] < 2)
            {
                u.User_Id = (int)Session["userId"];
            }
            else
            {
                //gebruiker is niet ingelogd: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }

            // al de data uit het form halen en indien nodig updaten in db.
            //in eerste fase alle data updaten in db.
            var form = Request.Form.AllKeys;

            foreach (var key in Request.Form.Keys)
            {
                
            }

            return View();
        }

        public ActionResult Delete(String id)
        {
            //kijk of gebruiker de jusite rechten heeft
            User u = new User();
            if (Session["userId"] != null || (int)Session["userLevel"] < 2)
            {
                u.User_Id = (int)Session["userId"];
            }
            else
            {
                //gebruiker is niet ingelogd: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }

            QuizMasterModel qmm = new QuizMasterModel();

            Quiz q = new Quiz();
            try
            {
                q.Quiz_Id = int.Parse(id);
                q.active = 0;
                try
                {
                    ViewBag.quizId = qmm.updateQuiz(q);
                    ViewBag.status = "Quiz werd verwijderd";
                    ViewBag.statusCode = 1;
                }
                catch (Exception e)
                {
                    ViewBag.status = "Quiz kon niet verwijderd worden";
                    ViewBag.statusCode = 0;
                }
            }
            catch (Exception e)
            {
                ViewBag.status = "Quiz kon niet verwijderd worden - foute parameter";
                ViewBag.statusCode = 0;
            }
           
            return View();
        }

        public ActionResult Enable(String id)
        {
            //kijk of gebruiker de jusite rechten heeft
            User u = new User();
            if (Session["userId"] != null || (int)Session["userLevel"] < 2)
            {
                u.User_Id = (int)Session["userId"];
            }
            else
            {
                //gebruiker is niet ingelogd: doorverwijzen naar account/login
                Response.Redirect("~/account/login");
            }

            QuizMasterModel qmm = new QuizMasterModel();

            Quiz q = new Quiz();
            try
            {
                q.Quiz_Id = int.Parse(id);
                q.active = 1;
                try
                {
                    ViewBag.quizId = qmm.updateQuiz(q);
                    ViewBag.status = "Quiz werd terug gezet";
                }
                catch (Exception e)
                {
                    ViewBag.status = "Quiz kon niet terug gehaald worden";
                }
            }
            catch (Exception e)
            {
                ViewBag.status = "Quiz kon niet terug gehaald worden - foute parameter";
            }

            return View();
        }

        public ActionResult Start(String id)
        {
            User u = new User();
            //check if user has the rights
            if (Session["userId"] != null || (int)Session["userLevel"] < 2)
            {
                u.User_Id = (int)Session["userId"];
            }
            else { Response.Redirect("~/account/login"); }
            
            //if id is null
            if (id == null)
            {
                QuizMasterModel qmm = new QuizMasterModel();
                List<Quiz> Allquizzes = qmm.getAllQuizzes(u);

                ViewBag.links = Allquizzes;
                ViewBag.mode = "blank";

                return View();
            }


            //else give the rounds:
            else
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
                    ViewBag.mode = "given";
                    ViewBag.status = "success";
                }
                else { ViewBag.status = "error"; }

                return View(rounds);
            }
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
