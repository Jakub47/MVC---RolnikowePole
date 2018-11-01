using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MoreLinq;
using NLog;
using Postal;
using RolnikowePole.App_Start;
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
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


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

        public ActionResult Contact(FormCollection formCollection = null)
        {
            if(Request.IsAjaxRequest())
            {
                var email = new EmailDoAdmina
                {
                    To = "jakub7249@gmail.com",
                    From = formCollection["Email"],
                    Content = formCollection["Tresc"],
                    Subject = formCollection["Temat"]
                };
                email.Send();
            }

            return View();
        }

        public ActionResult StronyStatyczne(string nazwa)
        {
            return View(nazwa);
        }

        public ActionResult Index2()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult GetLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                var wiadomosciUzytkownika = db.Wiadomosci.Where(a => a.ReceiverId == user.Id || a.SenderId == user.Id).OrderByDescending(a => a.DateAndTimeOfSend).DistinctBy(a =>
                                                                                                                                   a.ZwierzeId).ToList();
                if (wiadomosciUzytkownika.TrueForAll(a => a.Read == false))
                {
                    ViewBag.Klasa = "glyphicon glyphicon-star";
                }
            }
            return PartialView("~/Views/Shared/_LoginPartial.cshtml");
        }
    }
}