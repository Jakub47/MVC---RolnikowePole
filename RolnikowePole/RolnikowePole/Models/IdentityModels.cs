using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace RolnikowePole.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //public virtual ICollection<Zamowienie> Zamowienia { get; set; }

        public virtual ICollection<Zwierze> ZwierzetaUzytkownika { get; set;}

        [InverseProperty("Sender")]
        public virtual ICollection<Wiadomosc> SenderMessages { get; set; }
        [InverseProperty("Receiver")]
        public virtual ICollection<Wiadomosc> ReceiverMessages { get; set; }

        public DaneUzytkownika DaneUzytkownika { get; set; } 


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}