using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using App.Models;

namespace App.Controllers
{[Authorize]
    public class UsersController : Controller
    {
        private AppDBEntities db = new AppDBEntities();

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }



        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Supervisor, "Id", "Id", user.Id);
            ViewBag.Id = new SelectList(db.Student, "Id", "TokenGrop", user.Id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                if (foundEmail(user.Email))
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    //FormsAuthentication.RedirectFromLoginPage(user.Id.ToString(), true);
                    //FormsAuthentication.SetAuthCookie(user.Id.ToString(), true);
                    return RedirectToAction("Details", "Users", new { id = int.Parse(User.Identity.Name) });
                }
            }
            ViewBag.Id = new SelectList(db.Supervisor, "Id", "Id", user.Id);
            ViewBag.Id = new SelectList(db.Student, "Id", "TokenGrop", user.Id);
            return View(user);
        }
        public bool CheckType(int IdUser)
        {
            var curentUser = db.Supervisor.FirstOrDefault(x => x.Id == IdUser);
            if (curentUser != null)
                return true;
            else
                return false;
        }

        public bool foundEmail(string email)
        {
            int id = int.Parse(User.Identity.Name);
            var c = db.User.Where(x=>x.Id!=id).FirstOrDefault(x => x.Email == email);
            if (c != null)
            {
                ViewBag.EmailProfile ="This Email is Found ! .....Plase Enter another Email";
                return false;
            }

            else
                return true;
        }

        [HttpGet]
        public ActionResult convert(string Token)
        {
            return RedirectToAction("IndexAdmin", "Main",new { token=Token});
        }

        [HttpGet]
        public ActionResult convert2()
        {
            
            return RedirectToAction("SelectGrop", "Main");
        }

    }
}
