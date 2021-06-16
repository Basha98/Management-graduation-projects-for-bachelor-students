using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public int ToGroup { get; set; }

        public string FullName { get; set; }
        public string UserName { get; set; }
        public bool IsOnline { get; set; }
    }
}