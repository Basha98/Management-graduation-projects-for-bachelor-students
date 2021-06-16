using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using App.Models;

namespace App.Controllers
{[Authorize]
    public class GroupsController : Controller
    {
        private AppDBEntities db = new AppDBEntities();

        // GET: Groups
        [Authorize(Roles = "admin")]
        public ActionResult Index(int id)
        {
            var group = db.Project.Where(g => g.Supervisor_Id==id);
            return View(group.ToList());
        }



        [HttpGet]
        public bool CheckType(int IdUser)
        {
            var curentUser = db.Supervisor.FirstOrDefault(x => x.Id == IdUser);
            if (curentUser != null)
                return true;
            else
                return false;
        }

        public ActionResult Details(string id)
        {
            
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            if (CheckType(int.Parse(User.Identity.Name)))
            {
                Project project = db.Project.Find(id);
                return View(project);
            }
            else
            {
                if (ckeckToken(id))
                {
                    Project group = db.Project.Find(id);
                    return View(group);
                }
            }
            return HttpNotFound();

            
        }
        public bool ckeckToken(string token)
        {
            var c = db.Student.FirstOrDefault(x => x.Token == token && x.Id == int.Parse(User.Identity.Name));
            if (c != null)
            {
                return true;
            }

            else
                return false;
 }
        [HttpGet]
        public ActionResult convert()
        {
            return RedirectToAction("SelectGrop", "Main");
        }


        // GET: Groups/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.Admin_Id = new SelectList(db.Supervisor, "Id", "Id", project.Supervisor_Id);
            return View(project);
        }
        
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Project project)
        {
            if (ModelState.IsValid)
            {
                project.Supervisor_Id = int.Parse(User.Identity.Name);
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id=int.Parse(User.Identity.Name) });
            }
            ViewBag.Admin_Id = new SelectList(db.Supervisor, "Id", "Id", project.Supervisor_Id);
            return View(project);
        }



        // GET: Groups/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string token)
        {

            var Messag = db.Message.Where(x => x.Token == token).ToList();
            db.Message.RemoveRange(Messag);
            db.SaveChanges();
           
            var Students = db.Student.Where(x => x.Token == token).ToList();
            foreach (Student x in Students)
            {
                var connection = db.UserConnection.Where(y => y.UserId == x.Id).ToList();
                var role = db.Role.FirstOrDefault(z => z.UserId == x.Id);
                User user = db.User.Find(x.Id);
                db.UserConnection.RemoveRange(connection);
                db.Role.Remove(role);
                db.Student.Remove(x);
                db.User.Remove(user);
                db.SaveChanges();
            }
          



            Project project = db.Project.Find(token);
            db.Project.Remove(project);
            db.SaveChanges();
          return RedirectToAction("Index", new { id=int.Parse(User.Identity.Name) });
        }
        

    }
}
