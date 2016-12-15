using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class QuestionAnswerDAO
    {
        KoiManagementEntities db = null;

        /// <summary>
        /// QuestionAnswerDAO
        /// </summary>
        public QuestionAnswerDAO()
        {
            db = new KoiManagementEntities();
        }

        /// <summary>
        /// CreateQuestion
        /// </summary>
        /// <param name="question">question</param>
        /// <returns>bool</returns>
        public bool CreateQuestion(Question question)
        {
            try
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Question GetQuestionByID(int id)
        {
            return db.Questions.Find(id);
        }

        public Answer GetAnswerById(int id)
        {
            return db.Answers.Find(id);
        }

        /// <summary>
        /// CreateAnswer
        /// </summary>
        /// <param name="answer">answer</param>
        /// <returns>bool</returns>
        public bool CreateAnswer(Answer answer)
        {
            try
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Question> GetListQuestion()
        {
            return db.Questions.ToList();
        }

        public Question GetQuestionDetail(int id)
        {
            return db.Questions.Find(id);
        }

        public int CountAnswer(int Qid)
        {
            return db.Answers.Count(p => p.QuestionID == Qid&&p.Status);
        }

        public Answer GetLastAnswer(int Qid)
        {
            return db.Answers.Where(p => p.QuestionID == Qid && p.Status).OrderBy(p => p.Datetime).FirstOrDefault();
        }

        public List<Answer> GetListAnswerbyId(int Qid)
        {
            return db.Answers.Where(p => p.QuestionID == Qid &&p.AnswerDetail==null&& p.Status).OrderBy(p => p.Datetime).ToList();
        }

        public List<List<Answer>> GetListAnswerDetail(int Qid)
        {
            List<List<Answer>> ListA = new List<List<Answer>>();
          var an =  db.Answers.Where(p => p.QuestionID == Qid && p.AnswerDetail == null && p.Status).OrderBy(p => p.Datetime).ToList();
            foreach (var item in an)
            {
                var b = db.Answers.Where(p => p.QuestionID == Qid && p.AnswerDetail == item.AnswerID && p.Status).ToList();
                ListA.Add(b);
            }
            return ListA;
        }

        public int getRateQuestion(int Qid)
        {
            decimal? rate = 0;
            var t = db.Rates.Where(p => p.QuestionID == Qid ).ToList();
            if (t.Count > 0)
            {
                foreach (var item in t)
                {
                    
                        rate += item.RateSocre;
                }

                int t1 = (int)Math.Round((decimal)(rate / getTotalRateQuestion(Qid)));
                return t1;
            }
            return 0;
        }

        public int getTotalRateQuestion(int Qid)
        {
            return  db.Rates.Count(p => p.QuestionID == Qid );
        }

        public int getRateAnswer(int Aid)
        {
            decimal? rate = 0;
            var t = db.Rates.Where(p => p.AnswerID == Aid).ToList();
            if (t.Count > 0)
            {
                foreach (var item in t)
                {

                    rate += item.RateSocre;
                }

                int t1 = (int)Math.Round((decimal)(rate / getTotalRateAnswer(Aid)));
                return t1;
            }
            return 0;
        }

        public int getTotalRateAnswer(int Aid)
        {
            return db.Rates.Count(p => p.AnswerID == Aid);
        }
        public bool CheckRateA(int userId, int Aid)
        {
            try
            {
                var t = db.Rates.Count(p => p.MemberID == userId && p.AnswerID == Aid);
                if (t > 0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckRateQ(int userId, int Qid)
        {
            try
            {
                var t = db.Rates.Count(p => p.MemberID == userId && p.QuestionID == Qid);
                if (t > 0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}