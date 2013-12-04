using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GymGame.Models;
using GymGame.Classes;

namespace GymGame.Controllers
{
    public class LeaderBoardController : Controller
    {

        public ActionResult Index()
        {
            GameModel gm = new GameModel();

            //get the results
            List<CompleteResult> results = new List<CompleteResult>();
            try
            {
                List<Result> res = gm.getAllResults();
                foreach (Result r in res)
                {
                    results.Add(new CompleteResult(r));
                }
                ViewBag.status = "success";
            }
            catch (Exception e)
            {
                ViewBag.status = "error";
            }

            //create a leader per quiz

            List<Leader> leaders = new List<Leader>();
            //select all unique players
            List<int> users = results.Select(r => r.user.User_Id).Distinct().ToList<int>();
            for(int i = 0; i < users.Count(); i++)
            {
                //filter results by user
                List<CompleteResult> crByUser = new List<CompleteResult>();
                UserModel um = new UserModel();
                User u = um.getUser(users[i]);
                crByUser = results.Where(r => r.user.User_Id == u.User_Id).ToList<CompleteResult>();

                //filter results by quiz
                List<int> quizzes = results.Select(r => r.quiz.Quiz_Id).Distinct().ToList<int>();
                for (int j = 0; j < quizzes.Count(); j++)
                {
                    List<CompleteResult> crByUserQuiz = new List<CompleteResult>();
                    Quiz q = new Quiz();
                    q.Quiz_Id = quizzes[j];
                    q = gm.getQuiz(q);
                    crByUserQuiz = results.Where(x => x.quiz.Quiz_Id == q.Quiz_Id).ToList<CompleteResult>();

                    //calculate the values
                    int rightAnswers = 0;
                    int questions = crByUserQuiz.Select(r => r.question.Question_Id).Distinct().Count();
                    foreach (CompleteResult cr in crByUserQuiz)
                    {
                        if (cr.answer.Answer_value == 1) rightAnswers++;
                    }
                    int score = (int)Math.Round(((double)rightAnswers / questions) * 100);

                    //create the leader
                    Leader l = new Leader();
                    l.score = score;
                    l.goodAnswers = rightAnswers;
                    l.questions = questions;
                    l.user = u;
                    l.quiz = q;

                    //add the leader to list
                    leaders.Add(l);
                }
            }

            leaders = leaders.OrderBy(l => l.quiz.Quiz_Id).ThenBy(l => l.score).ToList<Leader>();
            return View(leaders);
        }

        public ActionResult Quiz(String id)
        {
            GameModel gm = new GameModel();
            Quiz quiz = new Quiz();
            //kijk of de quiz gegeven is, anders gaan we de status aanpassen naar "no quiz given"
            int quizId;
            try
            {
                quizId = int.Parse(id);
                try
                {
                    quiz.Quiz_Id = quizId;
                    quiz = gm.getQuiz(quiz);
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
                    quiz.code = id;
                    quiz = gm.getQuizByCode(quiz);
                    ViewBag.status = "success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //probeer op naam te zoeken (kleine kans, maar vergroot UX)
                    try
                    {
                        quiz.name = id;
                        quiz = gm.getQuizByName(quiz);
                        ViewBag.status = "success";
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                        ViewBag.status = "no quiz given";
                    }
                }
            }

            //get the results
            List<CompleteResult> results = new List<CompleteResult>();
            try
            {
                List<Result> resultsByQuiz = gm.getAllResultsByQuiz(quiz);
                foreach (Result r in resultsByQuiz)
                {
                    results.Add(new CompleteResult(r));
                }
                ViewBag.status = "success";
            }
            catch (Exception e)
            {
                ViewBag.status = "error";
            }

            List<Leader> leaders = new List<Leader>();
            //select all unique players
            List<int> users = results.Select(r => r.user.User_Id).Distinct().ToList<int>();
            for (int i = 0; i < users.Count(); i++)
            {
                //filter results by current user
                List<CompleteResult> crByUser = new List<CompleteResult>();
                UserModel um = new UserModel();
                User u = um.getUser(users[i]);
                crByUser = results.Where(r => r.user.User_Id == u.User_Id).ToList<CompleteResult>();
                
                //calculate the values
                int rightAnswers = 0;
                int questions = crByUser.Select(r => r.question.Question_Id).Distinct().Count();
                foreach (CompleteResult cr in crByUser)
                {
                    if (cr.answer.Answer_value == 1) rightAnswers++;
                }
                int score = (int)Math.Round(((double)rightAnswers / questions) * 100);

                //create the leader
                Leader l = new Leader();
                l.score = score;
                l.goodAnswers = rightAnswers;
                l.questions = questions;
                l.user = u;
                l.quiz = quiz;

                //add the leader to list
                leaders.Add(l);

            }

            leaders = leaders.OrderByDescending(l => l.score).ToList<Leader>();
            return View(leaders);
        }

    }
}
