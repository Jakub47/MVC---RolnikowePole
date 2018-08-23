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
            RolnikowePole.Infrastucture.GatunkiDynamicNodeProvider
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

        [ChildActionOnly]
        public ActionResult GatunkiMenu()
        {
            var gatunki = db.Gatunki.ToList();
            return PartialView("_GatunkiMenu", gatunki);
        }
    }
}