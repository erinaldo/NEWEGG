using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.Models.DomainModels;
using TWNewEgg.Models.ViewModels;

namespace TWNewEgg.Models.ServiceMappingRules
{
    public class ViewDomainModelMappingRules
    {
        public static void RegisterRules()
        {
            //#region // 依據 BSATW-173 廢四機需求增加 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160429
            //Mapper.CreateMap<TWNewEgg.Models.ViewModels.Discard4.Discard4VM, TWNewEgg.Models.DomainModels.Discard4.Discard4DM>()
            //    .ForMember(dest => dest.CreateUser, opt => opt.MapFrom(src => src.AccountEmail)); //癈四機同意
            //Mapper.CreateMap<TWNewEgg.Models.ViewModels.Discard4.Discard4ItemVM, TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>(); //癈四機回收四聯單
            //#endregion
        }
    }
}
