using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Framework.Common;
using TWNewEgg.Models.DomainModels.PaymentGateway;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.Models.ServiceMappingRules
{
    public class DBDomainModelMapingRule
    {
        public static void RegisterRules()
        {


            #region //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160719
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWBACKENDDB.SellerCorrectionPriceDB, TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>();            
            #endregion


            #region 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdSetDB, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSetDM>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdContentDB, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdContentDM>();
            //Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdMenuDB, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdMenuDM>();
            //Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdDataDB, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdEditDM>();
            #endregion


            Mapper.CreateMap<ItemPriceInfo, ItemInfo>()
                .ForMember(dest => dest.ItemBase, opt => opt.MapFrom(src => src.item));

            #region ViewTracksCartItems
            Mapper.CreateMap<ViewTracksCartItems, CartItem>()
                .ForMember(dest => dest.CategoryID, opt => opt.MapFrom(src => src.CategoryID))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.TrackCreateDate))
                .ForMember(dest => dest.DelvDate, opt => opt.MapFrom(src => src.ItemDelvDate))
                .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.ItemID))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.ItemName))
                .ForMember(dest => dest.NTPrice, opt => opt.MapFrom(src => src.DisplayPrice ?? src.ItemPriceCash))
                .ForMember(dest => dest.OriginPrice, opt => opt.MapFrom(src => src.ItemPriceCash))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.TrackQty ?? 1))
                .ForMember(dest => dest.SellerID, opt => opt.MapFrom(src => src.ItemSellerID))
                .ForMember(dest => dest.TrackStatus, opt => opt.MapFrom(src => src.TrackStatus))
                .ForMember(dest => dest.ShowOrder, opt => opt.MapFrom(src => src.ItemShowOrder));
            #endregion

            Mapper.CreateMap<ItemPriceInfoSimplify, ItemBase>().ReverseMap();
            Mapper.CreateMap<Models.DBModels.TWSQLDB.Item, ItemBase>().ReverseMap();
            Mapper.CreateMap<SalesOrderGroup, SOGroupBase>().ReverseMap();
            Mapper.CreateMap<SalesOrder, SOBase>().ReverseMap();
            Mapper.CreateMap<SalesOrderItem, SOItemBase>().ReverseMap();
            Mapper.CreateMap<SalesOrderGroup, SalesOrderGroupTemp>().ReverseMap();
            Mapper.CreateMap<SalesOrder, SalesOrderTemp>().ReverseMap();
            Mapper.CreateMap<SalesOrderItem, SalesOrderItemTemp>().ReverseMap();
            
            #region GreetingWords -----------------------add by bruce 20160329
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.GreetingWordsDB, TWNewEgg.Models.DomainModels.GreetingWords.HomeGreetingWordsDM>(); //登入問候語
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.GreetingWordsDB, TWNewEgg.Models.DomainModels.GreetingWords.HolidayGreetingWordsDM>(); //問候卡
            #endregion


            #region // 依據 BSATW-173 廢四機需求增加 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160429
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Discard4DB, TWNewEgg.Models.DomainModels.Discard4.Discard4DM>(); //癈四機同意
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWBACKENDDB.Discard4ItemDB, TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>(); //癈四機回收四聯單
            #endregion

            Mapper.CreateMap<SalesOrderGroup, SalesOrderGroupTemp>().ReverseMap();
            Mapper.CreateMap<SalesOrder, SalesOrderTemp>().ReverseMap();
            Mapper.CreateMap<SalesOrderItem, SalesOrderItemTemp>().ReverseMap();
            

            #region VotingActivity投票活動
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup, TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems, TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec, TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec>()
                .ForMember(dest => dest.Rec, opt => opt.MapFrom(src => TWNewEgg.Framework.Common.JSONSerialization.Deserializer <List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRecDetail>>(src.Rec)));
            #endregion

            #region ItemGroup&ItemGroupProperty
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup, TWNewEgg.Models.DomainModels.Item.ItemGroup>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty, TWNewEgg.Models.DomainModels.Item.ItemGroupDetailProperty>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty, TWNewEgg.Models.DomainModels.Item.ItemGroupProperty>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup, TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName, TWNewEgg.Models.DomainModels.Item.ItemPropertyName>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue, TWNewEgg.Models.DomainModels.Item.ItemPropertyValue>();
            #endregion

            #region Cart
            Mapper.CreateMap<CartTemp, CartTempDM>();
            Mapper.CreateMap<CartItemTemp, CartItemTempDM>();
            Mapper.CreateMap<CartCouponTemp, CartCouponTempDM>();
            Mapper.CreateMap<Cart, CartDataMaintain_DM>().ReverseMap();
            Mapper.CreateMap<Retgood, RetgoodDataMaintain_DM>().ReverseMap();
            Mapper.CreateMap<refund2c, refund2cDataMaintain_DM>().ReverseMap();
            Mapper.CreateMap<Models.DBModels.TWSQLDB.PayType, Models.DomainModels.CartPayment.DM_PayType>().ReverseMap();
            #endregion

            #region Lottery
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.LotteryGame, TWNewEgg.Models.DomainModels.Lottery.Game>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.LotteryAward, TWNewEgg.Models.DomainModels.Lottery.Award>();
            #endregion

            #region Manufacture
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Manufacture, TWNewEgg.Models.DomainModels.Manufacture.Manufacture>();
            #endregion

            #region PaymentGateway
            Mapper.CreateMap<NCCCTrans, NCCCResult>().ReverseMap();
            Mapper.CreateMap<NCCCTrans, NCCCInput>().ReverseMap();
            #endregion

            #region HotWords
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.HotWords, TWNewEgg.Models.DomainModels.HotWords.HotWords>();
            #endregion

            #region View_ItemSellingQty
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty, TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty>();
            #endregion

            #region Redeem
            //Event & Coupon :DB To Domain
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Coupon, TWNewEgg.Models.DomainModels.Redeem.Coupon>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Event, TWNewEgg.Models.DomainModels.Redeem.Event>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.EventFile, TWNewEgg.Models.DomainModels.Redeem.EventFile>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.EventTempImport, TWNewEgg.Models.DomainModels.Redeem.EventTempImport>();
            //Event & Coupon : DB To DB
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Event, TWNewEgg.Models.DBModels.TWSQLDB.Event>().ForMember(dest => dest.createuser, opt => opt.Ignore()); ;
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.EventFile, TWNewEgg.Models.DBModels.TWSQLDB.EventFile>().ForMember(dest => dest.createuser, opt => opt.Ignore()); ;
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.EventTempImport, TWNewEgg.Models.DBModels.TWSQLDB.EventTempImport>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Coupon, TWNewEgg.Models.DBModels.TWSQLDB.Coupon>().ForMember(dest => dest.createuser, opt => opt.Ignore()); ;
            //Promotion : DB To Domain
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftBasic, TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBasic>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftBlackList, TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBlackList>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftWhiteList, TWNewEgg.Models.DomainModels.Redeem.PromotionGiftWhiteList>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftInterval, TWNewEgg.Models.DomainModels.Redeem.PromotionGiftInterval>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.PromotionGiftRecords, TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords>();
            //Promotion : DB to DB
            Mapper.CreateMap<Models.DBModels.TWSQLDB.PromotionGiftBasic, Models.DBModels.TWSQLDB.PromotionGiftBasic>().ForMember(dest => dest.CreateUser, opt=> opt.Ignore());
            Mapper.CreateMap<Models.DBModels.TWSQLDB.PromotionGiftBlackList, Models.DBModels.TWSQLDB.PromotionGiftBlackList>().ForMember(dest => dest.CreateUser, opt => opt.Ignore());
            Mapper.CreateMap<Models.DBModels.TWSQLDB.PromotionGiftWhiteList, Models.DBModels.TWSQLDB.PromotionGiftWhiteList>().ForMember(dest => dest.CreateUser, opt => opt.Ignore());
            Mapper.CreateMap<Models.DBModels.TWSQLDB.PromotionGiftInterval, Models.DBModels.TWSQLDB.PromotionGiftInterval>().ForMember(dest => dest.CreateUser, opt => opt.Ignore());
            Mapper.CreateMap<Models.DBModels.TWSQLDB.PromotionGiftRecords, Models.DBModels.TWSQLDB.PromotionGiftRecords>();
            #endregion

            #region CategoryImage
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.Category, TWNewEgg.Models.DomainModels.Category.CategoryDM>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3, TWNewEgg.Models.DomainModels.Category.AdLayer3DM>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.AdLayer3Item, TWNewEgg.Models.DomainModels.Category.AdLayer3ItemDM>();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.CategoryTopItem, TWNewEgg.Models.DomainModels.Category.CategoryTopItemDM>();
            #endregion

            #region SubCategory_OptionStore
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.SubCategory_OptionStore, TWNewEgg.Models.DomainModels.Store.SubCategory_OptionStore_DM>();
            #endregion

            #region 活動/中獎名單
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.PromoActive, TWNewEgg.Models.DomainModels.PromoActive.PromoActiveDM>().ReverseMap();
            #endregion
            #region 活動/中獎名單
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.PromoAwardLog, TWNewEgg.Models.DomainModels.PromoAwardLog.PromoAwardLogDM>().ReverseMap();
            #endregion

            #region BankBonus

            Mapper.CreateMap<BankBonus, TWNewEgg.Models.DomainModels.BankBonus.BankBonus_DM>().ReverseMap();
            Mapper.CreateMap<BankBonusTemp, TWNewEgg.Models.DomainModels.BankBonus.BankBonusTemp_DM>().ReverseMap();

            #endregion BankBonus

            #region Bank

            Mapper.CreateMap<Bank, TWNewEgg.Models.DomainModels.Bank.Bank_DM>().ReverseMap();

            #endregion Bank
            #region ItemProperty
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDBExtModels.DbItemProperty, TWNewEgg.Models.DomainModels.Item.ItemPropertyDM>().ReverseMap();
            #endregion


            #region Finance
            Mapper.CreateMap<DBModels.TWBACKENDDBExtModels.SapBapiAccDocumentInfo, DomainModels.Finance.SapBapiAccDocumentDM>().ReverseMap();
            Mapper.CreateMap<DBModels.TWBACKENDDB.Sap_BapiAccDocument_DocHeader, DomainModels.Finance.SapBapiAccDocumentDocHeaderDM>().ReverseMap();
            Mapper.CreateMap<DBModels.TWBACKENDDB.Sap_BapiAccDocument_DocDetail, DomainModels.Finance.SapBapiAccDocumentDocDetailDM>().ReverseMap();
            Mapper.CreateMap<DBModels.TWBACKENDDB.AccountsDocumentType.DocTypeEnum, DomainModels.Finance.DocTypeEnum>().ReverseMap();

            //Mapper.CreateMap<DBModels.TWSQLDB.SalesOrder, DomainModels.Finance.SalesOrderDM>().ReverseMap();
            Mapper.CreateMap<DBModels.TWBACKENDDBExtModels.CustomerInfo, DomainModels.Finance.ZNETW_CUSTOMERCUSTOMERDATA>().ReverseMap();
            Mapper.CreateMap<DBModels.TWBACKENDDBExtModels.SAPLogInfo, DomainModels.Finance.SAPLogDM>().ReverseMap();
            Mapper.CreateMap<DBModels.TWBACKENDDB.FinanceDocumentCreateNote, DomainModels.Finance.FinanceDocumentCreateNoteDM>().ReverseMap();
            Mapper.CreateMap<DBModels.TWBACKENDDB.FinDocTransLog, DomainModels.Finance.FinDocTransLogDM>().ReverseMap();
            #endregion

            #region Paytype

            Mapper.CreateMap<Models.DBModels.TWSQLDB.PayType, Models.DomainModels.CartPayment.DM_PayType>().ReverseMap();
            #endregion Paytype

            #region PaymentTerm
            Mapper.CreateMap<Models.DBModels.TWSQLDB.PaymentTerm, Models.DomainModels.CartPayment.DM_PaymentTerm>().ReverseMap();
            #endregion PaymentTerm

            #region BeneficiaryParty
            Mapper.CreateMap<Models.DBModels.TWSQLDB.BeneficiaryParty, Models.DomainModels.CartPayment.DM_BeneficiaryParty>().ReverseMap();
            #endregion BeneficiaryParty
        }
    }
}
