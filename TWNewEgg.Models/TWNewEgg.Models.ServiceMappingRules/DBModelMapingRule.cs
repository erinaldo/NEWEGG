using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

namespace TWNewEgg.Models.ServiceMappingRules
{
    public class DBModelMapingRule
    {
        public static void RegisterRules()
        {
            Mapper.CreateMap<Seller_FinanDetail, Seller_FinanDetail>()
                .ForMember(x => x.SN, y => y.Ignore());
        }
    }
}
