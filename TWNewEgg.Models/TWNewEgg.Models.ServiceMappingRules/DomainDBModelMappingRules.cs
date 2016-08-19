using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.Product;
using TWNewEgg.Models.DomainModels.Property;
using TWNewEgg.Models.DomainModels.Seller;
using TWNewEgg.Models.DomainModels.PaymentGateway;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;

namespace TWNewEgg.Models.ServiceMappingRules
{
    public class DomainDBModelMappingRules
    {
        public static void RegisterRules()
        {

            #region //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160719
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM, TWNewEgg.Models.DBModels.TWBACKENDDB.SellerCorrectionPriceDB>();            
            #endregion


            #region 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSetDM, TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdSetDB>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdContentDM, TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdContentDB>();
            //Mapper.CreateMap<TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdMenuDM, TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdMenuDB>();
            //Mapper.CreateMap<TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdEditDM, TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdDataDB>();
            #endregion


            #region GreetingWords  -----------------------add by bruce 20160329
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.GreetingWords.HomeGreetingWordsDM, TWNewEgg.Models.DBModels.TWSQLDB.GreetingWordsDB>(); //登入問候語
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.GreetingWords.HolidayGreetingWordsDM, TWNewEgg.Models.DBModels.TWSQLDB.GreetingWordsDB>(); //問候卡
            #endregion

            #region// 依據 BSATW-173 廢四機需求增加 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160429
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Discard4.Discard4DM, TWNewEgg.Models.DBModels.TWSQLDB.Discard4DB>(); //癈四機同意
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM, TWNewEgg.Models.DBModels.TWBACKENDDB.Discard4ItemDB>(); //癈四機回收四聯單
            #endregion

            #region Item
            Mapper.CreateMap<Product, ProductDetailDM>();
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.Item, ItemInfo>()
                .ForMember(dest => dest.ItemBase, opt => opt.MapFrom(src => src));
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.Item, ItemBase>();
            Mapper.CreateMap<Product, ProductBase>();

            Mapper.CreateMap<ItemDisplayPrice, ItemPrice>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.ItemPropertyGroup, PropertyGroup>()
                .ForMember(dest => dest.GroupID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupNameTW));
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup, PropertyGroup>()
                .ForMember(dest => dest.GroupID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupNameTW));
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.ItemPropertyName, PropertyKey>()
                .ForMember(dest => dest.PNID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PNName, opt => opt.MapFrom(src => src.PropertyNameTW));
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName, PropertyKey>()
                .ForMember(dest => dest.PNID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PNName, opt => opt.MapFrom(src => src.PropertyNameTW));
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.ItemPropertyValue, PropertyValue>()
                .ForMember(dest => dest.PVID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PVName, opt => opt.MapFrom(src => src.PropertyValueTW));
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue, PropertyValue>()
                .ForMember(dest => dest.PVID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PVName, opt => opt.MapFrom(src => src.PropertyValueTW));
            
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.ItemDisplayPrice, TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice>();
            Mapper.CreateMap<Models.DBModels.TWSQLDB.Item, ItemInfo>()
                .ForMember(dest => dest.ItemBase, opt => opt.MapFrom(src => src));
            #endregion

            #region View_ItemSellingQty
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty, TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty>();
            #endregion

            #region ItemGroup&ItemGroupProperty
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.ItemGroup, TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.ItemGroupDetailProperty, TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.ItemGroupProperty, TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup, TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.ItemPropertyName, TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.ItemPropertyValue, TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue>();
            #endregion

            #region Answer
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Account, TWNewEgg.Models.DomainModels.Answer.AccountInfo>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder, TWNewEgg.Models.DomainModels.Answer.SalesOrderInfo>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Problem, TWNewEgg.Models.DomainModels.Answer.ProbelmBase>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Answer, TWNewEgg.Models.DomainModels.Answer.AnswerBase>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem, TWNewEgg.Models.DomainModels.Answer.SalesOrderItemInfo>();
            #endregion

