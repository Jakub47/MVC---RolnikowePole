using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class WiadomosciViewModel
    {
        public string NazwaUzytkownika { get; set; }
        public DateTime DataWyslania { get; set; }
        public string TrescWiadomosci { get; set; }
    }
}