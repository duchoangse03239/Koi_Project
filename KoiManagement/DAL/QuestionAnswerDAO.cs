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
            return db.Answers.Where(p => p.QuestionID == Qid && p.Status).OrderBy(p => p.Datetime).ToList();
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

    }
}