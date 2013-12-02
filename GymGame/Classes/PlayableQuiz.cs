using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GymGame.Models;

namespace GymGame.Classes
{
    public class PlayableQuiz
    {
        Quiz quiz = new Quiz();
        List<PlayableRound> playableRounds = new List<PlayableRound>();
        GameModel gm = new GameModel();

        //constructor
        public PlayableQuiz() { }
        
        public PlayableQuiz(int quizId)
        {
            //select the quiz
            Quiz selectQuiz = new Quiz();
            selectQuiz.Quiz_Id = quizId;
            quiz = gm.getQuiz(selectQuiz);

            //select the rounds and create playable rounds
            List<Round> rounds = gm.getAllRounds(quiz);
            foreach(Round round in rounds)
            {
                playableRounds.Add(new PlayableRound(round));
            }
        }

        public void setQuiz(Quiz qz) {
            quiz = qz;

            //clear the lists and recreate the values
            playableRounds.Clear();
            List<Round> rounds = gm.getAllRounds(quiz);
            foreach (Round round in rounds)
            {
                playableRounds.Add(new PlayableRound(round));
            }
        }
    }
}