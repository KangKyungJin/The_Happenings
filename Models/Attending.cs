using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CSharpBeltTest.Models{
    public class Attending{
        [Key]
        public int AttendingId {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
        public int HappeningId {get;set;}
        public Happening Happening {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}