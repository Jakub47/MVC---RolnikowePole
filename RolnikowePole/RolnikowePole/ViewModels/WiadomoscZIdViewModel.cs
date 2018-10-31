using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class WiadomoscZIdViewModel
    {
        public Wiadomosc Wiadomosc { get; set; }
        public string UserID { get; set; }
        public string NazwaPliku { get; set; }
        public string NazwaZwierza { get; set; }
        public string ImieUzytkownika { get; set; }
        public string NazwiskoUzytkownika { get; set; }
        public string Email { get; set; }
    }
}