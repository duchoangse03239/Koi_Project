using System;
using System.Collections.Generic;
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
    }
}