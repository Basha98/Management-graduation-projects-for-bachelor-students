using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class DetailsForStudent
    {
        public string Token { get; set; }
        public Student Student { get; set; }
    }
}