using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using App.Models;
using App.Servier;
namespace App.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        AppDBEntities db = new AppDBEntities();

        /// <summary>
        /// register Student
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RegisterStudent()
        {
            if (User.Identity.IsAuthenticated)
            { return RedirectToAction("IndexAdmin", "Main"); }
            return View();
        }

        [HttpPost]
        public ActionResult RegisterStudent(StudentDetials student)
        {
            if (ModelState.IsValid)
            {
                if ((foundEmail(student.Email) == true) && (foundToken(student.Token) == true))
                {
                    db.User.Add(new User { FullName = student.FullName, Email = student.Email, Password = student.Password });
                    db.SaveChanges();
                    var ID_User = db.User.FirstOrDefault(x => x.Email == student.Email).Id;
                    db.Student.Add(new Student { Id = ID_User, Token = student.Token });
                    db.SaveChanges();
                    db.Role.Add(new Role { UserId = ID_User, NameRole = "student" });
                    db.SaveChanges();

                    FormsAuthentication.RedirectFromLoginPage(ID_User.ToString(), true);
                    FormsAuthentication.SetAuthCookie(ID_User.ToString(),true);
                   
                    return RedirectToAction("IndexAdmin", "Main",new { token=student.Token});
                }
            }
            return View();
        }

        public bool foundToken(string token)
        {
            var c = db.Project.FirstOrDefault(x => x.Token == token);
            if (c != null)
                return true;
            else
            {
                ViewBag.TokenMessage = "This Code of Project  isn't  Found ! ......Plase Enter another Code";
                return false;
            }
        }

        public bool foundEmail(string email)
        {
            var c = db.User.FirstOrDefault(x => x.Email == email);
            if (c != null)
            {
                ViewBag.EmailMesage = "This Email is Found ! .....Plase Enter another Email";
                return false;
            }

            else
                return true;
        }


        /// <summary>
        /// Register Admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RegisterAdmin()
        {

            if (User.Identity.IsAuthenticated)
            { return RedirectToAction("CreateGroup", "Main"); }
            return View();
        }

        [HttpPost]
        public ActionResult RegisterAdmin(User admin)
        {
            if (ModelState.IsValid)
            {
                if (foundEmail(admin.Email) == true)
                {
                    db.User.Add(new User { FullName = admin.FullName, Email = admin.Email, Password = admin.Password });
                    db.SaveChanges();
                    var ID_User = db.User.FirstOrDefault(x => x.Email == admin.Email).Id;
                    db.Supervisor.Add(new Supervisor { Id = ID_User });
                    db.SaveChanges();
                    db.Role.Add(new Role { UserId = ID_User, NameRole = "admin" });
                    db.SaveChanges();

                    FormsAuthentication.RedirectFromLoginPage(ID_User.ToString(), true);
                    FormsAuthentication.SetAuthCookie(ID_User.ToString(), true);
                  
                    return RedirectToAction("CreateGroup", "Main");
                }
            }
            return View();
        }






        /// <summary>
        /// Login Admin
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult login()
        {
            if (User.Identity.IsAuthenticated)
            { return RedirectToAction("SelectGrop", "Main"); }
            return View(new loginData());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult login(loginData user)
        {
            if (ModelState.IsValid)
            {
                int userIdd;
                if (new AppServices().loginAdmin(user, out userIdd) ==true)
                {
                    FormsAuthentication.RedirectFromLoginPage(userIdd.ToString(), user.RemmmberMe);
                    FormsAuthentication.SetAuthCookie(userIdd.ToString(), user.RemmmberMe);
                    
                    return RedirectToAction("SelectGrop", "Main");
                }
                else
                {
                    ViewBag.loginError = "Please entry a correct Email and Password!";

                    return View();
                }

            }
            return HttpNotFound();
        }
        /// <summary>
        /// logOut
        /// </summary>
        /// <returns></returns>
        public ActionResult logout()
        {
           int userId = int.Parse(User.Identity.Name);
            new AppServices().RemovAllUserConnection(userId);
            Hubs.ChatHubs.offlineUsers(userId);
            FormsAuthentication.SignOut();
            return RedirectToAction("start");
        }


/// <summary>
/// login Student
/// </summary>
/// <returns></returns>


        
        public string gettoken(int id)
        {
            return db.Student.FirstOrDefault(x => x.User.Id == id).Token;
        }
        [HttpGet]
        public ActionResult loginStudent()
        {
            if (User.Identity.IsAuthenticated)
            { 

                return RedirectToAction("IndexAdmin", "Main", new { token = gettoken(int.Parse(HttpContext.User.Identity.Name)) });
            }
            return View(new loginData());
        }
   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult loginStudent(loginData user)
        {
            if (ModelState.IsValid)
            {
                int userIdd;
                if (new AppServices().loginStudent(user, out userIdd) == true)
                {
                    FormsAuthentication.RedirectFromLoginPage(userIdd.ToString(), user.RemmmberMe);
                    FormsAuthentication.SetAuthCookie(userIdd.ToString(), true);
                   
                    return RedirectToAction("IndexAdmin", "Main",new { token=gettoken(userIdd) });
                }
                else
                {
                    ViewBag.loginStudentError = "Please entry a correct Email and Password!";

                    return View();
                }

            }
            return HttpNotFound();
        }


        /// <summary>
        /// start and chose
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult start()
        {
       
            if (User.Identity.IsAuthenticated)
            {
                if (CheckType(int.Parse(User.Identity.Name)))
                { return RedirectToAction("SelectGrop", "Main"); }
                else
                {
                    return RedirectToAction("IndexAdmin", "Main", new { token = gettoken(int.Parse(HttpContext.User.Identity.Name)) });
                }

            }
            return View();
        }
        public bool CheckType(int IdUser)
        {
            var curentUser = db.Supervisor.FirstOrDefault(x => x.Id == IdUser);
            if (curentUser != null)
                return true;
            else
                return false;
        }


    }
}
