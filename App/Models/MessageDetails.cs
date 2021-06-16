using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class MessageDetails
    {
        public string MessageContent { get; set; }
        public HttpPostedFileBase file { set; get; }
    }
}