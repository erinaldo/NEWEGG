using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Models.WebsiteMappingRules;

namespace TWNewEgg.ECWeb.App_Start
{
    public class AutoMapperConfig
    {
        public static void Bootstrapper()
        {
            //View to Domain model
            ViewDomainModelMappingRules.RegisterRules();
            //Domain to View model
            DomaintoViewModelMappingRules.RegisterRules();
            //DB to View model
            DBtoViewModelMappingRules.RegisterRules();
            //View to View model
            ViewtoViewModelMappingRules.RegisterRules();

            TWNewEgg.ECWeb.Models.OldCart.AutoMapper.ViewModelToViewModel.RegisterRules();
        }
    }
}