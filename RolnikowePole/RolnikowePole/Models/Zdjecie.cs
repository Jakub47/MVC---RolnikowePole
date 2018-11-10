using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.Models
{
    public class Zdjecie
    {
        public int ZdjecieID { get; set; }
        public string FilePath { get; set; }

        public int ZwierzeId { get; set; }
        public virtual Zwierze Zwierze { get; set; }
    }
}