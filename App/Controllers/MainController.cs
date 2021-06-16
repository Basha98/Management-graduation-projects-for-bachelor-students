using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using App.Models;
using App.Servier;

namespace App.Controllers
{
    [Authorize(Roles = "student , admin")]
    public class MainController : Controller
    {
        AppDBEntities db = new AppDBEntities();
        public string Token;

        

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult SelectGrop()
        {
            var IdUser = int.Parse(HttpContext.User.Identity.Name);
            if (CheckType(IdUser))
            {
                var list = db.Project.Where(x => x.Supervisor_Id == IdUser).ToList();
                return View(list);
            }
            return HttpNotFound();
        }
        /// <summary>
        /// Index of Admin 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult IndexAdmin(string token)
        {
            if (token == null)
            {
                return HttpNotFound();
            }
            if (check(token) == true)
            {
                Token = token;
                return View(new IndexAdminDetails { Token = token });
            }
            return RedirectToAction("SelectGrop");
        }
        public bool CheckType(int IdUser)
        {
            var curentUser = db.Supervisor.FirstOrDefault(x => x.Id == IdUser);
            if (curentUser != null)
                return true;
            else
                return false;
        }
        public bool check(string token)
        {
            
            int id = int.Parse(User.Identity.Name);
            if (CheckType(id))
            {
               var stu = db.Project.FirstOrDefault(x => x.Supervisor_Id == id && x.Token == token);
                if (stu != null)
                    return true;
                else
                    return false;
            }
            else
            {
                 var stu = db.Student.FirstOrDefault(x => x.Id == id && x.Token == token);
                if (stu != null)
                    return true;
                else
                    return false;
            }
        }

      
        /// <summary>
        /// CreateGrop
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreateGroup()
        {
            var IdUser = int.Parse(HttpContext.User.Identity.Name);
            if (CheckType(IdUser))
            {
                return View();
            }
            return HttpNotFound();
         
        }
        [HttpPost]

        [Authorize(Roles = "admin")]
        public ActionResult CreateGroup(Project project)
        {
            if (ModelState.IsValid)
            {
                if (project != null && foundToken(project.Token) == true)
                {
                    var IdUser = int.Parse(HttpContext.User.Identity.Name);
                    db.Project.Add(new Project { GroupName = project.GroupName, Descriptation = project.Descriptation, Supervisor_Id = IdUser, Token = project.Token });
                    db.SaveChanges();
                    return RedirectToAction("SelectGrop");
                }

            }
            return View();
        }

        public bool foundToken(string token)
        {
            var c = db.Project.FirstOrDefault(x => x.Token == token);
            if (c != null)
            {
                ViewBag.GroupMessage = "This Code of Project  is  Found ! ......Plase Enter another Code";
                return false;
            }
            else
            {
                return true;
            }
        }
        // <summary>
        /// ///////////GETchatbox
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>

        [HttpGet]

        public ActionResult GetChatbox(string token)
        {
            if(token==null)
            { return HttpNotFound(); }
            
                ChatboxModel ChatboxModel = new AppServices().GetChatbox(token);
                return PartialView("~/Views/Partials/_ChatBox.cshtml", ChatboxModel);
            
        }

        /// <summary>
        /// //SendMessage and file
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult SendMessage(string GroupId, string message)
        {
            return Json(new AppServices().SendMessage(GroupId, message));
        }

        /// <summary>
        /// files
        /// </summary>
        /// <param name="token"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult SendFile(string token, HttpPostedFileBase file)
        {
            if (new AppServices().SendFiles(token, file) == true)
                return RedirectToAction("FileDetail", new { token = token });
            else

                return RedirectToAction("IndexAdmin", new { token = token });

        }


        [HttpGet]
        public PartialViewResult FileDetail(string token)
        {
           var list = db.Message.Where(x => x.Token == token && x.FileName != null).Select(x => new MyFiles { Id = x.id, FileName = x.FileName, ContentFile = x.FileContent,Time=x.Time,
                Token = x.Token ,UserID= x.FromUserId}).ToList();

            return PartialView("_FilesFromMsg", new FileDetails { files = list, Token = token });
        }


        [HttpGet]
        public FileResult DownLoadFileMessage(string token, int id)
        {
            var list = db.Message.Where(x => x.Token == token && x.FileName != null).Select(x => new MyFiles { Id = x.id, FileName = x.FileName, ContentFile = x.FileContent }).ToList();

            var file = list.FirstOrDefault(x => x.Id == id);

            return File(file.ContentFile, "files", file.FileName);

        }
        /// <summary>
        /// Delet File
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult DeleteFile(string Token, int id)
        {
            var message = db.Message.FirstOrDefault(x => x.id == id);
            if (message != null)
            {
                db.Message.Remove(message);
                db.SaveChanges();
                return RedirectToAction("FileDetail", new { token = Token });
            }
            else
                return RedirectToAction("IndexAdmin", new { token = Token });

        }


        /// <summary>
        /// Delet User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public PartialViewResult StudentMangment(string token)
        {
            var IdUser = int.Parse(HttpContext.User.Identity.Name);
            if (CheckType(IdUser))
            {

                var list = db.Student.Where(x => x.Token == token).ToList();


                return PartialView("_UserManagment", new UserManagment { Token = token, StudentList = list });
            }
            return PartialView("notfound");

        }

        
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteUser(int id, string token)
        {
            var IdUser = int.Parse(HttpContext.User.Identity.Name);
            if (CheckType(IdUser))
            {
                var Student = db.Student.FirstOrDefault(x => x.Id == id);
                if (Student != null)
                {
                    var Messag = db.Message.Where(x => x.FromUserId == id).ToList();
                    var connection = db.UserConnection.Where(y => y.UserId == Student.Id).ToList();
                    var role = db.Role.FirstOrDefault(x=>x.UserId==id);
                    User user = db.User.Find(Student.Id);

                    db.Message.RemoveRange(Messag);
                    db.Role.Remove(role);
                    db.UserConnection.RemoveRange(connection);
                    db.Student.Remove(Student);
                    db.User.Remove(user);
                    db.SaveChanges();
                }
                return RedirectToAction("StudentMangment", new { token = token });
            }
            return HttpNotFound();
      

        }
        /////////////////////
        ///
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ckeckToken(id))
            {
                Project project = db.Project.Find(id);
                return View(project);
            }
            return HttpNotFound();

        }
        public bool ckeckToken(string token)
        {
            int id = int.Parse(User.Identity.Name);
            var c = db.Student.FirstOrDefault(x => x.Token == token && x.Id==id);
            if (c != null)
            {
                return true;
            }

            else
                return false;
        }

        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult DetailsForStudent(int id,string token)
        {
            Student stude = db.Student.Find(id);
            if (stude == null)
            {
                return HttpNotFound();
            }
            return View(new DetailsForStudent { Student=stude,Token=token});
        }


    }
}