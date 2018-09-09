using Hangfire;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //GlobalConfiguration.Configuration
            //    .UseSqlServerStorage("RolnikowePoleContext");
            //app.UseHangfireDashboard();
            //app.UseHangfireServer();

        }
    }
}