using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RolnikowePole.Models
{
    public class Gatunek
    {
        public int GatunekId { get; set; }
        [Required(ErrorMessage = "Wprowadz Nazwe Gatunku")]
        [StringLength(100)]
        public string NazwaGatunku { get; set; }
        [Required(ErrorMessage = "Wprowadz Opis Gatunku")]
        public string OpisGatunku { get; set; }
        public string NazwaPlikuIkony { get; set; }

        public virtual ICollection<Zwierze> Zwierzeta { get; set; }
    }
}