using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSharpBeltTest.Models;
using Microsoft.EntityFrameworkCore;
using CSharpBeltTest.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace LogReg.Controllers{
    public class LogRegController : Controller{
        private MyContext dbContext;
        public LogRegController(MyContext context)
        {
            dbContext = context;
        }
        public int? InSession {
            get { return HttpContext.Session.GetInt32("loggedUser"); }
            set { HttpContext.Session.SetInt32("loggedUser", (int)value); }
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]
        public IActionResult RegisterUser(Wrapper newUser){
            User nUser = newUser.thisUser;
            User dupe = dbContext.Users.FirstOrDefault(u => u.Email == newUser.thisUser.Email);
            if(dupe != null){
                ModelState.AddModelError("newUser.thisUser.Email", "Email already exists");
                return View("Index", newUser);
            }
            if(ModelState.IsValid){
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                nUser.Password = Hasher.HashPassword(nUser, nUser.Password);
                dbContext.Add(nUser);
                dbContext.SaveChanges();
                InSession = nUser.UserId;
                return RedirectToAction("Dashboard", "Home");
            } else {
                return View("Index", newUser);
            }
        }
        [HttpPost("login")]
        public IActionResult Login(Wrapper userLog){
            LoginUser logU = userLog.logUser;
            if(ModelState.IsValid){
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == logU.Email);
                if(userInDb == null){
                    ModelState.AddModelError("userLog.logUser.Email", "Invalid Email.");
                    return View("Index", userLog);
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(logU, userInDb.Password, logU.Password);
                if(result == 0){
                    ModelState.AddModelError("userLog.logUser.Password", "Wrong Password.");
                    return View("Index", userLog);
                }
                InSession = userInDb.UserId;
                return RedirectToAction("Dashboard", "Home");
            } else {
                return View("Index", userLog);
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}