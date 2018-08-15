using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RolnikowePole.DAL
{
    public class RolnikowePoleInitializer : DropCreateDatabaseAlways<RolnikowePoleContext>
    {
        protected override void Seed(RolnikowePoleContext context)
        {
            SeedRolnikowePoleData(context);
            base.Seed(context);
        }

        private void SeedRolnikowePoleData(RolnikowePoleContext context)
        {
            var gatunki = new List<Gatunek>
            {
                new Gatunek() {GatunekId = 1, NazwaGatunku = "Konie", NazwaPlikuIkony="kon.png", OpisGatunku = "Konie"},
                new Gatunek() {GatunekId = 2, NazwaGatunku = "Bydlo Domowe", NazwaPlikuIkony="bydlo.png", OpisGatunku = "Bydlo domowe"},
                new Gatunek() {GatunekId = 3, NazwaGatunku = "Kury", NazwaPlikuIkony="kura.png", OpisGatunku = "Kury"},
                new Gatunek() {GatunekId = 4, NazwaGatunku = "Kaczki", NazwaPlikuIkony="kaczka.png", OpisGatunku = "Kaczki"},
                new Gatunek() {GatunekId = 5, NazwaGatunku = "Gesi", NazwaPlikuIkony="ges.png", OpisGatunku = "Gesi"},
                new Gatunek() {GatunekId = 6, NazwaGatunku = "Indyki", NazwaPlikuIkony="indyk.png", OpisGatunku = "Indyki"},
                new Gatunek() {GatunekId = 7, NazwaGatunku = "Swinie", NazwaPlikuIkony="swinia.png", OpisGatunku = "Swinie"},
                new Gatunek() {GatunekId = 8, NazwaGatunku = "Owce", NazwaPlikuIkony="owca.png", OpisGatunku = "Owce"},
                new Gatunek() {GatunekId = 9, NazwaGatunku = "Kozy", NazwaPlikuIkony="koza.png", OpisGatunku = "Kozy"},
                new Gatunek() {GatunekId = 10, NazwaGatunku = "Pszczoly", NazwaPlikuIkony="pszczola.png", OpisGatunku = "Pszczoly"},
                new Gatunek() {GatunekId = 11, NazwaGatunku = "Kroliki", NazwaPlikuIkony="krolik.png", OpisGatunku = "Kroliki"},
            };

            gatunki.ForEach(i => context.Gatunki.Add(i));
            context.SaveChanges();


            var zwierzeta = new List<Zwierze>
            {
                new Zwierze() { ZwierzeId = 1, GatunekId = 1, Nazwa = "Tomek", DataNarodzin = new DateTime(1997,3,1), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Tomek.png", OpisZwierza = "Tomek to pogodny koń",CenaZwierza = 700,Wojewodztwo = "Pomorskie",Miasto = "Gdańsk", },
                new Zwierze() { ZwierzeId = 2, GatunekId = 2, Nazwa = "Jan", DataNarodzin = new DateTime(2002,6,17), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Jan.png", OpisZwierza = "Jan to miła krowa",CenaZwierza = 455,Wojewodztwo = "Lódzkie",Miasto = "Łódź", Wyrozniony = true },
                new Zwierze() { ZwierzeId = 3, GatunekId = 3, Nazwa = "Krowa", DataNarodzin = new DateTime(1998,8,13), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Krowa.png", OpisZwierza = "Panie To je Krowa!",CenaZwierza = 60,Wojewodztwo = "Małopolskie",Miasto = "Kraków", },
                new Zwierze() { ZwierzeId = 4, GatunekId = 4, Nazwa = "Marek", DataNarodzin = new DateTime(1999,4,5), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Marek.png", OpisZwierza = "Kaczka dobra do upieczenia",CenaZwierza = 678,Wojewodztwo = "Małopolskie",Miasto = "Tarnów", },
                new Zwierze() { ZwierzeId = 5, GatunekId = 5, Nazwa = "Krzysztof", DataNarodzin = new DateTime(2005,7,6), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Krzysztof.png", OpisZwierza = "Krzysztof Gęś z dziećmi",CenaZwierza = 1200,Wojewodztwo = "Lubelskie",Miasto = "Lublin", Wyrozniony = true },
                new Zwierze() { ZwierzeId = 6, GatunekId = 6, Nazwa = "Anna", DataNarodzin = new DateTime(2007,8,7), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Anna.png", OpisZwierza = "Indik!",CenaZwierza = 754,Wojewodztwo = "Zachodniopomorskie",Miasto = "Szczecin", },
                new Zwierze() { ZwierzeId = 7, GatunekId = 7, Nazwa = "Magda", DataNarodzin = new DateTime(2018,2,8), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Magda.png", OpisZwierza = "Polecam dobre mięso",CenaZwierza = 20,Wojewodztwo = "Śląskie",Miasto = "Katowice", },
                new Zwierze() { ZwierzeId = 8, GatunekId = 8, Nazwa = "Jan", DataNarodzin = new DateTime(2015,4,7), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Jan.png", OpisZwierza = "Dobry Włos :) ",CenaZwierza = 75,Wojewodztwo = "Pomorskie",Miasto = "Gdynia", Wyrozniony = true },
                new Zwierze() { ZwierzeId = 9, GatunekId = 9, Nazwa = "Jakub", DataNarodzin = new DateTime(1997,7,11), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Jakub.png", OpisZwierza = "Najlepsze mleko we wsi",CenaZwierza = 687,Wojewodztwo = "Śląskie",Miasto = "Sosnowiec", },
                new Zwierze() { ZwierzeId = 10, GatunekId = 10, Nazwa = "Tomek", DataNarodzin = new DateTime(2013,6,15), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Tomek.png", OpisZwierza = "Miód pirewsza klasa",CenaZwierza = 777,Wojewodztwo = "Kujawsko-pomorskie",Miasto = "Bydgoszcz ", },
                new Zwierze() { ZwierzeId = 11, GatunekId = 11, Nazwa = "Jakub", DataNarodzin = new DateTime(2011,5,16), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Jakub.png", OpisZwierza = "Rozmnażają się bardzo szybko",CenaZwierza = 2000,Wojewodztwo = "Pomorskie",Miasto = "Gdańsk", },
                new Zwierze() { ZwierzeId = 12, GatunekId = 1, Nazwa = "Wojtek", DataNarodzin = new DateTime(1995,2,10), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Wojtek.png", OpisZwierza = "Konik szybko biegnie",CenaZwierza = 354,Wojewodztwo = "Kujawsko-pomorskie",Miasto = "Toruń", },
                new Zwierze() { ZwierzeId = 13, GatunekId = 2, Nazwa = "Anna", DataNarodzin = new DateTime(2000,5,2), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Anna.png", OpisZwierza = "Dobre mięcho",CenaZwierza = 7854,Wojewodztwo = "Podlaskie",Miasto = "Białystok", },
                new Zwierze() { ZwierzeId = 14, GatunekId = 3, Nazwa = "Ewa", DataNarodzin = new DateTime(2007,4,2), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Ewa.png", OpisZwierza = "Dodam Koguta :)",CenaZwierza = 45,Wojewodztwo = "Zachodniopomorskie",Miasto = "Koszalin", },
                new Zwierze() { ZwierzeId = 15, GatunekId = 4, Nazwa = "Kajetan", DataNarodzin = new DateTime(2018,7,3), DataDodania = DateTime.Now, NazwaPlikuObrazka = "Kajetan.png", OpisZwierza = "Znosi złote Jaja",CenaZwierza = 35,Wojewodztwo = "Lubelskie",Miasto = "Chełm", },
            };

            zwierzeta.ForEach(i => context.Zwierzeta.Add(i));
            context.SaveChanges();
        }
    }
}