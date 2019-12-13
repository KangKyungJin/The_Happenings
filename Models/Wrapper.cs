using System;
using System.Collections.Generic;

namespace CSharpBeltTest.Models{
    public class Wrapper{
        public User thisUser {get;set;}
        public LoginUser logUser {get;set;}
        public int loggedUser {get;set;}
        public Happening haps {get;set;}
        public List<Happening> allHaps {get;set;}
        public List<User> allUsers {get;set;}
    }
}