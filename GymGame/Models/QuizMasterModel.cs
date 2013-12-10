using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GymGame.Models
{
    public class QuizMasterModel
    {
        private GymGameModelDataContext dc = new GymGameModelDataContext();

        /** 
         * moet 
         * -apart al zijn quizzen kunnen opvragen
         * -per quiz zijn vragen (volgende pagina/ajax)
         * -per vraag zijn antwoorden (volgende pagina/ajax) 
         * 
         * -Update queries --ok
         * 
         * -schrijven (quiz, vraag1, ..) --ok
         * nog insertquery waar id's automatisch worden toegevoegd.
         * 
         * (-statussen opvolgen (juiste vragen,...) )
         **/

        /**
         * --------------------------------------INSERT ------------------------------------------*
         **/

        public int insertQuiz(Quiz q)
        {
            //posten naar database
            dc.Quizs.InsertOnSubmit(q);
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Query couldn't be executed...");
            }

            return q.Quiz_Id;
        }

        public int insertRound(Round r)
        {
            dc.Rounds.InsertOnSubmit(r);
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Query couldn't be executed...");
            }

            return r.Round_Id;
        }

        public int insertQuestion(Question q)
        {
            dc.Questions.InsertOnSubmit(q);
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Query couldn't be executed...");
            }

            return q.Question_Id;
        }

        public int insertAnswer(Answer a)
        {
            dc.Answers.InsertOnSubmit(a);
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Query couldn't be executed...");
            }

            return a.Answer_Id;
        }



        /**
         * --------------------------------------UPDATE ------------------------------------------*
         **/

        public int updateQuiz(Quiz quiz)
        {
            //query naar database om waarde te vinden
            Quiz result = (from q in dc.Quizs
                          where q.Quiz_Id == quiz.Quiz_Id
                          select q).Single();

            //veranderingen aanbrengen
            if (quiz.FK_Users != null) result.FK_Users = quiz.FK_Users;
            if (quiz.Location != null) result.Location = quiz.Location;
            if (quiz.Date != null) result.Date = quiz.Date;

            //updaten in db
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Value couldn't be updated...");
            }

            return result.Quiz_Id;
        }

        public int updateRound(Round round)
        {
            //query naar database om waarde te vinden
            Round result = (from r in dc.Rounds
                           where r.Round_Id == round.Round_Id
                           select r).Single();

            //veranderingen aanbrengen
            if (round.FK_Quiz != null) result.FK_Quiz = round.FK_Quiz;
            if (round.Max_Time != null) result.Max_Time = round.Max_Time;
            if (round.Round_name != null) result.Round_name = round.Round_name;

            //updaten in db
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Value couldn't be updated...");
            }

            return result.Round_Id;
        }

        public Round startRound(Round round)
        {
            //start de ronde
            Round result = (from r in dc.Rounds
                            where r.Round_Id == round.Round_Id
                            select r).SingleOrDefault();

            result.Round_started = DateTime.Now;
            result.Max_Time = round.Max_Time;

            //updaten in db
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Value couldn't be updated...");
            }

            return result;
        }

        public Round stopRound(Round round)
        {
            //stop de ronde
            Round result = (from r in dc.Rounds
                            where r.Round_Id == round.Round_Id
                            select r).SingleOrDefault();

            TimeSpan maxTime = (TimeSpan)(DateTime.Now - result.Round_started);
            result.Max_Time = (int)maxTime.TotalSeconds;

            //updaten in db
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Value couldn't be updated...");
            }

            return result;
        }

        public int updateQuestion(Question question)
        {
            //query naar database om waarde te vinden
            Question result = (from q in dc.Questions
                            where q.Question_Id == question.Question_Id
                            select q).Single();

            //veranderingen aanbrengen
            if (question.Question_Text != null) result.Question_Text = question.Question_Text;
            if (question.FK_Round != null) result.FK_Round = question.FK_Round;

            //updaten in db
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Value couldn't be updated...");
            }

            return result.Question_Id;
        }

        public int updateAnswer(Answer answer)
        {
            //query naar database om waarde te vinden
            Answer result = (from a in dc.Answers
                            where a.Answer_Id == answer.Answer_Id
                            select a).Single();

            //veranderingen aanbrengen
            if (answer.Answer_Text != null) result.Answer_Text = answer.Answer_Text;
            if (answer.FK_Question != null) result.FK_Question = answer.FK_Question;
            if (answer.FK_Type != null) result.FK_Type = answer.FK_Type;
            if (answer.Answer_value != null) result.Answer_value = answer.Answer_value;

            //updaten in db
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Value couldn't be updated...");
            }

            return result.Answer_Id;
        }





        /**
         * --------------------------------------SELECT ------------------------------------------*
         * select query's:
         ** joins worden nog niet gedaan... gewoon elke functie in volgorde oproepen werkt ook... ** 
         **/

        public List<Quiz> getAllQuizzes(User u)
        {
            if (u == null)
            {
                throw new Exception("User must be given...");
            }
            var result = (from q in dc.Quizs
                          where q.FK_Users == u.User_Id
                          select q).ToList<Quiz>();
            return result;
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

        public List<Result> getResultsByUserAndQuiz(User u, Quiz q)
        {
            if (u == null)
            {
                throw new Exception("User must be given...");
            }
            else if (q == null)
            {
                throw new Exception("Quiz must be given...");
            }
            var result = (from r in dc.Results
                          where r.FK_Quiz == q.Quiz_Id && r.FK_User == u.User_Id
                          select r).ToList<Result>();
            return result;
        }

    }
}