            #region Advertising

            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.Ads, DomainModels.Advertising.Ads>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.AdSet, DomainModels.Advertising.AdSet>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.AdvEvent, DomainModels.Advertising.AdvEvent>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.AdvEventType, DomainModels.Advertising.AdvEventType>().ReverseMap();

            #endregion

            #region Account & Member

            Mapper.CreateMap<DBModels.TWSQLDB.Account, DomainModels.Account.AccountDM>().ReverseMap()
                .ForAllMembers(opt => opt.Condition(src => (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull) || (src.SourceType == typeof(string) && src.SourceValue == string.Empty)));
            Mapper.CreateMap<DBModels.TWSQLDB.Member, DomainModels.Account.MemberDM>().ReverseMap();
            Mapper.CreateMap<DBModels.TWSQLDB.Account, DBModels.TWSQLDB.Member>()
                .ForMember(dest => dest.AccID, opt => opt.MapFrom(src => src.ID));
            Mapper.CreateMap<DomainModels.Account.AccountDM, DomainModels.Account.MemberDM>()
                .ForMember(dest => dest.AccID, opt => opt.MapFrom(src => src.ID));

            #endregion

            #region AddressBokk & CompanyBook

            Mapper.CreateMap<DBModels.TWSQLDB.AddressBook, DomainModels.Account.AddressBookDM>().ReverseMap();
            Mapper.CreateMap<DBModels.TWSQLDB.CompanyBook, DomainModels.Account.CompanyBookDM>().ReverseMap();

            #endregion

            #region Seller
            Mapper.CreateMap<DBModels.TWSQLDB.Seller, DomainModels.Seller.SellerBase>().ReverseMap();
            #endregion

            #region VotingActivity投票活動
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup, TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems, TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec, TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec>()
                .ForMember(dest => dest.Rec, opt => opt.MapFrom(src => TWNewEgg.Framework.Common.JSONSerialization.Serializer(src.Rec)));
            #endregion

            #region Cart
            Mapper.CreateMap<CartTemp, CartTempDM>().ReverseMap();
            Mapper.CreateMap<CartItemTemp, CartItemTempDM>().ReverseMap();
            Mapper.CreateMap<CartCouponTemp, CartCouponTempDM>().ReverseMap();
            Mapper.CreateMap<Auth, TWNewEgg.Models.DomainModels.Auth.AuthDM>().ReverseMap();
            #endregion

            #region Lottery
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Lottery.Game, TWNewEgg.Models.DBModels.TWSQLDB.LotteryGame>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Lottery.Award, TWNewEgg.Models.DBModels.TWSQLDB.LotteryAward>();
            #endregion

            #region Manufacture
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Manufacture.Manufacture, TWNewEgg.Models.DBModels.TWSQLDB.Manufacture>();
            #endregion

            #region HotWords
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.HotWords.HotWords, TWNewEgg.Models.DBModels.TWSQLDB.HotWords>();
            #endregion

            #region PageMgmt
            Mapper.CreateMap<DSPageInfo, PageInfo>().ReverseMap();
            Mapper.CreateMap<DSComponentInfo, ComponentInfo>().ReverseMap();
            Mapper.CreateMap<TextObject, TextObject>()
                .ForMember(dest => dest.InDate, opt => opt.Ignore())
                .ForMember(dest => dest.InUser, opt => opt.Ignore());
            Mapper.CreateMap<ImageObject, ImageObject>()
                .ForMember(dest => dest.InDate, opt => opt.Ignore())
                .ForMember(dest => dest.InUser, opt => opt.Ignore());
            Mapper.CreateMap<DynamicObject, DynamicObject>()
                .ForMember(dest => dest.InDate, opt => opt.Ignore())
                .ForMember(dest => dest.InUser, opt => opt.Ignore());
            #endregion

