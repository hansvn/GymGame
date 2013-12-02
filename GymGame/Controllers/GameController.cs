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

        public ActionResult Index()
        {
            //test for playable quizzes... ----**!
            GameModel gm = new GameModel();
            PlayableQuiz playQuiz =  gm.getPlayableQuiz(1);
            //end test... ----**!
            return View();
        }

    }
}
