using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Models.ServiceMappingRules;

namespace TWNewEgg.ECService.App_Start
{
    public class AutoMapperConfig
    {
        public static void Bootstrapper()
        {
            DomainDBModelMappingRules.RegisterRules();
            DBDomainModelMapingRule.RegisterRules();
            DBModelMapingRule.RegisterRules();
        }
    }
}
