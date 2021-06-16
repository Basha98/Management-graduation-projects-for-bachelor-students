using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class FileDetails
    {
        public List<MyFiles> files { get; set; }
        public string  Token { get; set; }

    }
}