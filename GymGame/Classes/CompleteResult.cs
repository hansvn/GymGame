using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GymGame.Models;

namespace GymGame.Classes
{
    public class CompleteResult
    {
        public User user = new User();
        public Question question = new Question();
        public Answer answer = new Answer();
        public Quiz quiz = new Quiz();
        public Answer rightAnswer = new Answer();
        public Boolean isRight;
        Result result = new Result();
        GameModel gm = new GameModel();

        public CompleteResult(Result res)
        {
            //set the result
            result = res;

            try
            {
                //get the question
                question.Question_Id = res.FK_Question;
                question = gm.getQuestion(question);

                //get the answer
                answer.Answer_Id = res.FK_Answer;
                answer = gm.getAnswer(answer);
                isRight = (answer.Answer_value == 1);

                //if needed, get the right answer
                if (!isRight)
                {
                    Question q = new Question();
                    q.Question_Id = answer.FK_Question;
                    rightAnswer = gm.getRightAnswer(q);
                }

                //get the quiz
                quiz.Quiz_Id = res.FK_Quiz;
                quiz = gm.getQuiz(quiz);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}