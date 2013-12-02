using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GymGame.Classes;

namespace GymGame.Models
{
    public class GameModel
    {
        private GymGameModelDataContext dc = new GymGameModelDataContext();

        public int InsertResult(User user, Question question, Answer answer, Quiz quiz)
        {
            /**
             * User komt van Sessie,
             * Question ook (of uit controller)
             * Answer komt van controller (ajax post)
             * Quiz komt van sessie (of controller)
             **/

            //result aanmaken
            Result r = new Result();
            r.FK_User = user.User_Id;
            r.FK_Question = question.Question_Id;
            r.FK_Answer = answer.Answer_Id;
            r.FK_Quiz = quiz.Quiz_Id;

            //posten naar database
            dc.Results.InsertOnSubmit(r);
            dc.SubmitChanges();

            return r.Result_Id;
        }


        /**
         * --------------------------------------SELECT------------------------------------------*
         * select query's:
         ** !!!!!!!!!!!!!!!!!! joins doen, anders komen er enkel enkele integers terug !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! ** 
         **/

        public List<Result> getAllResults()
        {
            var result = (from r in dc.Results
                          select r).ToList<Result>();
            return result;
        }

        public List<Result> getResultsByUser(User u)
        {
            var result = (from r in dc.Results
                          where r.FK_User == u.User_Id
                          select r).ToList<Result>();
            return result;
        }

        public List<Result> getAllResultsByQuiz(Quiz q)
        {
            var result = (from r in dc.Results
                          where r.FK_Quiz == q.Quiz_Id
                          select r).ToList<Result>();
            return result;
        }

        public List<Result> getAllResultsByQuestion(Question qu)
        {
            var result = (from r in dc.Results
                          where r.FK_Question == qu.Question_Id
                          select r).ToList<Result>();
            return result;
        }

        public List<Result> getResultsByUserAndQuiz(User u, Quiz q)
        {
            var result = (from r in dc.Results
                          where r.FK_Quiz == q.Quiz_Id && r.FK_User == u.User_Id
                          select r).ToList<Result>();
            return result;
        }

        public int getRightAnswersByQuiz(Quiz q)
        {
            var result = (from r in dc.Results
                          join a in dc.Answers on r.FK_Answer equals a.Answer_Id
                          where r.FK_Quiz == q.Quiz_Id
                          select r).ToList();

            //tel de rijen met een goed antwoord.
            foreach (var el in result)
            {
                if (el.Answer.Answer_value == 0) result.Remove(el);
            }
            return result.Count;

        }


        //---------------------------------------------------------------
        //get quizzes etc.
        public Quiz getQuiz(Quiz quiz)
        {
            if (quiz == null)
            {
                throw new Exception("User must be given...");
            }
            var result = (from q in dc.Quizs
                          where q.Quiz_Id == quiz.Quiz_Id
                          select q).Single();
            return (Quiz)result;
        }

        public Quiz getQuizByName(Quiz quiz)
        {
            if (quiz == null)
            {
                throw new Exception("User must be given...");
            }
            var result = (from q in dc.Quizs
                          where q.name == quiz.name
                          select q).Single();
            return (Quiz)result;
        }

        public List<Round> getAllRounds(Quiz q)
        {
            if (q == null)
            {
                throw new Exception("Quiz must be given...");
            }
            var result = (from round in dc.Rounds
                          where round.FK_Quiz == q.Quiz_Id
                          select round).ToList<Round>();
            return result;
        }

        public List<Question> getAllQuestions(Round round)
        {
            if (round == null)
            {
                throw new Exception("Round must be given...");
            }
            var result = (from q in dc.Questions
                          where q.FK_Round == round.Round_Id
                          select q).ToList<Question>();
            return result;
        }

        public List<Answer> getAllAnswers(Question qu)
        {
            if (qu == null)
            {
                throw new Exception("Question must be given...");
            }
            var result = (from a in dc.Answers
                          where a.FK_Question == qu.Question_Id
                          select a).ToList<Answer>();
            return result;
        }

        public List<Object> getFullQuiz(Quiz quiz)
        {
            if (quiz == null)
            {
                throw new Exception("Quiz must be given...");
            }

            Quiz playQuiz = getQuiz(quiz);
            List<Round> playRounds = getAllRounds(playQuiz);
            List<Question> playQuestions = new List<Question>();
            foreach (Round el in playRounds)
            {
                List<Question> toAdd = getAllQuestions(el);
                foreach (Question q in toAdd)
                {
                    playQuestions.Add(q);
                }
            }
            List<Answer> playAnswers = new List<Answer>();
            foreach (Question el in playQuestions)
            {
                List<Answer> toAdd = getAllAnswers(el);
                foreach (Answer a in toAdd)
                {
                    playAnswers.Add(a);
                }
            }

            List<Object> playableQuiz = new List<Object>();
            playableQuiz.Add(playQuiz);
            playableQuiz.Add(playRounds);
            playableQuiz.Add(playQuestions);
            playableQuiz.Add(playAnswers);

            return playableQuiz;
        }

        public PlayableQuiz getPlayableQuiz(int quizId)
        {
            return new PlayableQuiz(quizId);
        }

        public PlayableQuiz getPlayableQuizByName(String quizName)
        {
            return new PlayableQuiz(quizName);
        }


    }
}