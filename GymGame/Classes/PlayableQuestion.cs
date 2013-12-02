using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GymGame.Models;

namespace GymGame.Classes
{
    public class PlayableQuestion
    {
        public Question question = new Question();
        public List<Answer> answers = new List<Answer>();
        GameModel gm = new GameModel();

        //constructor
        public PlayableQuestion() { }

        public PlayableQuestion(Question qst)
        {
            //set the question
            question = qst;

            //get the answers
            answers = gm.getAllAnswers(question);
        }

        public void setQuestion(Question qst) {
            question = qst;

            //clear the lists and recreate the values
            answers.Clear();
            answers = gm.getAllAnswers(question);
        }
    }
}