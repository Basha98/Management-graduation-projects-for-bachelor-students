using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using App.Models;

namespace App.Servier
{
    public class AppServices
    {
        AppDBEntities _context;
        public Guid PconnectionId;
        public AppServices()
        {
            _context = new AppDBEntities();
        }

      
        public bool loginAdmin(loginData Userlog, out int userId)
        {
            userId = 0;
            var admin = _context.Supervisor.FirstOrDefault(x => x.User.Email == Userlog.Email && x.User.Password == Userlog.Password);

            if (admin != null)
            {
                userId = admin.Id;
                return true;
            }

            return false;
        }
        public bool loginStudent(loginData Userlog, out int userId)
        {
            userId = 0;
            var student = _context.Student.FirstOrDefault(x => x.User.Email == Userlog.Email && x.User.Password == Userlog.Password);

            if (student != null)
            {
                userId = student.Id;
                return true;
            }

            return false;

        }

        public List<string> GetAllConnectionId(string token)
        {
            List<string> Connection = new List<string>();
            var admins = GetAdminToChat(token);
            var students = GetUsersToChat(token);

            Connection.AddRange(GetUSerConnections(admins.UserId));
            foreach (UserDTO x in students)
            {
                Connection.AddRange(GetUSerConnections(x.UserId));
            }
            return Connection;
        }
        internal IList<string> GetUSerConnections(int uSerId)
        {
            return _context.UserConnection.Where(x => x.UserId == uSerId).Select(x => x.ConnectionId.ToString()).ToList();
        }

        internal string[] Admns()
        {
            int z=0;
            var list= _context.Supervisor.Select(x => x.Id.ToString()).ToList();
            string[] a = new string[list.Count];
            foreach (string  x in list) 
            {
                a[z++] = x;
            }
            return a;
        }

        public UserDTO GetAdminToChat(string token)
        {
            var AdminId = _context.Project.FirstOrDefault(i => i.Token == token).Supervisor_Id;
           
            var x = _context.User.FirstOrDefault(y => y.Id == AdminId);
                return new UserDTO
                {
                    UserId = x.Id,
                    UserName = x.Email,
                    FullName = x.FullName,

                    IsOnline = x.UserConnection.Count > 0
                };
        }

        ////لجلب جميع المستخدمين 
        public List<UserDTO> GetUsersToChat( string token)
        {
            return _context.Student.Where(x => x.Token ==token).Select(x => new UserDTO
            {
                UserId = x.User.Id,
                UserName = x.User.Email,
                FullName = x.User.FullName,

                IsOnline =x.User.UserConnection.Count > 0
            }).ToList();


        }


        public int AddUserConnection(Guid guid)
        {
            int IdUser = int.Parse(HttpContext.Current.User.Identity.Name);
                _context.UserConnection.Add(new UserConnection { UserId = IdUser, ConnectionId = guid });
                _context.SaveChanges();
            return IdUser;
        }

        internal int RemovUserConnection(Guid ConnectionId)
        {
            int UserId = 0;
            UserConnection current = _context.UserConnection.FirstOrDefault(x => x.ConnectionId == ConnectionId);
            if (current != null)
            {
                UserId = current.Id;
                _context.UserConnection.Remove(current);
                _context.SaveChanges();

            }

            return UserId;
        }
        

    public List<string> UserOnline(int UseId)
    {
        return _context.UserConnection.Where(x => x.UserId == UseId).Select(x => x.ConnectionId.ToString()).ToList();

    }

        internal void RemovAllUserConnection(int UserID)
    {
        var current = _context.UserConnection.Where(x => x.UserId == UserID).ToList();

        _context.UserConnection.RemoveRange(current);
        _context.SaveChanges();
           
        }


    public ChatboxModel GetChatbox(string token)
        {
            App.Controllers.MainController nn = new Controllers.MainController();
            int UserId = int.Parse(HttpContext.Current.User.Identity.Name);

        var Msg = _context.Message.Where(x => (x.Token == token&&x.FileName==null))

            .Select(x => new MsgDTO { idUSER=x.FromUserId, Message = x.MessageContent ,Time=x.Time,Class = x.FromUserId == UserId ? "from" : "to" }).ToList();

        return new ChatboxModel { TOGroup = token, Messages = Msg };
    }


      

        


        public bool SendMessage(string token, string Msg)
        {
            try
            {
               int FromUserID = int.Parse(HttpContext.Current.User.Identity.Name);
                var NameOfuser = _context.User.FirstOrDefault(x => x.Id == FromUserID).FullName;
                _context.Message.Add(new Message
                {
                    FromUserId = FromUserID,
                    Token = token,
                    MessageContent = Msg,
                    Time=Time()

                });
                _context.SaveChanges();
                Hubs.ChatHubs.ReciveMessage(token,CheckType(FromUserID), NameOfuser,Msg, HttpContext.Current.User.Identity.Name,Time() );
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string CheckType(int IdUser)
        {
            var curentUser = _context.Supervisor.FirstOrDefault(x => x.Id == IdUser);
            if (curentUser != null)
                return "superviser_2.png";
            else
                return "male-student-icon.jpg";
        }
        public string Time()
        {
            var remoteTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time");
            var remoteTime = TimeZoneInfo.ConvertTime(DateTime.Now, remoteTimeZone);
        return(remoteTime.ToLocalTime().ToUniversalTime().ToString());
        }


        public bool SendFiles(string token, HttpPostedFileBase file)
        {
            try
            {


                int FromUserID = int.Parse(HttpContext.Current.User.Identity.Name);
                Stream str = file.InputStream;
                BinaryReader Br = new BinaryReader(str);
                Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                _context.Message.Add(new Message
                {
                    FileName = file.FileName,
                    FileContent = FileDet,
                    FromUserId = FromUserID,
                    Token =token,
                    Time=Time()
                });
                _context.SaveChanges(); 
                return true;
            }
            catch
            {
                return false;
            }
        }



  
}


}
