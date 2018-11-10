using Microsoft.AspNet.Identity;
using RolnikowePole.DAL;
using RolnikowePole.Infrastucture;
using RolnikowePole.Models;
using RolnikowePole.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace RolnikowePole.Controllers
{
    public class ZwierzetaController : Controller
    {
        private RolnikowePoleContext db = new RolnikowePoleContext();

        // GET: Zwierzeta
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lista(string nazwaGatunku, string searchQuery = null, string nazwa = null)
        {
            //Sometimes a action is called with given name in case that will happen an error will ocur to avoid such thing
            //Simply return empty result that will change litery nothing
            if (Request.IsAjaxRequest() && nazwaGatunku == "bootstrap.bundle.min.js")
            {
                return new EmptyResult();
            }

            var gatunki = db.Gatunki.Include("Zwierzeta").Where(k => k.NazwaGatunku.ToUpper() == nazwaGatunku.ToUpper()).Single();

            var zwierzeta = gatunki.Zwierzeta.Where(a => (searchQuery == null ||
                                                    a.Nazwa.ToLower().Contains(searchQuery.ToLower())) && !a.Ukryty);

            var WszystkieZwierzeta = db.Zwierzeta.ToList();
            List<string> wojewodztwa = new List<string>();
            WszystkieZwierzeta.ForEach(a =>
                {
                    //When launching delete a.Wojewodztwo != null
                    if ( a.Wojewodztwo != null && !a.Wojewodztwo.Equals(String.Empty))
                        wojewodztwa.Add(a.Wojewodztwo);
                });

            ViewBag.Wojewodztwa = wojewodztwa.Distinct();
            ViewBag.NazwaGatunku = char.ToUpper(nazwaGatunku[0]) + nazwaGatunku.Substring(1);

            if (Request.IsAjaxRequest())
            {
                if (nazwa == "DataW")
                {
                    //var NoweZwierzeta = gatunki.Zwierzeta.OrderByDescending(a => a.DataDodania).ToList();
                    zwierzeta = zwierzeta.OrderByDescending(a => a.DataDodania).ToList();
                    return View("_ZwierzetaList", zwierzeta);
                }

                else if (nazwa == "DataN")
                {
                    zwierzeta = zwierzeta.OrderByDescending(a => a.DataNarodzin).ToList();
                    return View("_ZwierzetaList", zwierzeta);
                }

                else if (nazwa == "CenaNajt")
                {
                    zwierzeta = zwierzeta.OrderBy(a => a.CenaZwierza).ToList();
                    return View("_ZwierzetaList", zwierzeta);
                }

                else if (nazwa == "CenaNajd")
                {
                    zwierzeta = zwierzeta.OrderByDescending(a => a.CenaZwierza).ToList();
                    return View("_ZwierzetaList", zwierzeta);
                }

                else if (nazwa == "Wszystkie Województwa")
                {
                    zwierzeta = gatunki.Zwierzeta.Where(a => (searchQuery == null ||
                                                    a.Nazwa.ToLower().Contains(searchQuery.ToLower())) && !a.Ukryty);
                    return View("_ZwierzetaList", zwierzeta);
                }

                else if (nazwa != null)
                {
                    //zwierzeta = zwierzeta.Where(a => a.Wojewodztwo.ToLower() == nazwa.ToLower());
                    //List<Zwierze> NewOnes = new List<Zwierze>();

                    //foreach (var item in zwierzeta)
                    //{
                    //    string c = item.Wojewodztwo.ToLower();
                    //    string g = nazwa.ToLower();

                    //    if (c == g)
                    //        NewOnes.Add(item);
                    //}

                    ////var NoweZwierzeta = gatunki.Zwierzeta.Where(a => !a.Ukryty && a.Wojewodztwo.ToLower() == nazwa.ToLower());

                    //if (NewOnes.Count <= 0)
                    //    return new EmptyResult();

                    zwierzeta = zwierzeta.Where(a => a.Wojewodztwo.ToLower() == nazwa.ToLower());


                    return View("_ZwierzetaList", zwierzeta);
                }
            }

            return View(zwierzeta);
        }

        public ActionResult Szczegoly(string id)
        {
            ICacheProvider cache = new DefaultCacheProvider();

            List<Zwierze> nowosci;

            //Check if given values already exits in cache {Remeber to use Consts because we don't want to remeber the key don't we? Let just use variable for that!}
            if (cache.IsSet(Consts.NowosciCacheKey))
            {
                //If it exitsts get that value from cache
                nowosci = cache.Get(Consts.NowosciCacheKey) as List<Zwierze>;
            }
            else
            {
                //If it does not exists get from database
                nowosci = db.Zwierzeta.Where(a => !a.Ukryty).OrderByDescending(a => a.DataDodania).Take(3).ToList();
                cache.Set(Consts.NowosciCacheKey, nowosci, 60);
            }

            List<Zwierze> wyroznione;

            if (cache.IsSet(Consts.WyroznioneCacheKey))
            {
                wyroznione = cache.Get(Consts.WyroznioneCacheKey) as List<Zwierze>;
            }
            else
            {
                wyroznione = db.Zwierzeta.Where(a => !a.Ukryty && a.Wyrozniony).OrderBy(a => Guid.NewGuid()).Take(3).ToList();
                cache.Set(Consts.NowosciCacheKey, wyroznione, 60);
            }

            var zwierze = db.Zwierzeta.Find(int.Parse(id));
            var user = zwierze.User;
            var W = new Wiadomosc();

            if (User.Identity.IsAuthenticated)
            {
                W.ReceiverId = user.Id;
                W.SenderId = User.Identity.GetUserId();
            }
            
            var vm = new HomeViewModel
            {
                Nowe = nowosci,
                Wyroznione = wyroznione,
                Zwierze = zwierze,
                //Since i am working on testing examples their' properties may not be set due to initialization. 
                //Later on this line must be modify to : daneUzytkownika = user.DaneUzytkownika
                daneUzytkownika = user.DaneUzytkownika,
                wiadomosc = W,
                Zdjecia = db.Zdjecie.Where(a=> a.ZwierzeId == zwierze.ZwierzeId).ToList()
            };

            ViewBag.Id = user.Id;
            return View(vm);
        }

        [OutputCache(Duration = 60000)]
        [ChildActionOnly]
        public ActionResult GatunkiMenu()
        {
            /*     W przypadku standardowego skorzystania z cache (asp.net caching) skorzystać z :
                       HttpContext.Cache.Add(); HttpContext.Cache.Get();
                   Niemniej w przypadku zmiany chache (możliwego w przyszłości np memcache reditcache) będziemy musieli zmienić wszyskie odnośniki stąd utworzenie dyna.
            */
            var gatunki = db.Gatunki.ToList();
            return PartialView("_GatunkiMenu", gatunki);
        }

        public ActionResult ZwierzePodpowiedzi(string term)
        {
            var zwierzeta = this.db.Zwierzeta.Where(a => !a.Ukryty && a.Nazwa.ToLower().Contains(term.ToLower()))
                            .Take(5).Select(a => new { label = a.Nazwa });

            return Json(zwierzeta, JsonRequestBehavior.AllowGet);
        }
    }
}