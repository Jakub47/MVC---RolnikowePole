using NLog;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        public ActionResult Index()
        {
            logger.Info("Jestes na stronie glownej");

            // Test Cache with create class
            ICacheProvider cache = new DefaultCacheProvider();

            List<Gatunek> gatunki;

            if (cache.IsSet(Consts.GatunkiCacheKey))
            {
                gatunki = cache.Get(Consts.GatunkiCacheKey) as List<Gatunek>;
            }

            else
            {
                gatunki = db.Gatunki.ToList();
                cache.Set(Consts.GatunkiCacheKey, gatunki, 60);
            }

            List<Zwierze> zwierzeta;

            if (cache.IsSet(Consts.ZwierzetaGatunkuCacheKey))
            {
                zwierzeta = cache.Get(Consts.ZwierzetaGatunkuCacheKey) as List<Zwierze>;
            }

            else
            {                
                zwierzeta = db.Zwierzeta.Where(z => !z.Ukryty).OrderBy(a => Guid.NewGuid()).ToList();
                cache.Set(Consts.ZwierzetaGatunkuCacheKey, zwierzeta, 60);
            }
            
            //var vm = new ZwierzetaGatunkiViewModel()
            //{
            //    Gatunki = gatunki,
            //    ZwierzetaGatunku = zwierzeta
            //};

            return View(zwierzeta);
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