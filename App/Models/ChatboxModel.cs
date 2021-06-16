using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class ChatboxModel
    {
        public string    TOGroup { get; set; }
        public List<MsgDTO> Messages { get; set; }

    }
}