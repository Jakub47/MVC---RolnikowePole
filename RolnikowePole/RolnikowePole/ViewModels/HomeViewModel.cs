using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class HomeViewModel
    {
        public Zwierze Zwierze { get; set; }
        public DaneUzytkownika daneUzytkownika { get; set; }
        public Wiadomosc wiadomosc { get; set; }
        public IEnumerable<Zdjecie> Zdjecia { get; set; }
        //public IEnumerable<Zwierze> Wyroznione { get; set; }
        public IEnumerable<Zwierze> Nowe { get; set; }
        
    }
}