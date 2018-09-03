using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class EditZwierzeViewModel
    {
        public Zwierze Zwierze { get; set; }
        public IEnumerable<Gatunek> Gatunki { get; set; }
        public bool? Potwierdzenie { get; set; }
    }
}