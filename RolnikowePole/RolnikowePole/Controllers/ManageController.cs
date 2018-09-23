using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MoreLinq;
using RolnikowePole.App_Start;
using RolnikowePole.DAL;
using RolnikowePole.Infrastucture;
using RolnikowePole.Models;
using RolnikowePole.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RolnikowePole.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private RolnikowePoleContext db = new RolnikowePoleContext();

        public enum ManageMessageId
        {
            ChangePasswordSuccess, //Jezeli zmiana hasła była sukcesem 
            Error //jeżeli jest jakiś bląd
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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: Manage
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }

            if (User.IsInRole("Admin"))
                ViewBag.UserIsAdmin = true;
            else
                ViewBag.UserIsAdmin = false;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }

            var model = new ManageCredentialsViewModel
            {
                Message = message,
                DaneUzytkownika = user.DaneUzytkownika
            };

            return View(model); //Możliwość zmiany
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeProfile([Bind(Prefix = "DaneUzytkownika")]DaneUzytkownika daneUzytkownika)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId()); //Najpierw bedzie pobierany użytkwonik
                user.DaneUzytkownika = daneUzytkownika; //jego dane zostana pobrane 
                var result = await UserManager.UpdateAsync(user); //a nastepnie uktualnione 

                AddErrors(result);
            }

            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index"); //Zostana przeslane dane ktore beda niepoprawne zwalidowane i przekierowanie do akcji index
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(Prefix = "ChangePasswordViewModel")]ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);

            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var message = ManageMessageId.ChangePasswordSuccess;
            return RedirectToAction("Index", new { Message = message });
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("password-error", error);
            }
        }

        //Zalogowanie asynchroniczne 
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        public ActionResult ListaWystawionychZwierzakow()
        {
            //Check if current user is admin
            bool IsAdmin = User.IsInRole("Admin");
            ViewBag.UserIsAdmin = IsAdmin;

            List<Zwierze> WystawioneZwierzeta;

            //Dla administratorów zwracamy wszystkie zamowienia
            if (IsAdmin)
            {
                WystawioneZwierzeta = db.Zwierzeta.ToList();
            }
            else
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                WystawioneZwierzeta = db.Zwierzeta.Where(a => a.UserId == user.Id).ToList();
                //ZamowieniaUzytkownika = db.Zamowienia.Where(o => o.UserId == userId).Include("PozycjeZamowienia").OrderByDescending(o => o.DataDodania).ToArray();
            }

            return View(WystawioneZwierzeta);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public StanZamowienia ZmianaStanuZamowienia(Zamowienie zamowienie)
        {
            //Pobierz Zamowienie z Bazy
            Zamowienie zamowienieDoModyfikacji = db.Zamowienia.Find(zamowienie.ZamowienieId);
            zamowienieDoModyfikacji.StanZamowienia = zamowienie.StanZamowienia;
            db.SaveChanges();

            return zamowienie.StanZamowienia;
        }


        [Authorize(Roles = "Admin")]
        public ActionResult UkryjZwierza(int zwierzeId)
        {
            var zwierze = db.Zwierzeta.Find(zwierzeId);
            zwierze.Ukryty = true;
            db.SaveChanges();

            return RedirectToAction("DodajZwierze", new { potwierdzenie = true });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult PokazZwierza(int zwierzeId)
        {
            var zwierze = db.Zwierzeta.Find(zwierzeId);
            zwierze.Ukryty = false;
            db.SaveChanges();

            return RedirectToAction("DodajZwierze", new { potwierdzenie = true });
        }


        public ActionResult DodajZwierze(int? zwierzeId, bool? potwierdzenie)
        {
            Zwierze zwierze;
            if (zwierzeId.HasValue)
            {
                //Edycja Kursu
                ViewBag.EditMode = true;
                zwierze = db.Zwierzeta.Find(zwierzeId);
            }
            else
            {
                //Dodanie nowego kursu
                ViewBag.EditMode = false;
                zwierze = new Zwierze();
            }

            var result = new EditZwierzeViewModel()
            {
                Gatunki = db.Gatunki.ToList(),
                Zwierze = zwierze,
                Potwierdzenie = potwierdzenie
            };

            return View(result);
        }


        [HttpPost]
        public ActionResult DodajZwierze(EditZwierzeViewModel model, HttpPostedFileBase file)
        {
            //Patrz pola ukryte w widoku
            if (model.Zwierze.ZwierzeId > 0)
            {
                //Modyfikacja Zwierzaka!
                db.Entry(model.Zwierze).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DodajZwierze", new { potwierdzenie = true });
            }
            else
            {
                //Sprawdzenie czy uzytkownik wybral plik
                if (file != null || file.ContentLength > 0)
                {
                    //Czy Pozostałe pola zostały wypełnione poprawnie
                    if (ModelState.IsValid)
                    {
                        //Generowanie plik
                        var fileExt = Path.GetExtension(file.FileName);
                        var filename = Guid.NewGuid() + fileExt; // Unikalny identyfikator + rozszerzenie

                        //W jakim folderze ma byc umiesczony dany plik oraz jego nazwa! Oraz zapis
                        var path = Path.Combine(Server.MapPath(AppConfig.ObrazkiFolderWzgledny), filename);
                        file.SaveAs(path);

                        model.Zwierze.NazwaPlikuObrazka = filename;
                        model.Zwierze.DataDodania = DateTime.Now;
                        model.Zwierze.UserId = User.Identity.GetUserId();
                        //Oczywiscie mozna wykonac standardowa procedure db.Zwierze.Add(); db.SaveChanges(), ale...
                        db.Entry(model.Zwierze).State = EntityState.Added;
                        var user = UserManager.FindById(User.Identity.GetUserId());
                        
                        db.SaveChanges();

                        return RedirectToAction("DodajZwierze", new { potwierdzenie = true });
                    }
                    else
                    {
                        var gatunki = db.Gatunki.ToList();
                        model.Gatunki = gatunki;
                        return View(model);
                    }

                }

                //Co gdy użytkownik nie wybral pliku
                else
                {
                    ModelState.AddModelError("", "Nie wskazano pliku");
                    //Model zostanie zwrocony, ponieważ w drpodown liście nie zostaną wyświetlone elementy! stąd musimy je jeszcze
                    //raz pobrać żeby poprostu zostały pokazane!
                    var gatunki = db.Gatunki.ToList();
                    model.Gatunki = gatunki;
                    return View(model);
                }
            }
        }

        public ActionResult WyswietlWiadomosciUzytkownika()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            //Jak ja wysyłam to mnie nie obchodzi wysyłający tylko odbiorca
            //Jak ja odbieram to mnie nie obchodzi odbierający tylko wysyłający

            //List of all users id !
            var GetUsersIDSended = user.SenderMessages.Where(a => a.ReceiverId != user.Id).Select(b => b.ReceiverId).ToList();
            var GetUsersIDRetrived = user.ReceiverMessages.Where(a => a.SenderId != user.Id).Select(b => b.SenderId).ToList();


            //Potrzebuje nazwyUzytkownika + Daty + Tresci kazdej wiadomosci
            var wiadomosci = new List<WiadomosciViewModel>();
            var w = new WiadomosciOdzieloneViewModel();

            var wiadomosciWyslane = user.SenderMessages.Where(a => a.SenderId == user.Id).OrderByDescending(a => a.DateAndTimeOfSend).DistinctBy(a => a.ReceiverId).ToList();

            //GIT
            var wiadomosciOtrzymane = user.ReceiverMessages.Where(a => a.ReceiverId == user.Id).OrderByDescending(a => a.DateAndTimeOfSend).DistinctBy(a => a.SenderId).ToList();

            wiadomosciWyslane.ForEach(a =>
            {
                //wiadomosci.Add(new WiadomosciViewModel()
                //{
                //    NazwaUzytkownika = a.Receiver.DaneUzytkownika.Imie + " " +  a.Receiver.DaneUzytkownika.Nazwisko,
                //    DataWyslania = a.DateAndTimeOfSend,
                //    TrescWiadomosci = a.Body
                //});

                var z = new WiadomosciViewModel()
                {
                    NazwaUzytkownika = a.Receiver.DaneUzytkownika.Imie + " " + a.Receiver.DaneUzytkownika.Nazwisko,
                    DataWyslania = a.DateAndTimeOfSend,
                    TrescWiadomosci = a.Body,
                    Id = a.ReceiverId,
                    Zwierze = db.Zwierzeta.Where(b => b.ZwierzeId == a.ZwierzeId).FirstOrDefault()
                };

                w.WiadomosciWyslane.Add(z);
            });

            wiadomosciOtrzymane.ForEach(a =>
            {
                //wiadomosci.Add(new WiadomosciViewModel()
                //{
                //    NazwaUzytkownika = a.Receiver.DaneUzytkownika.Imie + " " + a.Receiver.DaneUzytkownika.Nazwisko,
                //    DataWyslania = a.DateAndTimeOfSend,
                //    TrescWiadomosci = a.Body
                //});

                var z = new WiadomosciViewModel()
                {
                    NazwaUzytkownika = a.Receiver.DaneUzytkownika.Imie + " " + a.Receiver.DaneUzytkownika.Nazwisko,
                    DataWyslania = a.DateAndTimeOfSend,
                    TrescWiadomosci = a.Body,
                    Id = a.SenderId,
                    Zwierze = db.Zwierzeta.Where(b => b.ZwierzeId == a.ZwierzeId).FirstOrDefault()
                };

                w.WiadomosciOtrzymane.Add(z);
            });


            return View(w);

            //var WszyscyUserzy = UserManager.Users;

            //foreach (var userr in UserManager.Users)
            //{
            //    wiadomosciOtrzymane.ForEach(a =>
            //    {
                    
            //    });
            //}

            //Wziąć ostatnią wiadomość i id użytkownika innego niz zalogowany 
            //Co wziac


            //var wiadomoscVM = new WiadomosciViewModel
            //{
            //    WiadomosciWyslane = wiadomosciWyslane,
            //    WiadomosciOtrzymane = wiadomosciOtrzymane
            //};

            //return View(wiadomoscVM);
        }

        public string WiadomosciLista()
        {
            return "";
        }

        [HttpPost]
        public string WyslijWiadomosc(Wiadomosc wiadomosc)
        {

            //db.Users.Find(wiadomosc.ReceiverId).ReceiverMessages.Add(wiadomosc);
            //db.Users.Find(wiadomosc.SenderId).SenderMessages.Add(wiadomosc);

            db.Wiadomosci.AddOrUpdate(wiadomosc);

            try
            {
                db.SaveChanges();

            }
            catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    // Get entry

                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    // Display or log error messages

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        string message = string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                        //Console.WriteLine(message);
                        System.Diagnostics.Debug.WriteLine(message);
                    }
                }
            }

            return "dzieki stary cwelu";
        }

    }
}