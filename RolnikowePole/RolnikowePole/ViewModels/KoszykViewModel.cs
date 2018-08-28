using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class KoszykViewModel
    {
        public List<Zwierze> PozycjeKoszyka { get; set; }
        public decimal CenaCalkowita { get; set; }
    }
}