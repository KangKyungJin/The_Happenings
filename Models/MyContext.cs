using System;
using Microsoft.EntityFrameworkCore;

namespace CSharpBeltTest.Models{
    public class MyContext : DbContext{
        public MyContext(DbContextOptions options) : base(options) { }
	    public DbSet<User> Users {get;set;}
        public DbSet<Happening> Happenings {get;set;}
        public DbSet<Attending> Attendings {get;set;}
    }
}