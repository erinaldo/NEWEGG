using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels;
using TWNewEgg.Models.DomainModels;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.Models.WebsiteMappingRules
{

   

    public class ViewDomainModelMappingRules
    {
        public static void RegisterRules()
        {

            #region //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160719
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Seller.SellerCorrectionPriceVM, TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>().ReverseMap();
            #endregion


            #region //依據 BSATW-177 手機改版需求增加---------------add by bruce 20160615
            //行動設備的廣告設定
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdSetVM, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSetDM>().ReverseMap();
            //行動設備的廣告內文
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdContentDM>().ReverseMap();
            //行動設備首頁廣告的子選單下的廣告資料
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdEditVM, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdEditDM>().ReverseMap();
            //行動設備首頁廣告的一個左方選單
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdMenuVM, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdMenuDM>().ReverseMap();
            //行動設備的廣告資料查詢
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdSearchVM, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM>().ReverseMap();
            //行動設備首頁廣告的子選單下的廣告資料
            //EC用顯示用
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM, TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdShowDM>().ReverseMap();
            #endregion


            #region // 依據 BSATW-173 廢四機需求增加 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160429
            //癈四機同意
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Discard4.Discard4VM, TWNewEgg.Models.DomainModels.Discard4.Discard4DM>()
                .ForMember(dest => dest.CreateUser, opt => opt.MapFrom(src => src.AccountEmail));
            //癈四機回收四聯單
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Discard4.Discard4ItemVM, TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>();
            #endregion

            #region 登入問候語, 問候卡 GreetingWords -----------------------add by bruce 20160331 WMTWNOR-3394
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.GreetingWords.HomeGreetingWordsVM, TWNewEgg.Models.DomainModels.GreetingWords.HomeGreetingWordsDM>().ReverseMap(); //問候語
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.GreetingWords.HolidayGreetingWordsVM, TWNewEgg.Models.DomainModels.GreetingWords.HolidayGreetingWordsDM>().ReverseMap(); //問候卡
            #endregion

            Mapper.CreateMap<ViewModels.Product.ProductDetail, DomainModels.Product.ProductDetailDM>().ReverseMap();
            Mapper.CreateMap<ViewModels.Item.CategoryItemForm, DomainModels.Item.CategoryItemConditions>().ReverseMap();

            #region HiTrust

            Mapper.CreateMap<DomainModels.PaymentGateway.HiTrustAuth, DBModels.TWSQLDB.HiTrustTrans>().ReverseMap();
            Mapper.CreateMap<DomainModels.PaymentGateway.HiTrustQueryData, DBModels.TWSQLDB.HiTrustQuery>().ReverseMap();
            Mapper.CreateMap<DomainModels.PaymentGateway.HiTrustAuth, DBModels.TWSQLDB.HiTrustTransLog>().ReverseMap();
            Mapper.CreateMap<DomainModels.PaymentGateway.HiTrustQueryData, DBModels.TWSQLDB.HiTrustQueryLog>().ReverseMap();

            #endregion

            #region Advertising
            Mapper.CreateMap<ViewModels.Advertising.Ads, DomainModels.Advertising.Ads>().ReverseMap();
            Mapper.CreateMap<ViewModels.Advertising.AdSet, DomainModels.Advertising.AdSet>().ReverseMap();
            Mapper.CreateMap<ViewModels.Advertising.AdvEvent, DomainModels.Advertising.AdvEvent>().ReverseMap();
            Mapper.CreateMap<ViewModels.Advertising.AdvEventType, DomainModels.Advertising.AdvEventType>().ReverseMap();
            Mapper.CreateMap<ViewModels.Advertising.AdvEventType, DomainModels.Advertising.AdvEventType>().ReverseMap();
            Mapper.CreateMap<ViewModels.Advertising.AdvEventDisplay, DomainModels.Advertising.AdvEventDisplay>().ReverseMap();
            #endregion

            #region Category
            Mapper.CreateMap<ViewModels.Category.Category_TreeItem, DomainModels.Category.Category_TreeItem>().ReverseMap();
            #endregion

            #region Property
            Mapper.CreateMap<ViewModels.Category.Category_TreeItem, DomainModels.Category.Category_TreeItem>().ReverseMap();
            #endregion

            #region Account
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Register.RegisterVM, TWNewEgg.Models.DomainModels.Account.AccountDM>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => ((!string.IsNullOrEmpty(src.Lastname)) ? src.Lastname : string.Empty) + ((!string.IsNullOrEmpty(src.Firstname)) ? src.Firstname : string.Empty)))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday == null ? "" : src.Birthday.Value.ToString("yyyy/MM/dd")))
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex))
                .ForMember(dest => dest.AgreePaper, opt => opt.MapFrom(src => src.AgreePaper.Value))
                .ForMember(dest => dest.MessagePaper, opt => opt.MapFrom(src => src.MessagePaper.Value))
                .ForMember(dest => dest.Registeron, opt => opt.Ignore())
                .ForMember(dest => dest.Loginon, opt => opt.Ignore())
                .ForMember(dest => dest.RememberMe, opt => opt.Ignore())
                //.ForMember(dest => dest.AgreePaper, opt => opt.Ignore())
                //.ForMember(dest => dest.MessagePaper, opt => opt.Ignore())
                .ForMember(dest => dest.LoginStatus, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.LockedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Chkfailcnt, opt => opt.Ignore())
                .ForMember(dest => dest.Marrige, opt => opt.Ignore())
                .ForMember(dest => dest.Job, opt => opt.Ignore())
                .ForMember(dest => dest.Income, opt => opt.Ignore())
                .ForMember(dest => dest.Degree, opt => opt.Ignore())
                .ForMember(dest => dest.Subscribe, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmDate, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore());
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Login.Login, TWNewEgg.Models.DomainModels.Account.AccountInfoFB>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Account.AccountDM, TWNewEgg.Models.ViewModels.Account.AccountVM>();
            #endregion

            #region Answer 

            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Answer.AnswerViewModel, DomainModels.Answer.SalesOrderInfo>().ReverseMap();
            #endregion

            #region Redeem
            //Event & Coupon
            Mapper.CreateMap<ViewModels.Redeem.Event, DomainModels.Redeem.Event>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.Coupon, DomainModels.Redeem.Coupon>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.Coupon.CouponActiveTypeOption, DomainModels.Redeem.Coupon.CouponActiveTypeOption>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.Coupon.ValidTypeOption, DomainModels.Redeem.Coupon.ValidTypeOption>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.Coupon.CouponUsedStatusOption, DomainModels.Redeem.Coupon.CouponUsedStatusOption>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.Coupon.AddCouponStatusOption, DomainModels.Redeem.Coupon.AddCouponStatusOption>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.UserCouponsProducts, DomainModels.Redeem.UserCouponsProducts>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.CouponsLite, DomainModels.Redeem.CouponsLite>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.ItemLite, DomainModels.Redeem.ItemLite>().ReverseMap();
            //Promotion
            Mapper.CreateMap<ViewModels.Redeem.PromotionGiftBasic, DomainModels.Redeem.PromotionGiftBasic>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.PromotionGiftBlackList, DomainModels.Redeem.PromotionGiftBlackList>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.PromotionGiftWhiteList, DomainModels.Redeem.PromotionGiftWhiteList>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.PromotionGiftInterval, DomainModels.Redeem.PromotionGiftInterval>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.PromotionGiftRecords, DomainModels.Redeem.PromotionGiftRecords>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.PromotionInput, DomainModels.Redeem.PromotionInput>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.PromotionGiftDetail, DomainModels.Redeem.PromotionGiftDetail>().ReverseMap();
            Mapper.CreateMap<ViewModels.Redeem.PromotionDetail, DomainModels.Redeem.PromotionDetail>().ReverseMap();
            #endregion

            #region Store
            Mapper.CreateMap<ViewModels.Store.Breadcrumbs, DomainModels.Store.Breadcrumbs>().ReverseMap();
            Mapper.CreateMap<ViewModels.Store.BreadcrumbItem, DomainModels.Store.BreadcrumbItem>().ReverseMap();
            Mapper.CreateMap<ViewModels.Store.SubCategory_OptionStore_VM, DomainModels.Store.SubCategory_OptionStore_DM>().ReverseMap();
            #endregion

            #region Cart
            Mapper.CreateMap<ViewModels.Cart.CartStep1Data, DomainModels.Cart.CartTempDM>()
                .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.SerialNumber))
                .ForMember(dest => dest.PayType, opt => opt.MapFrom(src => src.CartPaytype_View.PayType0rateNum))
                .ForMember(dest => dest.BankID, opt => opt.MapFrom(src => src.CartPaytype_View.BankID))
                .ForMember(dest => dest.PayTypeGroupID, opt => opt.MapFrom(src => src.CartPaytype_View.PayTypeGroupID))
                .ForMember(dest => dest.CartTypeID, opt => opt.MapFrom(src => src.CartTypeID))
                .ForMember(dest => dest.CartItemTempDMs, opt => opt.MapFrom(src => src.CartItemDetailList_View))
                .ForMember(dest => dest.CartCouponTempDMs, opt => opt.MapFrom(src => src.CouponList));
            Mapper.CreateMap<ViewModels.Cart.CartItem_View, DomainModels.Cart.CartItemTempDM>();
            Mapper.CreateMap<ViewModels.Cart.CartItemDetail_View, DomainModels.Cart.CartItemTempDM>();
            Mapper.CreateMap<ViewModels.Redeem.Coupon, DomainModels.Cart.CartCouponTempDM>()
                .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.ItemId))
                .ForMember(dest => dest.CouponNumber, opt => opt.MapFrom(src => src.number));
            #endregion

            #region VotingActivity投票活動
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup, TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityGroup>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems, TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityItems>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec, TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityRec>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRecDetail, TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityRecDetail>();
            #endregion

            #region HotWords
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.HotWords.HotWords, TWNewEgg.Models.ViewModels.HotWords.HotWords>();
            #endregion

            #region BankBonus
            
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.BankBonus.BankBonus_VM, TWNewEgg.Models.DomainModels.BankBonus.BankBonus_DM>();
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.BankBonus.BankBonusTemp_VM, TWNewEgg.Models.DomainModels.BankBonus.BankBonusTemp_DM>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<TWNewEgg.Models.ViewModels.BankBonus.BankBonus_VM>>, TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<TWNewEgg.Models.DomainModels.BankBonus.BankBonus_DM>>>();

            #endregion BankBonus
            
            #region HotWords
            Mapper.CreateMap<ViewModels.Product.ProductDetail, DomainModels.Product.ProductDetailDM>().ReverseMap();
            Mapper.CreateMap<ViewModels.Item.CategoryItemForm, DomainModels.Item.CategoryItemConditions>().ReverseMap();
            #endregion



            #region Bank
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Bank.Bank, TWNewEgg.Models.DomainModels.Bank.Bank_DM>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Bank.Bank, TWNewEgg.DB.TWSQLDB.Models.Bank>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Bank.PayType0rateNumAndBank, TWNewEgg.Models.DomainModels.Bank.PayType0rateNumAndBank>().ReverseMap();
            #endregion

            #region 
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.PayType, TWNewEgg.Models.DomainModels.CartPayment.DM_PayType>().ReverseMap();
            #endregion

        }
    }

    public class DomaintoViewModelMappingRules
    {
        public static void RegisterRules()
        {

            #region //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160719            
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM, ViewModels.Seller.SellerCorrectionPriceVM>().ReverseMap();
            #endregion


            #region //依據 BSATW-177 手機改版需求增加---------------add by bruce 20160620
            //首頁輪播
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM, TWNewEgg.Models.ViewModels.DeviceAd.HomeSilderBannerVM>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.imgUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.link, opt => opt.MapFrom(src => src.Clickpath))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name));

            //生活提案
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM, TWNewEgg.Models.ViewModels.DeviceAd.HomeLifeProjectVM>()
                .ForMember(dest => dest.tabTitle, opt => opt.MapFrom(src => src.AdSet.SubName))
                .ForMember(dest => dest.sub, opt => opt.MapFrom(src => src.ListAdContent.FirstOrDefault()));

            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM, TWNewEgg.Models.ViewModels.DeviceAd.SubLifeProjectVM>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.desc, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.desc2, opt => opt.MapFrom(src => src.Name2))
                .ForMember(dest => dest.imgUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.link, opt => opt.MapFrom(src => src.Clickpath));

            //全館分類
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM, TWNewEgg.Models.ViewModels.DeviceAd.HomeCategoryMapVM>()
               .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
               .ForMember(dest => dest.imgUrl, opt => opt.MapFrom(src => src.ImageUrl))
               .ForMember(dest => dest.link, opt => opt.MapFrom(src => src.Clickpath))
               .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name));


            //促銷文案
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM, TWNewEgg.Models.ViewModels.DeviceAd.HomePromoProjectVM>()
               .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
               .ForMember(dest => dest.link, opt => opt.MapFrom(src => src.Clickpath))
               .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name));

            //美國直購            
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM, TWNewEgg.Models.ViewModels.DeviceAd.HomeShopUsVM>()
               .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
               .ForMember(dest => dest.imgUrl, opt => opt.MapFrom(src => src.ImageUrl))
               .ForMember(dest => dest.link, opt => opt.MapFrom(src => src.Clickpath))
               .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name));


            //手機版櫥窗輪播---------------add by bruce 20160622
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM, TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.imgUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.link, opt => opt.MapFrom(src => src.Clickpath))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.categoryId, opt => opt.MapFrom(src => src.CategoryID));



            //手機版分類輪播---------------add by bruce 20160622
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM, TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.imgUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.link, opt => opt.MapFrom(src => src.Clickpath))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.categoryId, opt => opt.MapFrom(src => src.CategoryID));

            #endregion


            #region 登入問候語, 節日問候卡 GreetingWords -----------------------add by bruce 20160331
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.GreetingWords.HomeGreetingWordsDM, ViewModels.GreetingWords.HomeGreetingWordsVM>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.GreetingWords.HolidayGreetingWordsDM, ViewModels.GreetingWords.HolidayGreetingWordsVM>().ReverseMap();
            #endregion


            #region Item
            Mapper.CreateMap<ItemInfo, TWNewEgg.Models.ViewModels.Item.CategoryItemInfo_View>()
                .ConvertUsing(src => Mapper.Map<TWNewEgg.Models.ViewModels.Item.CategoryItemInfo_View>(src.ItemBase));
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Category.CategoryAreaInfo, TWNewEgg.Models.ViewModels.Item.CategoryItemInfo_View>()
                .ConvertUsing(src => Mapper.Map<TWNewEgg.Models.ViewModels.Item.CategoryItemInfo_View>(src.ItemBaseList));
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Property.PriceWithQty, TWNewEgg.Models.ViewModels.Property.PriceWithQty_View>().ReverseMap();

            Mapper.CreateMap<ItemBase, TWNewEgg.Models.ViewModels.Item.CategoryItemInfo_View>()
                .ForMember(dest => dest.SellingQty, opt => opt.MapFrom(src => src.Qty));
            Mapper.CreateMap<ImageUrlReferenceDM, TWNewEgg.Models.ViewModels.Item.ItemUrl>();
            ItemDetailMappingRule.ItemDetailToItemBasic();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.ItemBase, TWNewEgg.Models.ViewModels.Cart.CartItemBase_View>()
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition(src =>
                    (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull) || (src.SourceType == typeof(string) && src.SourceValue == string.Empty)
                ));

            //View_ItemSellingQty
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty, TWNewEgg.Models.ViewModels.Item.View_ItemSellingQty>();
            #endregion

            #region ItemGroup&ItemGroupProperty
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Item.ItemMarketGroup, TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>().ReverseMap();
            #endregion
            #region AllAIForCart
            Mapper.CreateMap<Models.DomainModels.AdditionalItem.AllAIForCart, ViewModels.AdditionalItem.AllAIForCart>().ReverseMap();
            #endregion
            #region AIForCartDM
            Mapper.CreateMap<Models.DomainModels.AdditionalItem.AIForCartDM, ViewModels.AdditionalItem.AIForCartDM>().ReverseMap();
            #endregion
            #region Property
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Property.PropertyGroup, TWNewEgg.Models.ViewModels.Property.PropertyGroup_View>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Property.PropertyKey, TWNewEgg.Models.ViewModels.Property.PropertyKey_View>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Property.PropertyValue, TWNewEgg.Models.ViewModels.Property.PropertyValue_View>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyViewInfo, TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>().ReverseMap();
            #endregion

            #region Account
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Account.AccountDM, TWNewEgg.Models.ViewModels.Account.AccountVM>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Account.MemberDM, TWNewEgg.Models.ViewModels.Account.MemberVM>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Account.AccountInfoDM, TWNewEgg.Models.ViewModels.Account.AccountInfoVM>()
                .ForMember(dest => dest.AVM, opt => opt.MapFrom(src => src.ADM))
                .ForMember(dest => dest.MVM, opt => opt.MapFrom(src => src.MDM)).ReverseMap();
            #endregion

            #region AddressBook & CompanyBook
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Account.CartMemberInfoDM, TWNewEgg.Models.ViewModels.Account.CartMemberInfoVM>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Account.AddressBookDM, TWNewEgg.Models.ViewModels.Account.AddressBookVM>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Account.CompanyBookDM, TWNewEgg.Models.ViewModels.Account.CompanyBookVM>().ReverseMap();
            #endregion

            #region Redeem
            //Event & Coupon
            Mapper.CreateMap<DomainModels.Redeem.Event, ViewModels.Redeem.Event>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.Coupon, ViewModels.Redeem.Coupon>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.Coupon.CouponActiveTypeOption, ViewModels.Redeem.Coupon.CouponActiveTypeOption>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.Coupon.ValidTypeOption, ViewModels.Redeem.Coupon.ValidTypeOption>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.Coupon.CouponUsedStatusOption, ViewModels.Redeem.Coupon.CouponUsedStatusOption>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.Coupon.AddCouponStatusOption, ViewModels.Redeem.Coupon.AddCouponStatusOption>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.UserCouponsProducts, ViewModels.Redeem.UserCouponsProducts>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.CouponsLite, ViewModels.Redeem.CouponsLite>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.ItemLite, ViewModels.Redeem.ItemLite>().ReverseMap();
            //Promotion
            Mapper.CreateMap<ViewModels.Redeem.PromotionDetail, ViewModels.Promotion.PromotionDetail_View>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionDetail, ViewModels.Promotion.PromotionDetail_View>().ReverseMap();
            Mapper.CreateMap(typeof(TWNewEgg.Models.DomainModels.Redeem.ActionResponse<>), typeof(TWNewEgg.Models.ViewModels.Redeem.ActionResponse<>));
            Mapper.CreateMap<DomainModels.Redeem.PromotionDetail, ViewModels.Redeem.PromotionDetail>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.GetItemTaxDetail, ViewModels.Redeem.GetItemTaxDetail>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionGiftInterval, ViewModels.Redeem.PromotionGiftInterval>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionGiftBlackList, ViewModels.Redeem.PromotionGiftBlackList>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionGiftWhiteList, ViewModels.Redeem.PromotionGiftWhiteList>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionGiftBasic, ViewModels.Redeem.PromotionGiftBasic>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionGiftDetail, ViewModels.Redeem.PromotionGiftDetail>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionGiftRecords, ViewModels.Redeem.PromotionGiftRecords>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionInput, ViewModels.Redeem.PromotionInput>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionGiftExportToExcel, ViewModels.Redeem.PromotionGiftExportToExcel>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.PromotionGiftImportCheckData, ViewModels.Redeem.PromotionGiftImportCheckData>().ReverseMap();
            #endregion
            
            #region Property
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Property.PropertyGroup, TWNewEgg.Models.ViewModels.Property.PropertyGroup_View>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Property.PropertyKey, TWNewEgg.Models.ViewModels.Property.PropertyKey_View>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Property.PropertyValue, TWNewEgg.Models.ViewModels.Property.PropertyValue_View>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.GroupBuy.GroupBuyViewInfo, TWNewEgg.Models.ViewModels.GroupBuy.GroupBuyViewInfo>().ReverseMap();
            #endregion
            #region Answer
            Mapper.CreateMap<DomainModels.Answer.AnswerBase, TWNewEgg.Models.ViewModels.Answer.Answer>().ReverseMap();
            Mapper.CreateMap<DomainModels.Answer.ProbelmBase, TWNewEgg.Models.ViewModels.Answer.Probelm>().ReverseMap();
            Mapper.CreateMap<DomainModels.Answer.SalesOrderInfo, TWNewEgg.Models.ViewModels.Answer.AnswerViewModel>().ReverseMap();
            #endregion
            #region Track
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Track.TrackItem, TWNewEgg.Models.ViewModels.Track.TrackItem_View>().ReverseMap();
            #endregion

            #region Cart
            Mapper.CreateMap<DomainModels.Cart.CartItem, ViewModels.Cart.CartItem_View>().ReverseMap();
            Mapper.CreateMap<DomainModels.Cart.CartItemBase, ViewModels.Cart.CartItemBase_View>().ReverseMap();
            Mapper.CreateMap<DomainModels.Cart.CartItemClass, ViewModels.Cart.CartItemClass_View>().ReverseMap();
            Mapper.CreateMap<DomainModels.Cart.GroupDiscount, ViewModels.Cart.GroupDiscount_View>().ReverseMap();
            Mapper.CreateMap<DomainModels.Cart.ShoppingCartDM, ViewModels.Cart.ShoppingCart_View>().ReverseMap();
            Mapper.CreateMap<DomainModels.CartPayment.DM_CartPayment, ViewModels.CartPayment.VM_CartPayment>().ReverseMap();
            Mapper.CreateMap<DomainModels.CartPayment.DM_PaymentTerm, ViewModels.CartPayment.VM_PaymentTerm>().ReverseMap();

            Mapper.CreateMap<DomainModels.CartPayment.DM_PayType, ViewModels.CartPayment.VM_PayType>().ReverseMap();
            Mapper.CreateMap<DomainModels.CartPayment.DM_BeneficiaryParty, ViewModels.CartPayment.VM_BeneficiaryParty>().ReverseMap();
            Mapper.CreateMap<DomainModels.Bank.Bank_DM, ViewModels.CartPayment.VM_Bank>().ReverseMap();
            Mapper.CreateMap<DomainModels.BankBonus.BankBonus_DM, ViewModels.CartPayment.VM_BankBonus>().ReverseMap();
            #endregion

            #region Activity
            Mapper.CreateMap<DomainModels.Activity.ActivityDM, ViewModels.Activity.ActivityVM>()
             .ForAllMembers(opt => opt.Condition(src =>
                 (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull) || (src.SourceType == typeof(string) && src.SourceValue == string.Empty)
             ));
            Mapper.CreateMap<ViewModels.Activity.ActivitySectionInfor, ViewModels.Activity.ActivityVM>()
               .ForMember(opt => opt.Header, src => src.NullSubstitute("true"))
               .ForMember(opt => opt.Topper, src => src.NullSubstitute("true"))
               .ForMember(opt => opt.Bottomer, src => src.NullSubstitute("true"))
               .ForMember(opt => opt.Footer, src => src.NullSubstitute("true"))
               .ForMember(opt => opt.FloatMenu, src => src.NullSubstitute("true"));
            Mapper.CreateMap<DomainModels.PromoActive.AwardListSearchConditionDM, ViewModels.Activity.AwardListSearchCondition>().ReverseMap();
            Mapper.CreateMap<DomainModels.PromoActive.PromoActiveDM, ViewModels.Activity.PromoActive>().ReverseMap();
            Mapper.CreateMap<DomainModels.PromoActive.AwardDM, ViewModels.Activity.Award>().ReverseMap();
            Mapper.CreateMap<ResponsePacket<DomainModels.PromoActive.AwardDM>, ResponsePacket<ViewModels.Activity.Award>>().ReverseMap();
            #endregion

            #region Search
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Search.ItemSearchDM, TWNewEgg.Models.ViewModels.Search.ItemSearchVM>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Search.SearchCategoryDM, TWNewEgg.Models.ViewModels.Search.SearchCategoryVM>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Search.SearchResults, TWNewEgg.Models.ViewModels.Search.SearchPageView>().ReverseMap();
            #endregion

            #region Store
            Mapper.CreateMap<DomainModels.Store.Breadcrumbs, ViewModels.Store.Breadcrumbs>().ReverseMap();
            Mapper.CreateMap<DomainModels.Store.BreadcrumbItem, ViewModels.Store.BreadcrumbItem>().ReverseMap();
            #endregion


            #region EDM
            Mapper.CreateMap<DomainModels.EDM.EDMBookDM, ViewModels.EDM.EDMBookVM>().ReverseMap();
            //Mapper.CreateMap<DomainModels.EDM.EDMBookListDM, ViewModels.EDM.EDMBookListVM>().ReverseMap();
            #endregion

            #region Category
            Mapper.CreateMap<DomainModels.Category.AdLayer3DM, ViewModels.Category.CategoryTopBanner_View>().ReverseMap();
            Mapper.CreateMap<DomainModels.Category.AdLayer3ItemDM, ViewModels.Category.CategoryTopBanner_ItemView>().ReverseMap();
            Mapper.CreateMap<DomainModels.Category.CategoryTopItemDM, ViewModels.Category.CategoryTopItemVM>().ReverseMap();
            #endregion

            #region BankBonus
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.BankBonus.BankBonus_DM, TWNewEgg.Models.ViewModels.BankBonus.BankBonus_VM>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.BankBonus.BankBonusTemp_DM, TWNewEgg.Models.ViewModels.BankBonus.BankBonusTemp_VM>();
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<TWNewEgg.Models.DomainModels.BankBonus.BankBonus_DM>>, TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<TWNewEgg.Models.ViewModels.BankBonus.BankBonus_VM>>>();
            #endregion BankBonus

            #region AdditionalItem
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.AdditionalItem.AllAIForCart, ViewModels.Cart.AdditionalItem_View>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.itemDetail.Main.ItemBase.ID))
               .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.itemDetail.Main.ItemBase.CategoryID))
               .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.itemDetail.Main.ItemBase.imgPath))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.itemDetail.Main.ItemBase.Name))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.itemDetail.Price.DisplayPrice))
               .ForMember(dest => dest.SellingQty, opt => opt.MapFrom(src => src.itemDetail.SellingQty))
               .ForMember(dest => dest.ShowOrder, opt => opt.MapFrom(src => src.itemDetail.Main.ItemBase.ShowOrder));
            Mapper.CreateMap<TWNewEgg.Models.DomainModels.AdditionalItem.AllAIForCart, TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>()
               .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.itemDetail.Main.ItemBase.ID))
               .ForMember(dest => dest.SellingQty, opt => opt.MapFrom(src => src.itemDetail.SellingQty));
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems, TWNewEgg.DB.TWSQLDB.Models.ExtModels.ShoppingCartItems>()
               .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.ItemID))
               .ForMember(dest => dest.ItemSellingQty, opt => opt.MapFrom(src => src.ItemSellingQty))
               .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.ItemName))
               .ForMember(dest => dest.ItemShowOrder, opt => opt.MapFrom(src => src.ItemShowOrder)).ReverseMap();
            #endregion

            #region PayType
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.PayType.PayType.ItemPayType, TWNewEgg.Models.DomainModels.CartPayment.ItemPayType>().ReverseMap();

            #endregion
        }
    }

    public class DBtoViewModelMappingRules
    {
        public static void RegisterRules()
        {
            #region datatable
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.ExtModels.CartItems, TWNewEgg.Models.ViewModels.Cart.CartItemBase_View>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ItemID))
                .ForMember(dest => dest.DelvDate, opt => opt.MapFrom(src => src.ItemDelvDate))
                .ForMember(dest => dest.DelvType, opt => opt.MapFrom(src => src.ItemDelvType))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ItemName))
                .ForMember(dest => dest.PriceCash, opt => opt.MapFrom(src => src.ItemPriceCash))
                .ForMember(dest => dest.SellerID, opt => opt.MapFrom(src => src.ItemSellerID))
                .ForAllMembers(opt => opt.Condition(src =>
                    (src.PropertyMap.SourceMember != null && !src.IsSourceValueNull) || (src.SourceType == typeof(string) && src.SourceValue == string.Empty)
                ));
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.ExtModels.CartItems, TWNewEgg.Models.ViewModels.Cart.CartItem_View>().ReverseMap();
            #endregion

            #region PayType
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.PayType, TWNewEgg.Models.ViewModels.Cart.CartPayType_View>();

            #endregion

            #region SalesOrder
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.SalesOrderGroup, TWNewEgg.Models.ViewModels.Cart.SalesOrderGroup_View>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.SalesOrder, TWNewEgg.Models.ViewModels.Cart.SalesOrder_View>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.DB.TWSQLDB.Models.SalesOrderItem, TWNewEgg.Models.ViewModels.Cart.SalesOrderItem_View>().ReverseMap();
            #endregion
        }
    }

    public class ViewtoViewModelMappingRules
    {
        public static void RegisterRules()
        {
            #region Cart
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.Track.TrackItem_View, TWNewEgg.Models.ViewModels.Cart.CartItem_View>()
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.ItemQty))
                .ForMember(dest => dest.NTPrice, opt => opt.MapFrom(src => src.ItemPrice));
            #endregion


            #region Redeem
            Mapper.CreateMap<DomainModels.Redeem.Event, TWNewEgg.DB.TWSQLDB.Models.Event>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.Coupon, TWNewEgg.DB.TWSQLDB.Models.Coupon>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.Coupon.CouponActiveTypeOption, TWNewEgg.DB.TWSQLDB.Models.Coupon.CouponActiveTypeOption>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.Coupon.ValidTypeOption, TWNewEgg.DB.TWSQLDB.Models.Coupon.ValidTypeOption>().ReverseMap();
            Mapper.CreateMap<DomainModels.Redeem.Coupon.CouponUsedStatusOption, TWNewEgg.DB.TWSQLDB.Models.Coupon.CouponUsedStatusOption>().ReverseMap();
            #endregion

            #region PayType
            Mapper.CreateMap<TWNewEgg.Models.ViewModels.PayType.PayType.ItemPayType, TWNewEgg.Models.ViewModels.Item.ItemPayType>().ReverseMap();
            #endregion
        }
    }
}
