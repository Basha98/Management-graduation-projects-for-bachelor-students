using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class MyFiles
    {

        public int UserID { get; set; }
        public string Token { get; set; }
        public string Time { get; set; }
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] ContentFile { get; set; }
    }
}