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

            var gatunki = db.Gatunki.Include("Kursy").Where(k => k.NazwaGatunku.ToUpper() == nazwaGatunku.ToUpper()).Single();
            var zwierzeta = gatunki.Zwierzeta.ToList();
            return View(zwierzeta);
        }

        public ActionResult Szczegoly(string id)
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult GatunkiMenu()
        {
            var gatunki = db.Gatunki.ToList();
            return PartialView("_GatunkiMenu", gatunki);
        }
    }
}