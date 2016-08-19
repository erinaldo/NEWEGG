using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using TWNewEgg.API.Models;
using TWNewEgg.API.App_Start;

namespace TWNewEgg.API
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            //Mapper.Initialize();
            Mapper.Initialize(x =>
            {
                x.AddProfile<EntiyToDBModel>();
                x.AddProfile<ViewModelToDomainModel>();
            });
        }
    }

    public class EntiyToDBModel : Profile
    {
        public override string ProfileName
        {
            get
            {
                return "EntiyToDBModel";
            }
        }
        protected override void Configure()
        {
            AutoMapper.Mapper.CreateMap<ItemSketch_Item, TWNewEgg.DB.TWSQLDB.Models.ItemTemp>()
                .ForMember(opt => opt.Qty, x => x.MapFrom(src => src.CanSaleLimitQty.Value + src.ItemQtyReg.Value));
            AutoMapper.Mapper.CreateMap<ItemSketch_Product, TWNewEgg.DB.TWSQLDB.Models.ProductTemp>()
                .ForMember(opt => opt.BarCode, x => x.Ignore());
        }

    }
}