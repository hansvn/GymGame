using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
         ** !!!!!!!!!!!!!!!!!! joins doen, anders komen er enkel enkele integers terug !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!** 
         **/

        public List<Result> getAll()
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

        public List<Result> getAllByQuiz(Quiz q)
        {
            var result = (from r in dc.Results
                          where r.FK_Quiz == q.Quiz_Id
                          select r).ToList<Result>();
            return result;
        }

        public List<Result> getAllByQuestion(Question qu)
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


    }
}