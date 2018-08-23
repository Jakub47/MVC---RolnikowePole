using MvcSiteMapProvider;
using RolnikowePole.DAL;
using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.Infrastucture
{
    public class GatunkiDynamicNodeProvider : DynamicNodeProviderBase
    {
        private RolnikowePoleContext db = new RolnikowePoleContext();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode nodee)
        {
            var returnValue = new List<DynamicNode>();

            foreach (Gatunek gatunek in db.Gatunki)
            {
                DynamicNode node = new DynamicNode()
                {
                    Title = gatunek.NazwaGatunku,
                    Key = "Gatunek_" + gatunek.GatunekId
                };
                node.RouteValues.Add("nazwaGatunku", gatunek.NazwaGatunku);
                returnValue.Add(node);
            }

            return returnValue;
            
        }
    }
}