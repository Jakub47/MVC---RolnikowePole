using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RolnikowePole.Infrastucture
{
    public class AppConfig
    {
        private static string _IkonyGatunkowFolderWzgledny = ConfigurationManager.AppSettings["IkonyGatunkowFolder"];

        public static string IkonyGatunkowFolderWzgledny
        {
            get
            {
                return _IkonyGatunkowFolderWzgledny;
            }
        }

        private static string _obrazkiFolderWzgledny = ConfigurationManager.AppSettings["ObrazkiFolder"];

        public static string ObrazkiFolderWzgledny
        {
            get
            {
                return _obrazkiFolderWzgledny;
            }
        }

    }
}