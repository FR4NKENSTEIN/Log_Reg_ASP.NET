using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using YourNamespace.Models;

namespace YourNamespace.Controllers
{
    
    public class UserController: Controller
    {
        // -------------------- SESSION PROPERTIES --------------------
        public int? SessionId
        {
            get {return HttpContext.Session.GetInt32("UserID");}
            set {HttpContext.Session.SetInt32("UserID", (int)value);}
        }
        // public string SessionName
        // {
        //     get {return HttpContext.Session.GetString("UserName");}
        //     set {HttpContext.Session.SetString("UserName", value);}
        // }
        private YourContext database;
        // -------------------- CONSTRUCTOR --------------------
        public UserController(YourContext context)
        {
            database = context;
            Console.WriteLine("###### CONTEXT ACQUIRED");
        }
        // -------------------- RENDER FORMS --------------------
        [Route("")]
        public IActionResult UserIndex()
        {
            Console.WriteLine("###### @ UserIndex");
            return View();
        }
        // -------------------- PROCESS REGISTRATION --------------------
        [HttpPost("registering")]
        public IActionResult Register(UindexView patron)
        {
            if (ModelState.IsValid)
            {
                // check the email isn't already in the data base
                if (database.Users.Any(u => u.Email == patron.UserNew.Email))
                {
                    ModelState.AddModelError(
                        "UserNew.Email", "Email already in use... sorry"
                    );
                    Console.WriteLine("###### BAD EMAIL");
                    return View("UserIndex", patron);
                }
                // hash the password
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                string hashedPW = hasher
                    .HashPassword(patron.UserNew, patron.UserNew.Password);
                patron.UserNew.Password = hashedPW;
                Console.WriteLine("###### PW HAS BEEN HASHED");
                // Add and save to the database
                database.Add(patron.UserNew);
                database.SaveChanges();
                SessionId = patron.UserNew.UserId;
                // SessionName = patron.UserNew.FirstName + patron.UserNew.LastName;
                Console.WriteLine("###### VALID");
                return RedirectToAction("Success");
            }
            Console.WriteLine("###### INVALID");
            return View("UserIndex", patron);
        } 

        // -------------------- PROCESS LOGIN --------------------
        [Route("entering")]
        public IActionResult Login(UindexView patron)
        {
            if (ModelState.IsValid)
            {
                // I believe this query would prove inefficient for
                // large sets of data as I am looking at EVERY email.
                User UserInDb = database.Users
                    .FirstOrDefault(u => u.Email == patron.UserExist.Email);
                if (UserInDb == null)
                {
                    ModelState.AddModelError(
                        "UserExist.Email", "That Email does not exist"
                    );
                    Console.WriteLine("###### BAD EMAIL");
                    return View("UserIndex", patron);
                }
                PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();
                var result = hasher.VerifyHashedPassword(
                    patron.UserExist, UserInDb.Password, patron.UserExist.Password
                );
                if (result == 0)
                {
                    ModelState.AddModelError(
                        "UserExist.Password", "That password does not exist"
                    );
                    Console.WriteLine("###### BAD PASSWORD");
                    return View("UserIndex", patron);
                }
                SessionId = UserInDb.UserId;
                // SessionName = UserInDb.FirstName + UserInDb.LastName;
                return RedirectToAction("Success");
            }
            return View("UserIndex");
        } 
        // -------------------- LOGOUT --------------------
        [Route("exiting")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Console.WriteLine("###### BUH BUY");
            return RedirectToAction("UserIndex");
        }
        // -------------------- RUNNING A SESSION CHECK
        // [Route("success")]
        // public IActionResult Success()
        // {
        //     if (SessionId != null)
        //     {
        //         User Target = dbContext.Users
        //             .FirstOrDefault(u => u.UserId == SessionId);
        //         Console.WriteLine("###### SUCCESSFUL");
        //         return View(Target);
        //     }
        //     Console.WriteLine("###### NO NO NO NO NO, WE DON'T DO THAT");
        //     return RedirectToAction("UserIndex");
        // }
    }
}