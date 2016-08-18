using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using TWNewEgg.API.Models;
using TWNewEgg.API.View;

namespace TWNewEgg.API.View
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.CreateMap<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo, API.Models.Seller_BasicafterInfo>()
                .ForMember(x => x.UpdateUserID, y => y.Ignore())
                .ForMember(x => x.UpdateDate, y => y.Ignore());
        }
    }
}