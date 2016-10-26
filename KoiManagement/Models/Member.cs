//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KoiManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Member
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Member()
        {
            this.ActiveCodes = new HashSet<ActiveCode>();
            this.Answers = new HashSet<Answer>();
            this.Articles = new HashSet<Article>();
            this.Comments = new HashSet<Comment>();
            this.KoiFarms = new HashSet<KoiFarm>();
            this.Owners = new HashSet<Owner>();
            this.Questions = new HashSet<Question>();
            this.Rates = new HashSet<Rate>();
            this.Reports = new HashSet<Report>();
            this.Reports1 = new HashSet<Report>();
        }

        public Member(string name, string Username, string password, DateTime? Dob, string Image, string Gender, string Phone, string email, string Address, bool Status)
        {
            this.ActiveCodes = new HashSet<ActiveCode>();
            this.Answers = new HashSet<Answer>();
            this.Articles = new HashSet<Article>();
            this.Comments = new HashSet<Comment>();
            this.KoiFarms = new HashSet<KoiFarm>();
            this.Owners = new HashSet<Owner>();
            this.Questions = new HashSet<Question>();
            this.Rates = new HashSet<Rate>();
            this.Reports = new HashSet<Report>();
            this.Reports1 = new HashSet<Report>();
            this.Name = name;
            this.UserName = Username;
            this.Password = password;
            this.Dob = Dob;
            this.Image = Image;
            this.Gender = Gender;
            this.Phone = Phone;
            this.Email = email;
            this.Address = Address;
            this.Status = Status;
        }

        public int MemberID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> Dob { get; set; }
        public string Image { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActiveCode> ActiveCodes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answer> Answers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Article> Articles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KoiFarm> KoiFarms { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Owner> Owners { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Question> Questions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rate> Rates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Report> Reports { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Report> Reports1 { get; set; }
    }
}
