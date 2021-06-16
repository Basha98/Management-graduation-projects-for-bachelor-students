using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using App.Models;
using App.Servier;
using System.Threading.Tasks;

namespace App.Hubs
{
    public class ChatHubs : Hub
    {
        AppDBEntities db = new AppDBEntities();
        public static IHubContext contextOFF = GlobalHost.ConnectionManager.GetHubContext<ChatHubs>();
        AppServices Server = new AppServices();
        public void GetUser(string token)
        {
            List<UserDTO> Users = Server.GetUsersToChat(token);
            UserDTO admin = Server.GetAdminToChat(token);
            var x = Server.GetAllConnectionId(token);
            Clients.Clients(x).broadCastAdminToChat(admin);
            Clients.Clients(x).broadCastToChat(Users);
        }



        public override Task OnConnected()
        {
            int x = new AppServices().AddUserConnection(Guid.Parse(Context.ConnectionId));
            Clients.All.broadCastOnlineUser(x);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
          int x= new AppServices().RemovUserConnection(Guid.Parse(Context.ConnectionId));
            Clients.All.broadCastOfflineUser(x);
            return base.OnDisconnected(stopCalled);
        }

        public static void offlineUsers(int userid)
        {
            contextOFF.Clients.All.broadCastOfflineUser(userid);
        }

        public static void ReciveMessage( string ToGropToken,string type,string NameOfuser, string msg,string x,string  time)
        {
            var con = new AppServices().GetAllConnectionId(ToGropToken);
            contextOFF.Clients.Clients(con).BroadcastRecive(ToGropToken,type,NameOfuser, msg, x, time);
        }
        

    }
}