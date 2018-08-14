using RolnikowePole.DAL;
using RolnikowePole.Models;
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
        // GET: Home
        public ActionResult Index()
        {
            var gatunek = new Gatunek() { NazwaGatunku = "Ryby", OpisGatunku = "Test" };
            db.Gatunki.Add(gatunek);
            db.SaveChanges();
            return View();
        }
    }
}