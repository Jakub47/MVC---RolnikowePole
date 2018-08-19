using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RolnikowePole.Infrastucture
{
    public static class UrlHelpers
    {
        public static string IkonyGatunkowSciezka(this UrlHelper helper, string nazwaIkonyGatunku)
        {
            var IkonyGatunkiFolder = AppConfig.IkonyGatunkowFolderWzgledny;
            var sciezka = Path.Combine(IkonyGatunkiFolder, nazwaIkonyGatunku);
            var sciezkaBezWzgledna = helper.Content(sciezka);

            return sciezkaBezWzgledna;
        }

        public static string ObrazkiSciezka(this UrlHelper helper, string nazwaObrazka)
        {
            var ObrazkiFolder = AppConfig.ObrazkiFolderWzgledny;
            var sciezka = Path.Combine(ObrazkiFolder, nazwaObrazka);
            var sciezkaBezWzgledna = helper.Content(sciezka);

            return sciezkaBezWzgledna;
        }
    }
}