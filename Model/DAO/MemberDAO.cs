using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Model.DAO
{
    public class MemberDAO
    {
        KoiManagementEntities db = null;
        public MemberDAO()
        {
            db = new KoiManagementEntities();
        }

        public List<Member> GetMemberByNameAndPass(string username, string password)
        {
             return db.Members.Where(p => p.UserName.Equals(username) && p.Password.Equals(password)).ToList();
        }

        public bool CheckExistUserName(string username)
        {
            var member =db.Members.Where(p => p.UserName == username).ToList();
            return member.Count <= 0;
        }

        public bool CheckExistEmail(string email)
        {
            var member = db.Members.Where(p => p.Email == email).ToList();
            return member.Count <= 0;
        }

        public bool AddMember(Member mem)
        {
            try { 
            db.Members.Add(mem);
            db.SaveChanges();
            return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
