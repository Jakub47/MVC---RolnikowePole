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

            if (!db.Gatunki.Any(a => a.NazwaGatunku.ToLower() == nazwaGatunku.ToLower()))
                return RedirectToAction("Index", "Home");

            var gatunki = db.Gatunki.Include("Zwierzeta").Where(k => k.NazwaGatunku.ToUpper() == nazwaGatunku.ToUpper()).Single();

            var zwierzeta = gatunki.Zwierzeta.Where(a => (searchQuery == null ||
                                                    a.Nazwa.ToLower().Contains(searchQuery.ToLower())) && !a.Ukryty);

            var WszystkieZwierzeta = db.Zwierzeta.ToList();
            List<string> wojewodztwa = new List<string>();
            WszystkieZwierzeta.ForEach(a =>
                {
                    if ( !a.Wojewodztwo.Equals(String.Empty))
                        wojewodztwa.Add(a.Wojewodztwo);
                });

            ViewBag.Wojewodztwa = wojewodztwa.Distinct();
            ViewBag.NazwaGatunku = char.ToUpper(nazwaGatunku[0]) + nazwaGatunku.Substring(1);

            if (Request.IsAjaxRequest())
            {
                if (nazwa == "DataW")
                {
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
            nowosci = db.Zwierzeta.Where(a => !a.Ukryty).OrderByDescending(a => a.DataDodania).Take(6).ToList();

            //Check if given values already exits in cache {Remeber to use Consts because we don't want to remeber the key don't we? Let just use variable for that!}
            //if (cache.IsSet(Consts.NowosciCacheKey))
            //{
            //    //If it exitsts get that value from cache
            //    nowosci = cache.Get(Consts.NowosciCacheKey) as List<Zwierze>;
            //}
            //else
            //{
            //    //If it does not exists get from database
            //    cache.Set(Consts.NowosciCacheKey, nowosci, 60);
            //}

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
            if(zwierze == null)
            {
                return RedirectToAction("Index", "Home");
            }

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
                //Wyroznione = wyroznione,
                Zwierze = zwierze,
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

        //public ActionResult ZwierzePodpowiedzi(string term)
        //{
        //    var zwierzeta = this.db.Zwierzeta.Where(a => !a.Ukryty && a.Nazwa.ToLower().Contains(term.ToLower()))
        //                    .Take(5).Select(a => new { label = a.Nazwa });

        //    return Json(zwierzeta, JsonRequestBehavior.AllowGet);
        //}
    }
}