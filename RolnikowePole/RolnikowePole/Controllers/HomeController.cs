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
            var lk = db.Gatunki.ToList();
            return View();
        }
    }
}