using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GymGame.Models;

namespace GymGame.Classes
{
    public class PlayableRound
    {
        public Round round = new Round();
        public Boolean isActive = false;
        public List<PlayableQuestion> playableQuestions = new List<PlayableQuestion>();
        GameModel gm = new GameModel();

        //constructor
        public PlayableRound() { }

        public PlayableRound(Round rnd)
        {
            //select the round
            round = rnd;
            if (round.Round_started != null)
            {
                DateTime maxTime = (DateTime)round.Round_started;
                maxTime = maxTime.AddSeconds((double)round.Max_Time);
                DateTime currentTime = DateTime.Now;
                isActive = (round.Round_started <= currentTime && currentTime < maxTime);
            }

            //get the questions and create playable questions
            List<Question> questions = gm.getAllQuestions(round);
            foreach(Question q in questions)
            {
                playableQuestions.Add(new PlayableQuestion(q));
            }
        }

        public void setRound(Round rnd)
        {
            round = rnd;

            //clear the lists and recreate the values
            playableQuestions.Clear();
            List<Question> questions = gm.getAllQuestions(round);
            foreach (Question q in questions)
            {
                playableQuestions.Add(new PlayableQuestion(q));
            }
        }
    }
}