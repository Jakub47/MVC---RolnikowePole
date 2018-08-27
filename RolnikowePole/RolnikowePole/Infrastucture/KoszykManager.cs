﻿using RolnikowePole.DAL;
using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.Infrastucture
{
    public class KoszykManager
    {
        private RolnikowePoleContext db;
        private ISessionManager session;

        public KoszykManager(ISessionManager session, RolnikowePoleContext context)
        {
            this.session = session;
            this.db = context;
        }

        public List<PozycjaKoszyka> PobierzKoszyk()
        {
            List<PozycjaKoszyka> koszyk;

            if(session.Get<List<PozycjaKoszyka>>(Consts.KoszykSessionKlucz) == null )
            {
                koszyk = new List<PozycjaKoszyka>();
            }
            else
            {
                koszyk = session.Get<List<PozycjaKoszyka>>(Consts.KoszykSessionKlucz) as List<PozycjaKoszyka>;
            }

            return koszyk;
        }

        public void DodajDoKoszyka(int zwierzeId)
        {
            var koszyk = PobierzKoszyk();
            var pozycjaKoszyka = koszyk.Find(k => k.Zwierze.ZwierzeId == zwierzeId);

            if (pozycjaKoszyka != null)
                pozycjaKoszyka.Ilosc++;
            else
            {
                var ZwierzeDoDodania = db.Zwierzeta.Where(k => k.ZwierzeId == zwierzeId).SingleOrDefault();

                if(ZwierzeDoDodania != null)
                {
                    var nowaPozycjaKoszyka = new PozycjaKoszyka()
                    {
                        Zwierze = ZwierzeDoDodania,
                        Ilosc = 1,
                        Wartosc = ZwierzeDoDodania.CenaZwierza
                    };
                    koszyk.Add(nowaPozycjaKoszyka);
                }
            }

            session.Set(Consts.KoszykSessionKlucz,koszyk);
        }

        public int UsunZKoszyka(int ZwierzeId)
        {
            var koszyk = PobierzKoszyk();
            var pozycjaKoszyka = koszyk.Find(k => k.Zwierze.ZwierzeId == ZwierzeId);

            if(pozycjaKoszyka != null)
            {
                //Check wheter the pozycja exit and check how many of such element is currently in Koszyk. If less than 1 then remove whole item else just one element
                if(pozycjaKoszyka.Ilosc > 1)
                {
                    pozycjaKoszyka.Ilosc--;

                    return pozycjaKoszyka.Ilosc;
                }
                else
                {
                    koszyk.Remove(pozycjaKoszyka);
                }
            }

            return 0;
        }

        public decimal PobierzWartoscKoszyka()
        {
            var koszyk = PobierzKoszyk();
            return koszyk.Sum(i => (i.Wartosc * i.Ilosc));
        }


    }
}