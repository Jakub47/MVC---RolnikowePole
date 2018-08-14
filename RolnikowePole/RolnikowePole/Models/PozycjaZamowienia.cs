namespace RolnikowePole.Models
{
    public class PozycjaZamowienia
    {
        public int PozycjaZamowieniaId { get; set; }
        public int ZamowienieId { get; set; }
        public int ZwierzeId { get; set; }
        public int MyProperty { get; set; }
        public int Ilosc { get; set; }
        public decimal CenaZakupu { get; set; }

        public virtual Zwierze zwierze { get; set; }
        public virtual Zamowienie zamowienie { get; set; }
    }
}