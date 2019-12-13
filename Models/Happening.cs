using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CSharpBeltTest.Models{
    public class Happening{
        [Key]
        public int HappeningId {get;set;}
        [Required]
        public string Name {get;set;}
        [Required]
        public DateTime Time {get;set;}
        [Required]
        [DateFuture]
        public DateTime Date {get;set;}
        [Required]
        public int DurationT {get;set;}
        [Required]
        public string DurationD {get;set;}
        [Required]
        public string Desc {get;set;}
        [Required]
        public int UserId {get;set;}
        public User User {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Attending> Coming {get;set;}
    }
    public class DateFutureAttribute : ValidationAttribute{
        protected override ValidationResult IsValid(object value, ValidationContext validationContext){
            if((DateTime)value < DateTime.Now){
                return new ValidationResult("Date must be in the future");
            }
            return ValidationResult.Success;
        }
    }
}