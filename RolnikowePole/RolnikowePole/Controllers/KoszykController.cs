using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RolnikowePole.App_Start;
using RolnikowePole.DAL;
using RolnikowePole.Infrastucture;
using RolnikowePole.Models;
using RolnikowePole.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RolnikowePole.Controllers
{
    public class KoszykController : Controller
    {
        private KoszykManager koszykManager;
        private ISessionManager sessionManager { get; set; }
        private RolnikowePoleContext db;

        public KoszykController()
        {
            db = new RolnikowePoleContext();
            sessionManager = new SessionManager();
            koszykManager = new KoszykManager(sessionManager, db);
        }

        // GET: Koszyk
        public ActionResult Index()
        {
            var pozycjaKoszyka = koszykManager.PobierzKoszyk();
            var cenaCalkowita = koszykManager.PobierzWartoscKoszyka();
            KoszykViewModel koszykVM = new KoszykViewModel()
            {
                PozycjeKoszyka = pozycjaKoszyka,
                CenaCalkowita = cenaCalkowita
            };

            return View(koszykVM);
        }

        public ActionResult DodajDoKoszyka(int? id)
        {
            if (!id.HasValue && id.Value <= 0)
            {
                return RedirectToAction("Index");
            }

            koszykManager.DodajDoKoszyka(id);

            return RedirectToAction("Index");
        }

        public int PobierzIloscElementowKoszyka()
        {
            return koszykManager.PobierzIloscPozycjiKoszyka();
        }

        public ActionResult UsunZKoszyka(int ZwierzeId)
        {
            if(ZwierzeId <= 0)
            {
                return null;
            }

            int iloscPozycji = koszykManager.UsunZKoszyka(ZwierzeId);
            int iloscPozycjiKoszyka = koszykManager.PobierzIloscPozycjiKoszyka();
            decimal wartoscKoszyka = koszykManager.PobierzWartoscKoszyka();

            var wynik = new KoszykUsuwanieViewModel
            {
                IdPozycjiUsuwanej = ZwierzeId,
                IloscPozycjiUsuwanej = iloscPozycji,
                KoszykCenaCalkowita = wartoscKoszyka,
                KoszykIloscPozycji = iloscPozycjiKoszyka
            };
            return Json(wynik);
        }
        

        public void Call(string url)
        {
            var req = HttpWebRequest.Create(url);
            req.GetResponseAsync();
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

    }
}