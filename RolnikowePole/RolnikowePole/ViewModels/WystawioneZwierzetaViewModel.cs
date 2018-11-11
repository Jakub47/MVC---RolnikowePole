using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class WystawioneZwierzetaViewModel
    {
        public Zwierze WystawioneZwierzeta;
        public IEnumerable<Zdjecie> Zdjecia { get; set; }
    }
}