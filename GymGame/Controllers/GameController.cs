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

        public ActionResult Index(String Id)
        {
            

            // Op basis van wat er werd ingegeven achter Game/... wordt er een bepaalde quiz geselecteerd.
            // checken of de gebruiker al is ingelogd. Indien niet -> /login -> 
            ViewBag.id = Id;
            // is er een user sessie? En wie is de gebruiker?
            ViewBag.username = "Willy"; // test Willy hardcoded.
            //test for playable quizzes... ----**!
            GameModel gm = new GameModel();
            PlayableQuiz playQuiz =  gm.getPlayableQuiz(1);
            //end test... ----**!
            return View();
        }


        public void StartGame() 
        {
            
        }

    

    }
}