            #region Redeem
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Redeem.Coupon, TWNewEgg.Models.DBModels.TWSQLDB.Coupon>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Redeem.Event, TWNewEgg.Models.DBModels.TWSQLDB.Event>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Redeem.EventFile, TWNewEgg.Models.DBModels.TWSQLDB.EventFile>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Redeem.EventTempImport, TWNewEgg.Models.DBModels.TWSQLDB.EventTempImport>();

            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBasic, TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftBasic>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBlackList, TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftBlackList>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftWhiteList, TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftWhiteList>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftInterval, TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftInterval>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords, TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftRecords>();
            #endregion

            #region CategoryImage
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Category.CategoryDM, TWNewEgg.Models.DBModels.TWSQLDB.Category>()
                .ForAllMembers(opt => opt.Condition(src =>
                    (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull)  //|| (src.SourceType == typeof(string)&& src.SourceValue== string.Empty )
                ));
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Category.AdLayer3DM, TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition(src =>
                    (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull) || (src.SourceType == typeof(string) && src.SourceValue == string.Empty)
                ));
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Category.AdLayer3ItemDM, TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3Item>()
                .ForAllMembers(opt => opt.Condition(src =>
                    (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull) || (src.SourceType == typeof(string) && src.SourceValue == string.Empty)
                ));
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Category.CategoryTopItemDM, TWNewEgg.Models.DBModels.TWSQLDB.CategoryTopItem>()
                .ForAllMembers(opt => opt.Condition(src =>
                    (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull) || (src.SourceType == typeof(string) && src.SourceValue == string.Empty)
                ));
            #endregion

            #region SubCategory_OptionStore
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Store.SubCategory_OptionStore_DM, TWNewEgg.Models.DBModels.TWSQLDB.SubCategory_OptionStore>();
            #endregion
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.PromoActive.PromoActiveDM, TWNewEgg.Models.DBModels.TWSQLDB.PromoActive>()
                .ForAllMembers(opt => opt.Condition(src =>
                    (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull) || (src.SourceType == typeof(string) && src.SourceValue == string.Empty)
                ));
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.PromoAwardLog.PromoAwardLogDM, TWNewEgg.Models.DBModels.TWSQLDB.PromoAwardLog>()
                .ForAllMembers(opt => opt.Condition(src =>
                    (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull) || (src.SourceType == typeof(string) && src.SourceValue == string.Empty)
                ));

            #region BankBonus

            Mapper.CreateMap<TWNewEgg.Models.DomainModels.BankBonus.BankBonus_DM, BankBonus>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.BankBonus.BankBonusTemp_DM, BankBonusTemp>().ReverseMap();
            Mapper.CreateMap<BankBonusTemp, BankBonus>().ReverseMap();

            #endregion BankBonus

            #region Bank

            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Bank.Bank_DM, Bank>().ReverseMap();

            #endregion Bank

            #region AdditionalItem
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.AdditionalItemForCart, TWNewEgg.Models.DomainModels.AdditionalItem.AIForCartDM>().ReverseMap();
            #endregion

            #region SalesOrderGroup
            Mapper.CreateMap<SalesOrderGroup, SOGroupBase>().ReverseMap();
            Mapper.CreateMap<SOItemBase, SalesOrderItem>().ReverseMap();
            #endregion

            #region so to soTemp
            Mapper.CreateMap<SalesOrderGroup, SalesOrderGroupTemp>().ReverseMap();
            Mapper.CreateMap<SalesOrder, SalesOrderTemp>().ReverseMap();
            Mapper.CreateMap<SalesOrderItem, SalesOrderItemTemp>().ReverseMap();
            #endregion


            #region Installment

            Mapper.CreateMap<TWNewEgg.Models.DomainModels.ItemInstallment.ItemTopInstallment, TWNewEgg.Models.DBModels.TWSQLDB.ItemTopInstallment>().ReverseMap();

            #endregion Installment
        }
    }
}
