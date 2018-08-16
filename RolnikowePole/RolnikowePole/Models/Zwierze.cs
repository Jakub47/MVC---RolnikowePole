using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RolnikowePole.Models
{
    public class Zwierze
    {
        public int ZwierzeId { get; set; }
        public int GatunekId { get; set; }
        public string Rasa { get; set; }
        [Required(ErrorMessage = "Wprowadz Nazwe Zwierza")]
        [StringLength(100)]
        public string Nazwa { get; set; }
        [Required(ErrorMessage = "Wprowadz Date Narodzin Zwierza")]
        public DateTime DataNarodzin { get; set; }
        public DateTime DataDodania { get; set; }
        [StringLength(100)]
        public string NazwaPlikuObrazka { get; set; }
        [Required(ErrorMessage = "Wprowadz Opis Zwierza")]
        public string OpisZwierza { get; set; }
        [Required(ErrorMessage = "Wprowadz Cene")]
        public decimal CenaZwierza { get; set; }
        public string Wojewodztwo { get; set; }
        public string Miasto { get; set; }

        public bool Odrobaczony { get; set; }
        public bool Szczepiony { get; set; }
        public bool Wykastrowany { get; set; }
        public bool Wyrozniony { get; set; }
        public bool Ukryty { get; set; }
        public string OpisSkrocony { get; set; }

        public virtual Gatunek Gatunek { get; set; }
    }
}