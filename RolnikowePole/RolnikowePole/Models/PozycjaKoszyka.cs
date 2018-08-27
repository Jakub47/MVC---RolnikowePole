using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.Models
{
    public class PozycjaKoszyka
    {
        public Zwierze Zwierze { get; set; }
        public int Ilosc { get; set; }
        public decimal Wartosc { get; set; }
    }
}