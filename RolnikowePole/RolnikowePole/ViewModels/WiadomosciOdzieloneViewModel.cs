using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.ViewModels
{
    public class WiadomosciOdzieloneViewModel
    {
        public List<WiadomosciViewModel> WiadomosciWyslane { get; set; }
        public List<WiadomosciViewModel> WiadomosciOtrzymane { get; set; }
    }
}