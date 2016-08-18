using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TWNewEgg.Models.ViewModels;
using TWNewEgg.Models.DomainModels;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;

namespace TWNewEgg.Models.WebsiteMappingRules
{
    public class ItemDetailMappingRule
    {
        public static void ItemDetailToItemBasic()
        {
            Mapper.CreateMap<int, TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemPaymentOption>().ConvertUsing(new ItemPaymentTypeConverter());
            Mapper.CreateMap<int, TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemDeliveryOption>().ConvertUsing(new ItemDeliveryTypeConverter());
            Mapper.CreateMap<int, TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemSourceOption>().ConvertUsing(new ItemSourceConverter());
            Mapper.CreateMap<ItemDetail, ItemBasic>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Main.ItemBase.ID))
                .ForMember(dest => dest.ProuctId, opt => opt.MapFrom(src => src.Main.ProductBase.ID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Main.ItemBase.Name))
                .ForMember(dest => dest.Slogan, opt => opt.MapFrom(src => src.Main.ItemBase.Sdesc))
                .ForMember(dest => dest.DelvType, opt => opt.MapFrom(src => src.Main.ItemBase.DelvType))
                .ForMember(dest => dest.SellerID, opt => opt.MapFrom(src => src.Main.ItemBase.SellerID))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Main.ItemBase.MarketPrice))
                .ForMember(dest => dest.PromotionPrice, opt => opt.MapFrom(src => src.Price.DisplayPrice))
                .ForMember(dest => dest.SourceDescription, opt => opt.MapFrom(src => src.Main.ItemBase.ItemDesc))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Main.ItemBase.DescTW))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.SellingQty))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PayTypes))
                .ForMember(dest => dest.DeliveryType, opt => opt.MapFrom(src => src.DeliverTypes))
                .ForMember(dest => dest.ItemSource, opt => opt.MapFrom(src => src.Main.SellerBase.CountryID))
                .ForMember(dest => dest.Spec, opt => opt.MapFrom(src => src.Main.ProductBase.SPEC))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Main.ItemBase.Status))
                .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => src.Main.ItemBase.DelvDate))
                .ForMember(dest => dest.Countdown, opt => opt.MapFrom(src=> src.GroupBuyEndDate))
                .ForMember(dest => dest.DateStart, opt => opt.MapFrom(src => src.Main.ItemBase.DateStart))
                .ForMember(dest => dest.DateEnd, opt => opt.MapFrom(src => src.Main.ItemBase.DateEnd))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Main.ItemBase.Spechead))
                .ForMember(dest => dest.ItemDeliveryType, opt => opt.MapFrom(src => src.Main.ItemBase.DelvType))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Main.ItemBase.CategoryID))
                .ForMember(dest => dest.SellerProductID, opt => opt.MapFrom(src => src.Main.ProductBase.SellerProductID))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.Main.ItemBase.IsNew))
                .ForMember(dest => dest.IsChooseAny, opt => opt.MapFrom(src => src.Main.IsChooseAny))
                .ForMember(dest => dest.ShowOrder, opt => opt.MapFrom(src => src.Main.ItemBase.ShowOrder))
                .ForMember(dest => dest.Discard4, opt => opt.MapFrom(src => src.Main.ItemBase.Discard4))
                .ForMember(dest => dest.BrandStory, opt => opt.MapFrom(src => src.BrandStory));
        }
    }

    public class ItemPaymentTypeConverter : ITypeConverter<int, TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemPaymentOption>
    {
        public TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemPaymentOption Convert(ResolutionContext context)
        {
            int PayTypeCode = (int)context.SourceValue;
            return (TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemPaymentOption)PayTypeCode;
        }
    }

    public class ItemDeliveryTypeConverter : ITypeConverter<int, TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemDeliveryOption>
    {
        public TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemDeliveryOption Convert(ResolutionContext context)
        {
            int DeliverTypeCode = (int)context.SourceValue;
            return (TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemDeliveryOption)DeliverTypeCode;
        }
    }

    public class ItemSourceConverter : ITypeConverter<int, TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemSourceOption>
    {
        public TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemSourceOption Convert(ResolutionContext context)
        {
            int ItemSourceCode = (int)context.SourceValue;
            return (TWNewEgg.Models.ViewModels.Item.ItemBasic.ItemSourceOption)ItemSourceCode;
        }
    }

    public class CountDownResolver : ValueResolver<ItemDetail, TimeSpan?>
    {
        protected override TimeSpan? ResolveCore(ItemDetail source)
        {
            TimeSpan? countDown = new TimeSpan();
            if (source.GroupBuyEndDate.HasValue)
            {
                countDown = DateTime.Now - source.GroupBuyEndDate;
            }
            return countDown;
        }
    }

    public class ItemImagesUrlResolver : ValueResolver<ItemDetail, List<string>>
    {
        string imageDomain = System.Configuration.ConfigurationManager.AppSettings["ECWebHttpImgDomain"];
        protected override List<string> ResolveCore(ItemDetail source)
        {
            List<string> imgurls = new List<string>();
            if (source.Main.ItemBase.PicStart.HasValue && source.Main.ItemBase.PicEnd.HasValue)
            {
                for (int i = source.Main.ItemBase.PicStart.Value; i <= source.Main.ItemBase.PicEnd; i++)
                {
                    string url = string.Format(imageDomain + "/Pic/item/{0}/{1}_{2}_60.jpg",
                        (source.Main.ItemBase.ID / 10000).ToString("0000"),
                        (source.Main.ItemBase.ID % 10000).ToString("0000"),
                        i.ToString());
                }
            }
            return imgurls;
        }
    }
}
