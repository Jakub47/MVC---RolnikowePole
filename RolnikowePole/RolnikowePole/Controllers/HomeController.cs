using RolnikowePole.DAL;
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
            var gatunki = db.Gatunki.ToList();
            var nowosci = db.Zwierzeta.Where(a => !a.Ukryty).OrderByDescending(a => a.DataDodania).Take(3).ToList();
            var wyroznione = db.Zwierzeta.Where(a => !a.Ukryty && a.Wyrozniony).OrderBy(a => Guid.NewGuid()).Take(3).ToList();

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
    }
}