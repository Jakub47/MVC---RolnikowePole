using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class EmailDoAdmina : Email
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
    }
}