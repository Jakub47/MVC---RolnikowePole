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

        public ActionResult DodajDoKoszyka(int id)
        {
            koszykManager.DodajDoKoszyka(id);

            return RedirectToAction("Index");
        }

        public int PobierzIloscElementowKoszyka()
        {
            return koszykManager.PobierzIloscPozycjiKoszyka();
        }

        public ActionResult UsunZKoszyka(int ZwierzeId)
        {
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

        //Jezeli wykonujemy coś asynchroniczne (asynce) to rezultat musi zwracać  Task
        public async Task<ActionResult> Zaplac()
        {
            //Verifry whether user is loged in
            if(Request.IsAuthenticated)
            {
                //wiec obecnie zalogowany user ten jest uzyskiwany poprzez User.Identity.GetUserId()
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                //Create zamowienie from this User! Te dane jezeli juz sa w bazie to te dane zostana juz przypisane i uzytkownik nie bedzie ich musial ponownie wypelniac
                var zamowienie = new Zamowienie
                {
                    Imie = user.DaneUzytkownika.Imie,
                    Nazwisko = user.DaneUzytkownika.Nazwisko,
                    Adres = user.DaneUzytkownika.Adres,
                    Miasto = user.DaneUzytkownika.Miasto,
                    KodPocztowy = user.DaneUzytkownika.KodPocztowy,
                    Email = user.DaneUzytkownika.Email,
                    Telefon = user.DaneUzytkownika.Telefon
                };

                return View(zamowienie);
            }
            else
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Zaplac", "Koszyk") });
        }

        [HttpPost]
        public async Task<ActionResult> Zaplac(Zamowienie zamowienieSzczegoly)
        {
            if (ModelState.IsValid)
            {
                //pobieranie id użytkownika aktualnie zalogowanego
                var userId = User.Identity.GetUserId();

                //utworzenie obiektu zamówienia na podstawie tego co mamy w koszyku
                var newOrder = koszykManager.UtworzZamowienie(zamowienieSzczegoly, userId);

                //szczególy użytkownika - aktualizacja danych. Jezeli podczas skladania zamowienia jakies zaktualizuje dane np. imie
                //To te same dane zostana zaktualizowane w tabeli stad właśnie modyfikacja TryUpdate i UpdateAsync!
                var user = await UserManager.FindByIdAsync(userId);
                TryUpdateModel(user.DaneUzytkownika); //Jezeli bedziemy chcieli zmodyfikowac opis Patrz!
                await UserManager.UpdateAsync(user);

                //opróżnimy nasz koszyk zakupów
                koszykManager.PustyKoszyk();

                string url = Url.Action("PotwierdzenieZamowieniaEmail", "Koszyk", new { zamowienieId =
                    newOrder.ZamowienieId, nazwisko = newOrder.Nazwisko }, Request.Url.Scheme);
                
                //Przekazany zostanie url czy tym samym zostanie wywołana akcji w naszym kontrolerze odpowidzialna 
                //za wysłanie e-maila!
                BackgroundJob.Enqueue(() => Call(url));

                

                return RedirectToAction("PotwierdzenieZamowienia");
            }
            else
                return View(zamowienieSzczegoly);
        }

        public void Call(string url)
        {
            var req = HttpWebRequest.Create(url);
            req.GetResponseAsync();
        }

        public ActionResult PotwierdzenieZamowieniaEmail(int zamowienieId, string nazwisko)
        {
            //Zamówienie Które właśnie zostało wysłane
            var zamowienie = db.Zamowienia.Include("PozycjeZamowienia").Include("PozycjeZamowienia.zwierze")
                .SingleOrDefault(o => o.ZamowienieId == zamowienieId && o.Nazwisko == nazwisko);

            if (zamowienie == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            PotwierdzenieZamowieniaEmail email = new PotwierdzenieZamowieniaEmail()
            {
                To = zamowienie.Email,
                From = "jakub7249@gmail.com",
                Wartosc = zamowienie.WartoscZamowienia,
                NumerZamowienia = zamowienie.ZamowienieId,
                PozycjeZamowienia = zamowienie.PozycjeZamowienia
            };

            email.Send();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult PotwierdzenieZamowienia()
        {
            return View();
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