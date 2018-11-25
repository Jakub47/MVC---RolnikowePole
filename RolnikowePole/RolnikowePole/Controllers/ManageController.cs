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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
        public async Task<ActionResult> Index(ManageMessageId? message, string info = null)
        {
            if (info != null)
                ViewBag.Info = info;

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

        public ActionResult ListaWystawionychZwierzakow(Zwierze pozycjaZamowienia)
        {
            if (Request.IsAjaxRequest())
            {
                db.Entry(pozycjaZamowienia).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }


            //Check if current user is admin
            bool IsAdmin = User.IsInRole("Admin");
            ViewBag.UserIsAdmin = IsAdmin;

            var WszystkieZwierzeta = db.Zwierzeta.ToList();
            List<string> wojewodztwa = new List<string>();
            WszystkieZwierzeta.ForEach(a =>
            {
                //When launching delete a.Wojewodztwo != null
                if (a.Wojewodztwo != null && !a.Wojewodztwo.Equals(String.Empty))
                    wojewodztwa.Add(a.Wojewodztwo);
            });

            ViewBag.Wojewodztwa = wojewodztwa.Distinct();
            ViewBag.Gatunki = db.Gatunki.ToList().Select(a => a.NazwaGatunku);




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

            var vm = new List<WystawioneZwierzetaViewModel>();
            WystawioneZwierzeta.ForEach(a =>
            {
                vm.Add(new WystawioneZwierzetaViewModel()
                {
                    WystawioneZwierzeta = a,
                    Zdjecia = db.Zdjecie.Where(b => b.ZwierzeId == a.ZwierzeId).ToList()
                });
            });



            return View(vm);
        }

        [HttpDelete]
        public ActionResult UsunZdjecie(int zdjecieId)
        {
            var Zdjecie = db.Zdjecie.Find(zdjecieId);
            

            var ZwierzeId = Zdjecie.ZwierzeId;
            var Zwierze = db.Zwierzeta.Find(ZwierzeId);

            //Sprawdz ile zdjec jest obecnie przypisanych do tego zwierza
            var Zdjecia = db.Zdjecie.Where(a => a.ZwierzeId == ZwierzeId && a.ZdjecieID == zdjecieId).ToList();

            if (Zdjecia == null)
            {
                var zd = new Zdjecie()
                {
                    FilePath = "Domyslne.png",
                    ZwierzeId = ZwierzeId
                };
                db.Entry(zd).State = EntityState.Deleted;
            }

            //Sprawdz czy usuwane zdjecie jest glownym zdjeciem tego Zwierza
            var ZdjecieGlowne = db.Zwierzeta.Where(a => a.NazwaPlikuObrazka == Zdjecie.ZdjecieID.ToString()).SingleOrDefault();
            if(ZdjecieGlowne != null)
            {
                //Sprawdz czy zwierze ma inne Zdjecia
                if(Zdjecia != null)
                {
                    //Ustaw inne zdjecie głównym zdjeciem
                    Zwierze.NazwaPlikuObrazka = Zdjecia.Take(1).FirstOrDefault().FilePath;
                }
                //W innym przypadku ustaw domyślne zdjecie
                else
                {
                    Zwierze.NazwaPlikuObrazka = "Domyslne.png";
                }
            }

            db.Entry(Zdjecie).State = EntityState.Deleted;
            db.SaveChanges();
            //Check if current user is admin
            bool IsAdmin = User.IsInRole("Admin");
            ViewBag.UserIsAdmin = IsAdmin;

            var WszystkieZwierzeta = db.Zwierzeta.ToList();
            List<string> wojewodztwa = new List<string>();
            WszystkieZwierzeta.ForEach(a =>
            {
                //When launching delete a.Wojewodztwo != null
                if (a.Wojewodztwo != null && !a.Wojewodztwo.Equals(String.Empty))
                    wojewodztwa.Add(a.Wojewodztwo);
            });

            ViewBag.Wojewodztwa = wojewodztwa.Distinct();
            ViewBag.Gatunki = db.Gatunki.ToList().Select(a => a.NazwaGatunku);

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

            var vm = new List<WystawioneZwierzetaViewModel>();
            WystawioneZwierzeta.ForEach(a =>
            {
                vm.Add(new WystawioneZwierzetaViewModel()
                {
                    WystawioneZwierzeta = a,
                    Zdjecia = db.Zdjecie.Where(b => b.ZwierzeId == a.ZwierzeId).ToList()
                });
            });

            //Tutak
            var zz = db.Zdjecie.Where(a => a.ZwierzeId == ZwierzeId).ToList();
            var zwierze = db.Zwierzeta.Find(ZwierzeId);
            var cc = new WystawioneZwierzetaViewModel() { WystawioneZwierzeta = zwierze, Zdjecia = zz };
            //Tuaj

            var zw = vm.Where(a => a.WystawioneZwierzeta.ZwierzeId == ZwierzeId).FirstOrDefault();

            return PartialView("_Zdjecia", cc);
            //var result = new { Result = vm, ID = ZwierzeId };
            //return Json(result);
        }

        [HttpDelete]
        public ActionResult UsunZwierze(int zwierzeID)
        {
            //Usun Zwierze
            var Zwierze = db.Zwierzeta.Find(zwierzeID);
            var ZwierzeId = Zwierze.ZwierzeId;
            db.Entry(Zwierze).State = EntityState.Deleted;

            //Usun Zdjecia
            var Zdjecia = db.Zdjecie.Where(a => a.ZwierzeId == zwierzeID).ToList();
            Zdjecia.ForEach(b => db.Zdjecie.Remove(b));

            //Usun Wiadomosci
            var wiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == zwierzeID).ToList();
            wiadomosci.ForEach(b => db.Wiadomosci.Remove(b));

            db.SaveChanges();

            return new EmptyResult();
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
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (user.DaneUzytkownika.Adres == null || user.DaneUzytkownika.Email == null || user.DaneUzytkownika.Imie == null || user.DaneUzytkownika.KodPocztowy == null ||
               user.DaneUzytkownika.Miasto == null || user.DaneUzytkownika.Nazwisko == null || user.DaneUzytkownika.Telefon == null)
                return RedirectToAction("Index", new { info = "Prosze wprowadzić swoje dane" });

            var WszystkieZwierzeta = db.Zwierzeta.ToList();
            List<string> wojewodztwa = new List<string>();
            WszystkieZwierzeta.ForEach(a =>
            {
                //When launching delete a.Wojewodztwo != null
                if (a.Wojewodztwo != null && !a.Wojewodztwo.Equals(String.Empty))
                    wojewodztwa.Add(a.Wojewodztwo);
            });

            ViewBag.Wojewodztwa = wojewodztwa.Distinct();
            ViewBag.PierwszeWojewodztwo = wojewodztwa.Distinct().First();


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

            zwierze.DataNarodzin = DateTime.Now;
            var result = new EditZwierzeViewModel()
            {
                Gatunki = db.Gatunki.ToList(),
                Zwierze = zwierze,
                Potwierdzenie = potwierdzenie
            };

            return View(result);
        }


        [HttpPost]
        public ActionResult DodajZwierze(EditZwierzeViewModel model, IEnumerable<HttpPostedFileBase> file)
        {
            var WszystkieZwierzeta = db.Zwierzeta.ToList();
            List<string> wojewodztwa = new List<string>();
            WszystkieZwierzeta.ForEach(a =>
            {
                //When launching delete a.Wojewodztwo != null
                if (a.Wojewodztwo != null && !a.Wojewodztwo.Equals(String.Empty))
                    wojewodztwa.Add(a.Wojewodztwo);
            });

            ViewBag.Wojewodztwa = wojewodztwa.Distinct();
            ViewBag.PierwszeWojewodztwo = wojewodztwa.Distinct().First();
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

                //Co gdy użytkownik nie wybral pliku
                if (file == null)
                {
                    //Model zostanie zwrocony, ponieważ w drpodown liście nie zostaną wyświetlone elementy! stąd musimy je jeszcze
                    //raz pobrać żeby poprostu zostały pokazane!
                    var gatunki = db.Gatunki.ToList();
                    model.Gatunki = gatunki;
                    return View(model);
                }
                //Sprawdzenie czy uzytkownik wybral plik
                else
                {
                    //Czy Pozostałe pola zostały wypełnione poprawnie
                    if (ModelState.IsValid)
                    {
                        if (file.Count() == 1)
                        {
                            var sourceImage = Image.FromStream(file.ElementAt(0).InputStream);

                            sourceImage = ResizeImage(sourceImage, 500, 500);

                            //Generowanie plik
                            var fileExt = Path.GetExtension(file.ElementAt(0).FileName);
                            var filename = Guid.NewGuid() + fileExt; // Unikalny identyfikator + rozszerzenie

                            //W jakim folderze ma byc umiesczony dany plik oraz jego nazwa! Oraz zapis
                            var path = Path.Combine(Server.MapPath(AppConfig.ObrazkiFolderWzgledny), filename);
                            //file.SaveAs(path);
                            sourceImage.Save(path);

                            model.Zwierze.NazwaPlikuObrazka = filename;
                            model.Zwierze.DataDodania = DateTime.Now;
                            model.Zwierze.UserId = User.Identity.GetUserId();
                            //Oczywiscie mozna wykonac standardowa procedure db.Zwierze.Add(); db.SaveChanges(), ale...
                            db.Entry(model.Zwierze).State = EntityState.Added;
                            var user = UserManager.FindById(User.Identity.GetUserId());
                            Zdjecie zdjecie = new Zdjecie
                            {
                                FilePath = filename,
                                ZwierzeId = model.Zwierze.ZwierzeId,
                                Zwierze = model.Zwierze
                            };
                            db.Zdjecie.AddOrUpdate(zdjecie);
                            db.SaveChanges();
                        }
                        else
                        {
                            int licznik = 0;
                            foreach (var item in file)
                            {
                                if (licznik == 0)
                                {
                                    var sourceImage = Image.FromStream(item.InputStream);

                                    sourceImage = ResizeImage(sourceImage, 500, 500);

                                    //Generowanie plik
                                    var fileExt = Path.GetExtension(item.FileName);
                                    var filename = Guid.NewGuid() + fileExt; // Unikalny identyfikator + rozszerzenie

                                    //W jakim folderze ma byc umiesczony dany plik oraz jego nazwa! Oraz zapis
                                    var path = Path.Combine(Server.MapPath(AppConfig.ObrazkiFolderWzgledny), filename);
                                    //file.SaveAs(path);
                                    sourceImage.Save(path);

                                    model.Zwierze.NazwaPlikuObrazka = filename;

                                    //model.Zwierze.NazwyPlikowObrazkow.Add(filename);
                                    model.Zwierze.DataDodania = DateTime.Now;
                                    model.Zwierze.UserId = User.Identity.GetUserId();
                                    //Oczywiscie mozna wykonac standardowa procedure db.Zwierze.Add(); db.SaveChanges(), ale...
                                    db.Entry(model.Zwierze).State = EntityState.Added;
                                    var user = UserManager.FindById(User.Identity.GetUserId());
                                    Zdjecie zdjecie = new Zdjecie
                                    {
                                        FilePath = filename,
                                        ZwierzeId = model.Zwierze.ZwierzeId,
                                        Zwierze = model.Zwierze
                                    };
                                    db.Zdjecie.AddOrUpdate(zdjecie);
                                    db.SaveChanges();
                                    licznik++;
                                }
                                else
                                {
                                    var sourceImage = Image.FromStream(item.InputStream);

                                    sourceImage = ResizeImage(sourceImage, 500, 500);

                                    //Generowanie plik
                                    var fileExt = Path.GetExtension(item.FileName);
                                    var filename = Guid.NewGuid() + fileExt; // Unikalny identyfikator + rozszerzenie

                                    //W jakim folderze ma byc umiesczony dany plik oraz jego nazwa! Oraz zapis
                                    var path = Path.Combine(Server.MapPath(AppConfig.ObrazkiFolderWzgledny), filename);
                                    //file.SaveAs(path);
                                    sourceImage.Save(path);

                                    //model.Zwierze.NazwyPlikowObrazkow.Add(filename);
                                    //Oczywiscie mozna wykonac standardowa procedure db.Zwierze.Add(); db.SaveChanges(), ale...
                                    Zdjecie zdjecie = new Zdjecie
                                    {
                                        FilePath = filename,
                                        ZwierzeId = model.Zwierze.ZwierzeId,
                                        Zwierze = model.Zwierze
                                    };
                                    db.Zdjecie.AddOrUpdate(zdjecie);
                                    var user = UserManager.FindById(User.Identity.GetUserId());

                                    db.SaveChanges();
                                }
                            }
                            licznik = 0;
                        }

                        return RedirectToAction("DodajZwierze", new { potwierdzenie = true });
                    }
                    else
                    {
                        var gatunki = db.Gatunki.ToList();
                        model.Gatunki = gatunki;
                        return View(model);
                    }

                }

            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        [NonAction]
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public ActionResult WyswietlWiadomosciUzytkownika()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.ID = user.Id;

            //Jak ja wysyłam to mnie nie obchodzi wysyłający tylko odbiorca
            //Jak ja odbieram to mnie nie obchodzi odbierający tylko wysyłający

            //List of all users id !
            //var GetUsersIDSended = user.SenderMessages.Where(a => a.ReceiverId != user.Id).Select(b => b.ReceiverId).ToList();
            //var GetUsersIDRetrived = user.ReceiverMessages.Where(a => a.SenderId != user.Id).Select(b => b.SenderId).ToList();

            //var wiadomosciWyslane = user.SenderMessages.Where(a => a.SenderId == user.Id).OrderByDescending(a => a.DateAndTimeOfSend).DistinctBy(a => a.ZwierzeId).ToList();

            ////GIT
            //var wiadomosciOtrzymane = user.ReceiverMessages.Where(a => a.ReceiverId == user.Id).OrderByDescending(a => a.DateAndTimeOfSend).DistinctBy(a => a.ZwierzeId).ToList();


            //Potrzebuje nazwyUzytkownika + Daty + Tresci kazdej wiadomosci
            var wiadomosci = new List<WiadomoscZIdViewModel>();
            var w = new WiadomoscZIdViewModel();
            var g = new List<Wiadomosc>();

            var wiadomosciWyslane = user.SenderMessages.OrderByDescending(a => a.DateAndTimeOfSend).ToList();

            //GIT
            var wiadomosciOtrzymane = user.ReceiverMessages.OrderByDescending(a => a.DateAndTimeOfSend).ToList();

            //Działa,ale tylko dla wiadomosci jedna po drugiej!
            //Spróbuj utworzyc pustą liste i dla kazdej wiadomosci w bazie (odseparowanej) sprawdz czy:
            //1)W liscie znajduje sie taki obiekt z kluczem zwierza jezeli nie dodaj
            //2)jezeli nastepny obiekt ma taki sam klucz zwierza ale inny klucz obcy idUser to dodaj
            //Oczywiscie przypisz te wiadomosci z bazy do zmiennej a nastpenie posortuje OrderByDescending!
            var w1 = new List<Wiadomosc>();
            wiadomosciWyslane.ForEach((a, b) =>
            {
                if (!w1.Exists(q => q.ZwierzeId == a.ZwierzeId))
                    w1.Add(a);
                else
                {
                    ///such value with given key exits 
                    ///check his id
                    if (!w1.Exists(n => (n.ZwierzeId == a.ZwierzeId) && (n.ReceiverId == a.ReceiverId)))
                        w1.Add(a);
                }
            });

            var w2 = new List<Wiadomosc>();
            wiadomosciOtrzymane.ForEach((a, b) =>
            {
                if (!w2.Exists(q => q.ZwierzeId == a.ZwierzeId))
                    w2.Add(a);
                else
                {
                    ///such value with given key exits 
                    ///check his id
                    if (!w2.Exists(n => (n.ZwierzeId == a.ZwierzeId) && (n.SenderId == a.SenderId)))
                        w2.Add(a);
                }
            });

            //Dopre wartosci niemniej trzeba usunac duplikaty!


            w1.AddRange(w2);
            w1 = w1.OrderByDescending(a => a.DateAndTimeOfSend).Distinct().ToList();

            for (int i = 0; i < w1.Count; i++)
            {
                for (int y = 0; y < w1.Count; y++)
                {
                    var ww = w1[i];
                    var cc = w1[y];

                    if ((w1[i].ZwierzeId == w1[y].ZwierzeId) && (w1[i].ReceiverId == w1[y].SenderId &&
                       w1[i].SenderId == w1[y].ReceiverId))
                    {
                        if (w1[i].DateAndTimeOfSend > w1[y].DateAndTimeOfSend)
                            w1.Remove(w1[y]);
                        else if (w1[i].DateAndTimeOfSend < w1[y].DateAndTimeOfSend)
                        { w1.Remove(w1[i]); continue; }
                    }
                }
            }
            //for(int i = 0;i<wiadomosciOtrzymane.Count;i++)
            //{
            //    if(i == 0)
            //    {
            //        if (wiadomosciOtrzymane[i + 1].ZwierzeId != wiadomosciOtrzymane[i].ZwierzeId
            //            || wiadomosciOtrzymane[i + 1].SenderId != wiadomosciOtrzymane[i].SenderId)
            //        {
            //            g.Add(wiadomosciOtrzymane[i]);
            //        }
            //    }0

            //    if (i < wiadomosciOtrzymane.Count-1)
            //    {
            //        if (wiadomosciOtrzymane[i + 1].ZwierzeId != wiadomosciOtrzymane[i].ZwierzeId
            //            || wiadomosciOtrzymane[i + 1].SenderId != wiadomosciOtrzymane[i].SenderId)
            //        {
            //            g.Add(wiadomosciOtrzymane[i+1]);
            //        }
            //    }
            //}
            //for (int i = 0; i < wiadomosciWyslane.Count; i++)
            //{
            //    if(i == 0)
            //    {
            //        if (wiadomosciWyslane[i + 1].ZwierzeId != wiadomosciWyslane[i].ZwierzeId
            //            || wiadomosciWyslane[i + 1].SenderId != wiadomosciWyslane[i].SenderId)
            //        {
            //            g.Add(wiadomosciWyslane[i]);
            //        }
            //    }

            //    if (i < wiadomosciWyslane.Count-1)
            //    {
            //        if (wiadomosciWyslane[i + 1].ZwierzeId != wiadomosciWyslane[i].ZwierzeId
            //            || wiadomosciWyslane[i + 1].SenderId != wiadomosciWyslane[i].SenderId)
            //        {
            //            g.Add(wiadomosciWyslane[i+1]);
            //        }
            //    }
            //}

            var c = g.ToList();

            //Teraz wiem ze wiadomosciWyslane powinny miec unikalny na podstawie klucza otrzymane a otrzymane to klucz wys
            var wiadomosciUzytkownika = db.Wiadomosci.Where(a => a.ReceiverId == user.Id || a.SenderId == user.Id)
                .OrderByDescending(a => a.DateAndTimeOfSend).DistinctBy(a => a.ZwierzeId).ToList();


            try
            {
                w1.ForEach(a =>
            {
                wiadomosci.Add(new WiadomoscZIdViewModel()
                {
                    Wiadomosc = a,
                    UserID = a.ReceiverId == user.Id ? a.SenderId : a.ReceiverId,
                    NazwaPliku = db.Zwierzeta.Find(a.ZwierzeId).NazwaPlikuObrazka,
                    NazwaZwierza = db.Zwierzeta.Find(a.ZwierzeId).Nazwa,
                    ImieUzytkownika = db.Users.Find(a.ReceiverId == user.Id ? a.SenderId : a.ReceiverId).DaneUzytkownika.Imie,
                    NazwiskoUzytkownika = db.Users.Find(a.ReceiverId == user.Id ? a.SenderId : a.ReceiverId).DaneUzytkownika.Nazwisko,
                    Email = db.Users.Find(a.ReceiverId == user.Id ? a.SenderId : a.ReceiverId).Email
                });
            });
            }catch(NullReferenceException ex)
            {
                Debug.Write(ex.Data.Keys);
                Debug.Write(ex.InnerException.TargetSite.Name);
            }

            //wiadomosciWyslane.ForEach(a =>
            //{
            //    //wiadomosci.Add(new WiadomosciViewModel()
            //    //{
            //    //    NazwaUzytkownika = a.Receiver.DaneUzytkownika.Imie + " " +  a.Receiver.DaneUzytkownika.Nazwisko,
            //    //    DataWyslania = a.DateAndTimeOfSend,
            //    //    TrescWiadomosci = a.Body
            //    //});

            //    var z = new WiadomosciViewModel()
            //    {
            //        NazwaUzytkownika = a.Receiver.DaneUzytkownika.Imie + " " + a.Receiver.DaneUzytkownika.Nazwisko,
            //        DataWyslania = a.DateAndTimeOfSend,
            //        TrescWiadomosci = a.Body,
            //        Id = a.ReceiverId,
            //        Zwierze = db.Zwierzeta.Where(b => b.ZwierzeId == a.ZwierzeId).FirstOrDefault()
            //    };

            //    w.WiadomosciWyslane.Add(z);
            //});

            //wiadomosciOtrzymane.ForEach(a =>
            //{
            //    //wiadomosci.Add(new WiadomosciViewModel()
            //    //{
            //    //    NazwaUzytkownika = a.Receiver.DaneUzytkownika.Imie + " " + a.Receiver.DaneUzytkownika.Nazwisko,
            //    //    DataWyslania = a.DateAndTimeOfSend,
            //    //    TrescWiadomosci = a.Body
            //    //});

            //    var z = new WiadomosciViewModel()
            //    {
            //        NazwaUzytkownika = a.Receiver.DaneUzytkownika.Imie + " " + a.Receiver.DaneUzytkownika.Nazwisko,
            //        DataWyslania = a.DateAndTimeOfSend,
            //        TrescWiadomosci = a.Body,
            //        Id = a.SenderId,
            //        Zwierze = db.Zwierzeta.Where(b => b.ZwierzeId == a.ZwierzeId).FirstOrDefault()
            //    };

            //    w.WiadomosciOtrzymane.Add(z);
            //});


            return View(wiadomosci);

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

        public ActionResult WiadomosciKonwersacja(int idZwierza, string idUser, bool Otrzymane = false, int w = -1)
        {
            //  Jezeli jest to id uzytkownika wiemy ze powinnismy zwrococ wiadomosci tego uzytkownika a jesli chodzi o 
            //innych uzytkownikow to tam przekazujemy sernderID = isUser
            var userLogged = UserManager.FindById(User.Identity.GetUserId());

            if (w != -1)
            {
                var wiadomosc = db.Wiadomosci.Find(w);
                if (wiadomosc.ReceiverId == userLogged.Id)
                {
                    db.Wiadomosci.Find(w).Read = true;
                    db.SaveChanges();
                }
            }
            ViewBag.ID = userLogged.Id;
            var userDiffrent = UserManager.FindById(idUser);
            var wszystkieWiadomosci = new List<Wiadomosc>();

            wszystkieWiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == idZwierza && ((a.ReceiverId == idUser && a.SenderId == userLogged.Id)
                                                     || (a.SenderId == idUser && a.ReceiverId == userLogged.Id))).ToList();

            ///Tutaj zmiana
            //if (Otrzymane)
            //{
            //    //var mojeWiadomosci = userLogged.SenderMessages.Where(a => a.ZwierzeId == idZwierza && a.ReceiverId == idUser).ToList();
            //    //var inneWiadomosci = userDiffrent.ReceiverMessages.Where(a => a.ZwierzeId == idZwierza && a.SenderId == userLogged.Id).ToList();
            //    //mojeWiadomosci.ForEach(a => wszystkieWiadomosci.Add(a)); inneWiadomosci.ForEach(a => wszystkieWiadomosci.Add(a));

            //    wszystkieWiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == idZwierza && (a.ReceiverId == userLogged.Id
            //                                         && a.SenderId == userDiffrent.Id)).ToList();
            //}
            //else
            //{
            //    //var mojeWiadomosci = userLogged.SenderMessages.Where(a => a.ZwierzeId == idZwierza && a.ReceiverId == idUser).ToList();
            //    //var inneWiadomosci = userDiffrent.SenderMessages.Where(a => a.ZwierzeId == idZwierza && a.SenderId == userLogged.Id).ToList();
            //    //mojeWiadomosci.ForEach(a => wszystkieWiadomosci.Add(a)); inneWiadomosci.ForEach(a => wszystkieWiadomosci.Add(a));
            //    wszystkieWiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == idZwierza && (a.ReceiverId == userDiffrent.Id
            //                                         && a.SenderId == userLogged.Id)).ToList();
            //}



            //var listaMoichWiadomosci = new List<Wiadomosc>();
            //var listaInnychWiadomosci = new List<Wiadomosc>();
            //var userId = User.Identity.GetUserId();

            //var listaWiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == idZwierza && a.ReceiverId == idUser && a.SenderId == userId)
            //                            .ToList();

            //idUser == gosc do ktorego sie wysyla ... userId == mojeId

            //if (czyIdUzytkownika)
            //{
            //   listaMoichWiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == idZwierza && a.SenderId == idUser).ToList();
            //   listaInnychWiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == idZwierza && a.Re
            //}
            //else
            //{
            //    var listaMoichWiadomos
            //}

            //Chcę wziąć wszystkie wiadmości odnośnie konkretnego zwierza i od określonej osoby

            //a.ReceiverId.Equals(idReceiverId, StringComparison.CurrentCultureIgnoreCase)
            //|| (a.SenderId.Equals(idSenderID, StringComparison.CurrentCultureIgnoreCase)))).ToList();

            //Pobierz wiadomosci które ja wysłałem do tej osoby
            //var listaWiadomosciOsobyWysylającej = db.Wiadomosci.Where(a => a.ZwierzeId == idZwierza && )

            //Pobierz wiadomosci które ona wysłałą do mnie!
            var wWiadomosc = new Wiadomosc
            {
                ZwierzeId = idZwierza,
                SenderId = userLogged.Id,
                ReceiverId = userDiffrent.Id
            };

            var vm = new WiadomoscViewModel
            {
                ListaWiadomosci = wszystkieWiadomosci,
                wiadomosc = wWiadomosc
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult UploadFiles(int id)
        {
            var zwierze = db.Zwierzeta.Find(id);

            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {

                        HttpPostedFileBase file = files[i];
                        var sourceImage = Image.FromStream(file.InputStream);

                        sourceImage = ResizeImage(sourceImage, 500, 500);

                        //Generowanie plik
                        var fileExt = Path.GetExtension(file.FileName);
                        var filename = Guid.NewGuid() + fileExt; // Unikalny identyfikator + rozszerzenie

                        //W jakim folderze ma byc umiesczony dany plik oraz jego nazwa! Oraz zapis
                        var path = Path.Combine(Server.MapPath(AppConfig.ObrazkiFolderWzgledny), filename);
                        //file.SaveAs(path);
                        sourceImage.Save(path);

                        Zdjecie zdjecie = new Zdjecie
                        {
                            FilePath = filename,
                            ZwierzeId = id,
                            Zwierze = zwierze
                        };
                        db.Zdjecie.AddOrUpdate(zdjecie);
                        db.SaveChanges();
                    }

                    var zz = db.Zdjecie.Where(a => a.ZwierzeId == id).ToList();

                    var vm = new WystawioneZwierzetaViewModel() { WystawioneZwierzeta = zwierze, Zdjecia = zz };


                    return PartialView("_Zdjecia", vm);

                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public ActionResult WyslijWiadomosc(Wiadomosc wiadomosc, bool FromSzczegoly = false)
        {
            //db.Users.Find(wiadomosc.ReceiverId).ReceiverMessages.Add(wiadomosc);
            //db.Users.Find(wiadomosc.SenderId).SenderMessages.Add(wiadomosc);
            wiadomosc.DateAndTimeOfSend = DateTime.Now;

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
            if (FromSzczegoly)
            {
                return null;
            }
            var userLogged = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.ID = userLogged.Id;
            var userDiffrent = UserManager.FindById(wiadomosc.ReceiverId);
            var wszystkieWiadomosci = new List<Wiadomosc>();

            wszystkieWiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == wiadomosc.ZwierzeId
                                                     && ((a.ReceiverId == userDiffrent.Id && a.SenderId == userLogged.Id)
                                                     || (a.SenderId == userDiffrent.Id && a.ReceiverId == userLogged.Id))).ToList();

            foreach (var item in wszystkieWiadomosci)
            {
                if (item.Sender == null)
                {
                    item.Sender = UserManager.FindById(item.SenderId);
                }

                if (item.Receiver == null)
                {
                    item.Receiver = UserManager.FindById(item.ReceiverId);
                }
            }

            var idZwierza = wiadomosc.ZwierzeId;
            string idReceiverId = wiadomosc.ReceiverId;

            //var mojeWiadomosci = userLogged.SenderMessages.Where(a => a.ZwierzeId == idZwierza && a.ReceiverId == idUser).ToList();
            //var inneWiadomosci = userDiffrent.SenderMessages.Where(a => a.ZwierzeId == idZwierza && a.SenderId == userLogged.Id).ToList();
            //var wszystkieWiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == idZwierza && (a.ReceiverId == idReceiverId &&
            //                                         a.SenderId == idSenderID)).ToList();


            //var inneWiadomosci = db.Wiadomosci.Where(a => a.ZwierzeId == idZwierza && a.SenderId == idSenderID).ToList();
            return View("_Wiadomosci", wszystkieWiadomosci);
        }

    }
}