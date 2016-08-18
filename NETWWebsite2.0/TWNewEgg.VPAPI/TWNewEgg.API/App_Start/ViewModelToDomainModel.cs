using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TWNewEgg.API.Models.DomainModel;

namespace TWNewEgg.API.App_Start
{
    public class ViewModelToDomainModel : Profile
    {
        public override string ProfileName
        {
            get
            {
                return "ViewModelToDomainModel";
            }
        }
        protected override void Configure()
        {
            // ItemTemp ViewModel 轉換 ItemTemp List Domain Model
            AutoMapper.Mapper.CreateMap<Models.ItemSketch, TempList>()
                .ForMember(x => x.UpdateUserID, x => x.MapFrom(src => src.CreateAndUpdate.UpdateUser))
                .ForMember(x => x.ProductTempID, x => x.MapFrom(src => src.Product.ID))
                .ForMember(x => x.ProductID, x => x.MapFrom(src => src.Product.ProductID))
                .ForMember(x => x.ItemTempID, x => x.MapFrom(src => src.Item.ID))
                .ForMember(x => x.ItemID, x => x.MapFrom(src => src.Item.ItemID))
                .ForMember(x => x.ItemQty, x => x.MapFrom(src => src.Item.ItemQty))
                .ForMember(x => x.MarketPrice, x => x.MapFrom(src => src.Item.MarketPrice))
                .ForMember(x => x.Cost, x => x.MapFrom(src => src.Product.Cost))
                .ForMember(x => x.PriceCash, x => x.MapFrom(src => src.Item.PriceCash))
                .ForMember(x => x.Qty, x => x.MapFrom(src => src.ItemStock.CanSaleQty.Value + src.ItemStock.InventoryQtyReg))
                .ForMember(x => x.SafeQty, x => x.MapFrom(src => src.ItemStock.InventorySafeQty))
                .ForMember(x => x.ShipType, x => x.MapFrom(src => src.Item.ShipType))
                .ForMember(x => x.DateSatrt, x => x.MapFrom(src => src.Item.DateStart));
        }
    }
}
