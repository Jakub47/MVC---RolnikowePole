using RolnikowePole.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public ActionResult Lista(string nazwaGatunku)
        {

            var gatunki = db.Gatunki.Include("Zwierzeta").Where(k => k.NazwaGatunku.ToUpper() == nazwaGatunku.ToUpper()).Single();
            var zwierzeta = gatunki.Zwierzeta.ToList();
            return View(zwierzeta);
        }

        public ActionResult Szczegoly(string id)
        {
            var kurs = db.Zwierzeta.Find(int.Parse(id));
            return View(kurs);
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
    }
}