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
        public List<PlayableQuestion> playableQuestions = new List<PlayableQuestion>();
        GameModel gm = new GameModel();

        //constructor
        public PlayableRound() { }

        public PlayableRound(Round rnd)
        {
            //select the round
            round = rnd;

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