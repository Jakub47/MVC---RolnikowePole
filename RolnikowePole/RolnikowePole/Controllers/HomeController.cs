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
            //logger.Info("Jestes na stronie glownej");

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
            zwierzeta = db.Zwierzeta.Where(z => !z.Ukryty).OrderBy(a => Guid.NewGuid()).ToList();



            //  NIe pobierać zwięrząt cache! bo mogą zostać w każdym momencie zmodyfikowane!
            //if (cache.IsSet(Consts.ZwierzetaGatunkuCacheKey))
            //{
            //    zwierzeta = cache.Get(Consts.ZwierzetaGatunkuCacheKey) as List<Zwierze>;
            //}

            //else
            //{                
            //    zwierzeta = db.Zwierzeta.Where(z => !z.Ukryty).OrderBy(a => Guid.NewGuid()).ToList();
            //    cache.Set(Consts.ZwierzetaGatunkuCacheKey, zwierzeta, 60);
            //}
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
        

        [ChildActionOnly]
        public PartialViewResult GetLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user.SenderMessages != null || user.ReceiverMessages != null)
                {
                    var wiadomosciWyslane = user.SenderMessages.OrderByDescending(a => a.DateAndTimeOfSend).ToList();
                    var wiadomosciOtrzymane = user.ReceiverMessages.OrderByDescending(a => a.DateAndTimeOfSend).ToList();
                    var w1 = new List<Wiadomosc>();
                    wiadomosciWyslane.ForEach((a, b) =>
                    {
                        if (!w1.Exists(q => q.ZwierzeId == a.ZwierzeId))
                            w1.Add(a);
                        else
                        {
                        ///such value with given key exits 
                        ///check his id
                        if (!w1.Exists(n => (n.ZwierzeId == a.ZwierzeId) && (n.ReceiverId == a.ReceiverId)))
                                w1.Add(a);
                        }
                    });

                    var w2 = new List<Wiadomosc>();
                    wiadomosciOtrzymane.ForEach((a, b) =>
                    {
                        if (!w2.Exists(q => q.ZwierzeId == a.ZwierzeId))
                            w2.Add(a);
                        else
                        {
                        if (!w2.Exists(n => (n.ZwierzeId == a.ZwierzeId) && (n.SenderId == a.SenderId)))
                                w2.Add(a);
                        }
                    });

                    w1.AddRange(w2);
                    w1 = w1.OrderByDescending(a => a.DateAndTimeOfSend).Distinct().ToList();

                    for (int i = 0; i < w1.Count; i++)
                    {
                        for (int y = 0; y < w1.Count; y++)
                        {
                            var ww = w1[i];
                            var cc = w1[y];

                            if ((w1[i].ZwierzeId == w1[y].ZwierzeId) && (w1[i].ReceiverId == w1[y].SenderId &&
                               w1[i].SenderId == w1[y].ReceiverId))
                            {
                                if (w1[i].DateAndTimeOfSend > w1[y].DateAndTimeOfSend)
                                    w1.Remove(w1[y]);
                                else if (w1[i].DateAndTimeOfSend < w1[y].DateAndTimeOfSend)
                                { w1.Remove(w1[i]); continue; }
                            }
                        }
                    }

                    foreach (var item in w1)
                    {
                        if (item.Read == false && (User.Identity.GetUserId() == item.ReceiverId))
                        {
                            ViewBag.Klasa = "fas fa-envelope";
                        }
                    }
                }
            }
            return PartialView("~/Views/Shared/_LoginPartial.cshtml");
        }
    }
}