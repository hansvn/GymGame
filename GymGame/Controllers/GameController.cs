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

    

    }
}
