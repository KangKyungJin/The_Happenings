using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSharpBeltTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace CSharpBeltTest.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        public int? InSession {
            get { return HttpContext.Session.GetInt32("loggedUser"); }
            set { HttpContext.Session.SetInt32("loggedUser", (int)value); }
        }
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if(InSession == null){
                return RedirectToAction("Index", "LogReg");
            }
            Wrapper dash = new Wrapper();
            dash.allHaps = dbContext.Happenings
                .Include(h => h.Coming)
                .ThenInclude(a => a.User)
                .OrderByDescending(h => h.Time)
                .OrderByDescending(h => h.Date)
                .ToList();
            dash.thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == InSession);
            dash.allUsers = dbContext.Users.ToList();
            return View(dash);
        }

        [HttpGet("new")]
        public IActionResult NewHap(){
            if(InSession == null){
                return RedirectToAction("Index", "LogReg");
            }
            return View();
        }
        [HttpPost("new")]
        public IActionResult createHap(Wrapper newHap, string dur){
            if(InSession == null){
                return RedirectToAction("Index", "LogReg");
            }
            if(ModelState.IsValid){
                newHap.haps.UserId = (int)InSession;
                dbContext.Happenings.Add(newHap.haps);
                dbContext.SaveChanges();
                return RedirectToAction("Happening", new { id = newHap.haps.HappeningId});
            } else {
                return View("NewHap", newHap);
            }
        }

        [HttpGet("activity/{id}")]
        public IActionResult Happening(int id){
            if(InSession == null){
                return RedirectToAction("Index", "LogReg");
            }
            Wrapper thishaps = new Wrapper();
            thishaps.haps = dbContext.Happenings
                .Include(h => h.Coming)
                .ThenInclude(u => u.User)
                .FirstOrDefault(h => h.HappeningId == id);
            thishaps.allUsers = dbContext.Users.ToList();
            thishaps.thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)InSession);
            return View(thishaps);
        }

        [HttpGet("activity/join/{id}")]
        public IActionResult Join(int id){
            if(InSession == null){
                return RedirectToAction("Index", "LogReg");
            }
            if(dbContext.Attendings.Any(a => a.UserId == (int)InSession && a.HappeningId == id)){
                return RedirectToAction("Happening", new { id = id});
            }
            Attending newA = new Attending();
            newA.UserId = (int)InSession;
            newA.HappeningId = id;
            if(ModelState.IsValid){
                dbContext.Attendings.Add(newA);
                dbContext.SaveChanges();
                return RedirectToAction("Happening", new { id = id});
            }
            return RedirectToAction("Dashboard");
        }
        [HttpGet("activity/unjoin/{id}")]
        public IActionResult UnJoin(int id){
            if(InSession == null){
                return RedirectToAction("Index", "LogReg");
            }
            Attending att = dbContext.Attendings.FirstOrDefault(a => a.UserId == (int)InSession && a.HappeningId == id);
            if(att != null){
                dbContext.Attendings.Remove(att);
                dbContext.SaveChanges();
                return RedirectToAction("Happening", new { id = id});
            }
            return RedirectToAction("Dashboard");
        }
        [HttpGet("logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "LogReg");
        }
        [HttpGet("activity/delete/{id}")]
        public IActionResult Delete(int id){
            if(InSession == null){
                return RedirectToAction("Index", "LogReg");
            }
            Happening thisH = dbContext.Happenings.FirstOrDefault(h => h.HappeningId == id);
            if((int)InSession != thisH.UserId){
                return RedirectToAction("Index", "LogReg");
            } else {
                if(thisH != null){
                    dbContext.Happenings.Remove(thisH);
                    dbContext.SaveChanges();
                }
                return RedirectToAction("Dashboard");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
