using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class WiadomosciViewModel
    {
        public IEnumerable<Wiadomosc> WiadomosciWyslane { get; set; }
        public IEnumerable<Wiadomosc> WiadomosciOtrzymane { get; set; }
    }
}