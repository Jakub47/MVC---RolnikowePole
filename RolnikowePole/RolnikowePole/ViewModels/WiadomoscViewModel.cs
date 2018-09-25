using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class WiadomoscViewModel
    {
        public IEnumerable<Wiadomosc> ListaWiadomosci { get; set; }
        public Wiadomosc wiadomosc { get; set; }
    }
}