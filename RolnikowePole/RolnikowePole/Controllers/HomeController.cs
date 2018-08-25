using RolnikowePole.DAL;
using RolnikowePole.Infrastucture;
using RolnikowePole.Models;
using RolnikowePole.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RolnikowePole.Controllers
{
    public class HomeController : Controller
    {
        RolnikowePoleContext db = new RolnikowePoleContext();
        
        
        public ActionResult Index()
        {
            

            //              Test Cache with create class
            ICacheProvider cache = new DefaultCacheProvider();

            List<Gatunek> gatunki;

            if (cache.IsSet(Consts.GatunkiCacheKey))
            {
                gatunki = cache.Get(Consts.GatunkiCacheKey) as List<Gatunek>;
            }
            else
            {
                gatunki = db.Gatunki.ToList();
                cache.Set(Consts.NowosciCacheKey, gatunki, 60);
            }

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

            var vm = new HomeViewModel()
            {
                Gatunki = gatunki,
                NoweZwierzeta = nowosci,
                Wyroznione = wyroznione
            };

            return View(vm);
        }

        public ActionResult StronyStatyczne(string nazwa)
        {
            return View(nazwa);
        }

        public ActionResult Index2()
        {
            return View();
        }
    }
}