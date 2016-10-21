﻿using System;
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

        public Member GetMemberByEmail(string email)
        {
            var dbmem = db.Members.Where(p => p.Email == email).ToList();
            if (dbmem.Count>0)
            {
                     return dbmem.ElementAt(0);
            }
            return null;
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

        public bool CheckChangePass(string memberId,string password)
        {
            var member = db.Members.Find(int.Parse(memberId));
            if (member.Password.Equals(password)) return false;
            return true;
        }

        public int ChangePass(string memberId, string password)
        {
            var member = db.Members.Find(int.Parse(memberId));
            member.Password = password;
            db.Members.Attach(member);
            db.Entry(member).Property(x => x.Password).IsModified = true;
            return db.SaveChanges();
        }
    }
}
