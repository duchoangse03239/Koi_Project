using System;
using System.Collections.Generic;
using System.Linq;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class ReportDAO
    {
        KoiManagementEntities db = null;
        public ReportDAO()
        {
            db = new KoiManagementEntities();
        }

        public int CountMemberReport(int MemberID)
        {
            return db.Reports.Count(p => p.ObjectType == "Member" && p.ObjectId == MemberID);
        }
        public int CountKoiReport(int KoiID)
        {
            return db.Reports.Count(p => p.ObjectType == "Koi" && p.ObjectId == KoiID);
        }
        public int CountKoiFarmReport(int KoiFarmID)
        {
            return db.Reports.Count(p => p.ObjectType == "KoiFarm" && p.ObjectId == KoiFarmID);
        }
        public int CountQuestionReport(int QuestionID)
        {
            return db.Reports.Count(p => p.ObjectType == "Question" && p.ObjectId == QuestionID);
        }
        public int CountAnswerReport(int Answer)
        {
            return db.Reports.Count(p => p.ObjectType == "Answer" && p.ObjectId == Answer);
        }

    }
}
