using MvcSiteMapProvider;
using RolnikowePole.DAL;
using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RolnikowePole.Infrastucture
{
    public class ZwierzeSzczegolyDynamicNodeProvider : DynamicNodeProviderBase
    {
        private RolnikowePoleContext db = new RolnikowePoleContext();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode nodee)
        {
            var returnValue = new List<DynamicNode>();

            foreach (Zwierze zwierze in db.Zwierzeta)
            {
                DynamicNode node = new DynamicNode()
                {
                    Title = zwierze.Nazwa,
                    Key = "Zwierze_" + zwierze.ZwierzeId,
                    ParentKey = "Gatunek_" + zwierze.GatunekId,
                };
                node.RouteValues.Add("id",zwierze.ZwierzeId);
                returnValue.Add(node);
            }

            return returnValue;
        }
    }
}