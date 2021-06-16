using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class UserManagment
    {
        public string  Token { get; set; }
        public List<Student> StudentList { get; set; }
    }
}