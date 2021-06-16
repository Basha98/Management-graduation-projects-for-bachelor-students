using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class MsgDTO
    {
        public string  Message { get; set; }
        public int idUSER { get; set; }
        public String  Time { get; set; }
        public string Class { get; set; }
    }
}