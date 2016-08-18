#region 

using Autofac;
using Autofac.Integration.WebApi;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using TWNewEgg.DAL;
using TWNewEgg.DAL.Repository;
using TWNewEgg.DAL.Interface;
using TWNewEgg.DAL.DbContextFactory;
using System.Web.Mvc;
using System.Web.Http;
using System.Configuration;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels;
using TWNewEgg.ExampleFunc;
using TWNewEgg.ExampleFunc.Interface;
using TWNewEgg.AdvService.Service;
using TWNewEgg.CategoryItem;
using TWNewEgg.CategoryItem.Interface;
using TWNewEgg.ItemRepoAdapters;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.PropertyRepoAdapters;
using TWNewEgg.PropertyRepoAdapters.Interface;
using TWNewEgg.PropertyServices;
using TWNewEgg.PropertyServices.Interface;
using TWNewEgg.AccountEnprypt;
using TWNewEgg.AccountEnprypt.Core;
using TWNewEgg.AccountEnprypt.Interface;
using TWNewEgg.CartService;
using TWNewEgg.CartService.Interface;
using TWNewEgg.CategoryService.Service;
using TWNewEgg.CategoryService.Interface;
using TWNewEgg.ItemService.Service;
using TWNewEgg.ItemServices;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.GroupBuy;
using TWNewEgg.GroupBuy.Interface;
using TWNewEgg.GroupBuyServices;
using TWNewEgg.GroupBuyServices.Interface;
using TWNewEgg.AccountRepoAdapters;
using TWNewEgg.AccountRepoAdapters.Interface;
using TWNewEgg.AccountServices;
using TWNewEgg.AccountServices.Interface;
using TWNewEgg.SellerServices;
using TWNewEgg.SellerServices.Interface;
using TWNewEgg.StoreServices;
using TWNewEgg.StoreServices.Interface;
using TWNewEgg.MobileStoreServices;
using TWNewEgg.MobileStoreServices.Interface;
using TWNewEgg.AnswerServices;
using TWNewEgg.AnswerServices.Interface;
using TWNewEgg.NewsMediaServices;
using TWNewEgg.NewsMediaServices.Interface;
using TWNewEgg.SellerRepoAdapters;
using TWNewEgg.SellerRepoAdapters.Interface;
using TWNewEgg.PromotionRepoAdapters.Interface;
using TWNewEgg.PromotionRepoAdapters;
using TWNewEgg.GroupBuyRepoAdapters.Interface;
using TWNewEgg.GroupBuyRepoAdapters;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters;
using TWNewEgg.StoreRepoAdapters;
using TWNewEgg.StoreRepoAdapters.Interface;
using TWNewEgg.MobileStoreRepoAdapters;
using TWNewEgg.MobileStoreRepoAdapters.Interface;
using TWNewEgg.TrackServices;
using TWNewEgg.TrackServices.Interface;
using TWNewEgg.TrackRepoAdapters;
using TWNewEgg.TrackRepoAdapters.Interface;
using TWNewEgg.CategoryRepoAdapters;
using TWNewEgg.CategoryRepoAdapters.Interface;
using TWNewEgg.ActivityRepoAdapters.Interface;
using TWNewEgg.ActivityRepoAdapters;
using TWNewEgg.ActivityServices;
using TWNewEgg.ActivityServices.Interface;
using TWNewEgg.NewsMediaRepoAdapters;
using TWNewEgg.NewsMediaRepoAdapters.Interface;
using TWNewEgg.AnswerAdapters;
using TWNewEgg.AnswerAdapters.Interface;
using TWNewEgg.PromotionServices.Interface;
using TWNewEgg.StarProductRepoAdapters.Interface;
using TWNewEgg.StarProductRepoAdapters;
using TWNewEgg.CartServices.SOServices;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.CartServices.CartMachines;
using TWNewEgg.CartServices.CartStates;
using TWNewEgg.CartServices;
using TWNewEgg.EventRepoAdapters;
using TWNewEgg.EventRepoAdapters.Interface;
using TWNewEgg.ItemSketchRepoAdapters;
using TWNewEgg.ItemSketchRepoAdapters.Interface;
using TWNewEgg.ItemTempRepoAdapters;
using TWNewEgg.ItemTempRepoAdapters.Interface;

using TWNewEgg.StoredProceduresRepoAdapters;
using TWNewEgg.StoredProceduresRepoAdapters.Interface;
using TWNewEgg.ShoppingCartServices;
using TWNewEgg.ShoppingCartServices.Interface;
using TWNewEgg.VotingActivityServices;
using TWNewEgg.VotingActivityServices.Interface;
using TWNewEgg.VotingActivityRepoAdapters;
using TWNewEgg.VotingActivityRepoAdapters.Interface;
using TWNewEgg.EDMRepoAdapters.Interface;
using TWNewEgg.EDMRepoAdapters;
using TWNewEgg.EDMServices;
using TWNewEgg.EDMServices.Interface;
using TWNewEgg.PaymentGatewayAdapter;
using TWNewEgg.PaymentGatewayAdapter.Interface;
using TWNewEgg.CartServices.CartTempServices;
using TWNewEgg.CacheGenerateServices;
using TWNewEgg.CacheGenerateServices.Interface;
using TWNewEgg.Lottery;
using TWNewEgg.ManufactureRepoAdapters;
using TWNewEgg.ManufactureRepoAdapters.Interface;
using TWNewEgg.ManufactureServices;
using TWNewEgg.ManufactureServices.Interface;
using TWNewEgg.HotWordsRepoAdapters;
using TWNewEgg.HotWordsRepoAdapters.Interface;
using TWNewEgg.HotWordsServices;
using TWNewEgg.HotWordsServices.Interface;
using TWNewEgg.SearchServices;
using TWNewEgg.SearchServices.Interface;
using TWNewEgg.CategoryServices;
using TWNewEgg.CategoryServices.Interface;
using TWNewEgg.PageMgmt.Interface;
using TWNewEgg.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.ItemBatchServices.Service;
using TWNewEgg.ItemBatchServices.Interface;
using TWNewEgg.AdditionalItemRepoAdapters;
using TWNewEgg.AdditionalItemRepoAdapters.Interface;
using TWNewEgg.AdditionalItemServices;
using TWNewEgg.AdditionalItemServices.Interface;
using TWNewEgg.BankBonusServices;
using TWNewEgg.BankBonusServices.Interface;
using TWNewEgg.BankBonusRepoAdapters;
using TWNewEgg.BankBonusRepoAdapters.Interface;
using TWNewEgg.BankRepoAdapters;
using TWNewEgg.BankRepoAdapters.Interface;
using TWNewEgg.BankServices;
using TWNewEgg.BankServices.Interface;

using TWNewEgg.Redeem.Service.PromotionGiftExportService;
using TWNewEgg.SendMailServices;
using TWNewEgg.SendMailServices.Interface;
using TWNewEgg.DataMaintainServices;
using TWNewEgg.DataMaintainServices.Interface;
using TWNewEgg.OrderQueueAdapter.Interface;
using TWNewEgg.Framework.FlowEngine;
using salesordergroup.Interface;
using System.Xml.Serialization;
using TWNewEgg.Framework.FlowEngine.Model;
using System.Xml;
using TWNewEgg.SalesOrderRepoAdapters.Interface;
using TWNewEgg.SalesOrderItemRepoAdapters.Interface;
using TWNewEgg.NewEggUSGateway.Interface;
using TWNewEgg.NewEggUSGateway;
using TWNewEgg.BankInstallmentRepoAdapters;
using TWNewEgg.BankInstallmentRepoAdapters.Interface;
using TWNewEgg.ItemInstallmentRepoAdapters;
using TWNewEgg.ItemInstallmentRepoAdapters.Interface;
using TWNewEgg.CartServices.ShoppingCart;
using TWNewEgg.CartServices.CartPayment;
using TWNewegg.DelvTypePaymentTermRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.CartPayment;

//using salesordergroup.Interface;

#endregion

//問候語服務--------------add by bruce 20160329
using TWNewEgg.GreetingWordsServices;
using TWNewEgg.GreetingWordsServices.Interface;
using TWNewEgg.GreetingWordsRepoAdapters;
using TWNewEgg.GreetingWordsRepoAdapters.Interface;

using TWNewEgg.ItemInstallmentRepoAdapters;
using TWNewEgg.BankInstallmentRepoAdapters;
using TWNewEgg.ItemInstallmentRepoAdapters.Interface;
using TWNewEgg.BankInstallmentRepoAdapters.Interface;


// 依據 BSATW-173 廢四機需求增加 // 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160502
using TWNewEgg.Discard4RepoAdapters;
using TWNewEgg.Discard4RepoAdapters.Interface;
using TWNewEgg.Discard4Services;
using TWNewEgg.Discard4Services.Interface;

//依據 BSATW-177 手機改版需求增加---------------add by bruce 20160613
using TWNewEgg.DeviceAdRepoAdapters;
using TWNewEgg.DeviceAdRepoAdapters.Interface;
using TWNewEgg.DeviceAdServices;
using TWNewEgg.DeviceAdServices.Interface;

//依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720
using TWNewEgg.SellerRepoAdapters;
using TWNewEgg.SellerRepoAdapters.Interface;
using TWNewEgg.SellerServices;
using TWNewEgg.SellerServices.Interface;


namespace TWNewEgg.ECService.App_Start
{
    public class AutofacStart
    {
        public static void Bootstrapper()
        {

            #region //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720
            AutofacConfig.builder.RegisterType(typeof(SellerCorrectionPriceService)).As(typeof(ISellerCorrectionPriceService));
            #endregion

            #region //依據 BSATW-177 手機改版需求增加---------------add by bruce 20160613
            AutofacConfig.builder.RegisterType(typeof(DeviceAdSetService)).As(typeof(IDeviceAdSetService));
            AutofacConfig.builder.RegisterType(typeof(DeviceAdContentService)).As(typeof(IDeviceAdContentService));
            #endregion


            #region //依據 BSATW-173 廢四機需求增加 // 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160502
            AutofacConfig.builder.RegisterType(typeof(Discard4Service)).As(typeof(IDiscard4Service));
            AutofacConfig.builder.RegisterType(typeof(Discard4ItemService)).As(typeof(IDiscard4ItemService));
            #endregion


            #region 問候語服務 -------------------------------add by bruce 20160329
            AutofacConfig.builder.RegisterType(typeof(HolidayGreetingWordsService)).As(typeof(IHolidayGreetingWordsService));
            AutofacConfig.builder.RegisterType(typeof(HomeGreetingWordsService)).As(typeof(IHomeGreetingWordsService));            
            #endregion

            #region TWNewEgg.DAL
            AutofacConfig.builder.RegisterGeneric(typeof(TWSqlRepository<>)).As(typeof(IRepository<>));
            AutofacConfig.builder.RegisterGeneric(typeof(TWBackendRepository<>)).As(typeof(IBackendRepository<>));
            #endregion
            
            #region TWNewEgg.ExampleFunc
            AutofacConfig.builder.RegisterType(typeof(TestService2)).As(typeof(ITestService)).As(typeof(ITest3));
            AutofacConfig.builder.RegisterType(typeof(TestService3)).As(typeof(ITest3)).Keyed<ITest3>("TestService3");
            AutofacConfig.builder.RegisterType(typeof(TestService4)).As(typeof(ITest3)).Keyed<ITest3>("TestService4");
            #endregion
            
            #region TWNewEgg.AdvService
            AutofacConfig.builder.RegisterType(typeof(AdvEventTypeReposity)).As(typeof(IAdvEventType));
            AutofacConfig.builder.RegisterType(typeof(AdvEventItemService)).As(typeof(IAdvEventItem));
            AutofacConfig.builder.RegisterType(typeof(AdvEventReposity)).As(typeof(IAdvEvent));
            #endregion

            #region TWNewEgg.CategoryItem
            AutofacConfig.builder.RegisterType(typeof(CategoryItemService)).As(typeof(ICategoryItemService));
            #endregion

            #region TWNewEgg.PropertyServices
            AutofacConfig.builder.RegisterType(typeof(PropertyService)).As(typeof(IPropertyService));
            #endregion

            #region TWNewEgg.AccountEnprypt
            AutofacConfig.builder.RegisterType(typeof(AesService)).As(typeof(IAesService));
            AutofacConfig.builder.RegisterType(typeof(AesOld)).As(typeof(IAes));
            #endregion

            #region TWNewEgg.CartService
            AutofacConfig.builder.RegisterType(typeof(CartsRepository)).As(typeof(ICarts));
            #endregion

            #region TWNewEgg.CartServices
            AutofacConfig.builder.RegisterType(typeof(OPCCartMachineProxy)).As(typeof(ICartMachineProxy));
            AutofacConfig.builder.RegisterType(typeof(SOGroupInfoService)).As(typeof(ISOGroupInfoService));
            AutofacConfig.builder.RegisterType(typeof(OPCCartMachine)).As(typeof(ICartMachine));
            AutofacConfig.builder.RegisterType(typeof(CartInitialState)).As(typeof(ICartState)).Keyed<ICartState>("Cart_Initial");
            AutofacConfig.builder.RegisterType(typeof(CartNotPayedState)).As(typeof(ICartState)).Keyed<ICartState>("Cart_NotPayed");
            AutofacConfig.builder.RegisterType(typeof(CartPayedState)).As(typeof(ICartState)).Keyed<ICartState>("Cart_Payed");
            AutofacConfig.builder.RegisterType(typeof(CartFailedState)).As(typeof(ICartState)).Keyed<ICartState>("Cart_Failed");
            AutofacConfig.builder.RegisterType(typeof(CartCanceledState)).As(typeof(ICartState)).Keyed<ICartState>("Cart_Canceled");
            AutofacConfig.builder.RegisterType(typeof(CartCompletedState)).As(typeof(ICartState)).Keyed<ICartState>("Cart_Completed");
            AutofacConfig.builder.RegisterType(typeof(CartTempService)).As(typeof(ICartTempService));
            AutofacConfig.builder.RegisterType(typeof(AuthService)).As(typeof(IAuthService));
            AutofacConfig.builder.RegisterType(typeof(SalesOrderInfoService)).As(typeof(ISalesOrderInfoService));
            AutofacConfig.builder.RegisterType(typeof(ItemInstallmentService)).As(typeof(IItemInstallmentService));
            AutofacConfig.builder.RegisterType(typeof(NEShoppingCartService)).As(typeof(INEShoppingCartService));
            AutofacConfig.builder.RegisterType(typeof(PaymentTermsGetService)).As(typeof(IPaymentTermsGetService));
            AutofacConfig.builder.RegisterType(typeof(CreditPayOnceGetService)).As(typeof(IPayTypeGetService)).Keyed<IPayTypeGetService>(PaymentTermID.信用卡一次付清);//一次付清
            AutofacConfig.builder.RegisterType(typeof(InstallmentPayTypeGetService)).As(typeof(IPayTypeGetService)).Keyed<IPayTypeGetService>(PaymentTermID.信用卡分期);//分期付款
            AutofacConfig.builder.RegisterType(typeof(WebATMPayTypeGetService)).As(typeof(IPayTypeGetService)).Keyed<IPayTypeGetService>(PaymentTermID.網路ATM);//網路ATM
            AutofacConfig.builder.RegisterType(typeof(ATMPayTypeGetService)).As(typeof(IPayTypeGetService)).Keyed<IPayTypeGetService>(PaymentTermID.實體ATM);//實體ATM
            AutofacConfig.builder.RegisterType(typeof(BankBonusPayGetService)).As(typeof(IPayTypeGetService)).Keyed<IPayTypeGetService>(PaymentTermID.信用卡紅利折抵);//紅利折抵
            AutofacConfig.builder.RegisterType(typeof(CreditPayOnDeliveryGetService)).As(typeof(IPayTypeGetService)).Keyed<IPayTypeGetService>(PaymentTermID.貨到付款);
            AutofacConfig.builder.RegisterType(typeof(ConvenienceStorePayGetService)).As(typeof(IPayTypeGetService)).Keyed<IPayTypeGetService>(PaymentTermID.超商付款);
            AutofacConfig.builder.RegisterType(typeof(StoredPayGetService)).As(typeof(IPayTypeGetService)).Keyed<IPayTypeGetService>(PaymentTermID.儲值支付);
            AutofacConfig.builder.RegisterType(typeof(TelegraphicTransferGetService)).As(typeof(IPayTypeGetService)).Keyed<IPayTypeGetService>(PaymentTermID.電匯);
            AutofacConfig.builder.RegisterType(typeof(NECartPaymentAdapter)).As(typeof(ICartPaymentAdapter));
            AutofacConfig.builder.RegisterType(typeof(NECartPayment)).As(typeof(ICartPayment));
            AutofacConfig.builder.RegisterType(typeof(NENormalCartPayment)).As(typeof(ICartPayment));
            AutofacConfig.builder.RegisterType(typeof(ItemPagePayTypeGetService)).As(typeof(IItemPayTypeGetService));
            
            AutofacConfig.builder.RegisterType(typeof(PayTypeService)).As(typeof(TWNewEgg.CartServices.Interface.IPayTypeService));
            #endregion

            #region TWNewEgg.CategoryService
            AutofacConfig.builder.RegisterType(typeof(CategoryApiService)).As(typeof(ICategoryService));
            AutofacConfig.builder.RegisterType(typeof(CategoryNewServices)).As(typeof(ICategoryServices));
            AutofacConfig.builder.RegisterType(typeof(AdLayer3Services)).As(typeof(IAdLayer3Services));
            AutofacConfig.builder.RegisterType(typeof(CategoryTopItemService)).As(typeof(ICategoryTopItemService));
            #endregion

            #region TWNewEgg.ItemService
            AutofacConfig.builder.RegisterType(typeof(ItemPriceRepository)).As(typeof(IItemPrice));
            #endregion

            #region TWNewEgg.ItemServices
            AutofacConfig.builder.RegisterType(typeof(ItemDetailService)).As(typeof(IItemDetailService));
            AutofacConfig.builder.RegisterType(typeof(ItemDisplayPriceService)).As(typeof(IItemDisplayPriceService));
            AutofacConfig.builder.RegisterType(typeof(ItemInfoService)).As(typeof(IItemInfoService));
            AutofacConfig.builder.RegisterType(typeof(ItemGroupService)).As(typeof(IItemGroupService));
            AutofacConfig.builder.RegisterType(typeof(ItemStockService)).As(typeof(IItemStockService));
            AutofacConfig.builder.RegisterType(typeof(ShelveService)).As(typeof(IShelveService));
            #endregion

            #region TWNewEgg.GroupByServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.GroupBuyServices.GroupBuyService)).As(typeof(IGroupBuyService));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.GroupBuyServices.GetOldGroupBuy)).As(typeof(IGetOldGroupBuyServices));
            #endregion

            #region TWNewEgg.GroupBy
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.GroupBuy.GroupBuyService)).As(typeof(IGroupBuy));
            #endregion

            #region TWNewEgg.AccountServices
            AutofacConfig.builder.RegisterType(typeof(AccountService)).As(typeof(IAccountService));
            AutofacConfig.builder.RegisterType(typeof(GetMemberService)).As(typeof(IGetMemberService));
            #endregion

            #region TWNewEgg.DataMaintainServices
            AutofacConfig.builder.RegisterType(typeof(DataMaintainService)).As(typeof(IDataMaintainService));
            #endregion           

            #region TWNewEgg.PromotionServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.PromotionServices.PromotionService)).As(typeof(IPromotionService));
            #endregion

            #region TWNewEgg.SellerServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SellerServices.SellerServices)).As(typeof(ISellerServices));
            #endregion

            #region RedeemService
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Redeem.Service.CouponService.CouponServiceRepository)).As(typeof(TWNewEgg.Redeem.Service.CouponService.ICouponService));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Redeem.Service.CouponService.EventReponsitory)).As(typeof(TWNewEgg.Redeem.Service.CouponService.IEvent));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Redeem.Service.CouponService.EventFileRepository)).As(typeof(TWNewEgg.Redeem.Service.CouponService.IEventFile));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRepository)).As(typeof(TWNewEgg.Redeem.Service.PromotionGiftService.IPromotionGiftService));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Redeem.Service.PromotionGiftService.PromotionGiftRecordRepository)).As(typeof(TWNewEgg.Redeem.Service.PromotionGiftService.IPromotionGiftRecordService));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.Redeem.Service.PromotionGiftExportService.PromotionGiftImportCheckDataService)).As(typeof(TWNewEgg.Redeem.Service.PromotionGiftExportService.IPromotionGiftImportCheckDataService));
            #endregion

            #region TWNewEgg.StoreServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.StoreServices.StoreService)).As(typeof(IStoreService));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.StoreServices.HomeStoreService)).As(typeof(IHomeStoreService));
            #endregion

            #region TWNewEgg.TrackServices
            AutofacConfig.builder.RegisterType(typeof(TrackService)).As(typeof(ITrackService));
            #endregion

            #region TWNewEgg.ItemImageUrlService
            AutofacConfig.builder.RegisterType(typeof(ItemImageUrlService)).As(typeof(IItemImageUrlService));
            #endregion

            #region TWNewEgg.ActivityServices
            AutofacConfig.builder.RegisterType(typeof(ActivityService)).As(typeof(IActivityService));
            #endregion


            AutofacConfig.builder.RegisterType(typeof(NeweggUSAAPI)).As(typeof(INeweggUSAAPI));

            #region TWNewEgg.NewsMediaServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.NewsMediaServices.NewsService)).As(typeof(INewsService));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.NewsMediaServices.MediaService)).As(typeof(IMediaService));
            #endregion

            #region TWNewEgg.AnswerServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.AnswerServices.AnswerService)).As(typeof(IAnswerService));
       
            #endregion

            #region TWNewEgg.MobileStoreServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.MobileStoreServices.MobileStoreService)).As(typeof(IMobileStoreService));
            #endregion

            #region TWNewEgg.PaymentGatewayAdapter
            AutofacConfig.builder.RegisterType(typeof(HiTrustProxy)).As(typeof(IHiTrustProxy));
            AutofacConfig.builder.RegisterType(typeof(AllPayProxy)).As(typeof(IAllPayProxy));
            AutofacConfig.builder.RegisterType(typeof(NCCCProxy)).As(typeof(INCCCProxy));
            #endregion

            #region TWNewEgg.ShoppingCartService
            AutofacConfig.builder.RegisterType(typeof(ShoppingCartService)).As(typeof(IShoppingCartService));
            #endregion

            #region TWNewEgg.VotingActivityServices
            AutofacConfig.builder.RegisterType(typeof(VotingActivityService)).As(typeof(IVotingActivityServices));
            AutofacConfig.builder.RegisterType(typeof(VotingBusinessService)).As(typeof(IVotingBusinessServices));
            #endregion

            #region TWNewEgg.EDMServices
            AutofacConfig.builder.RegisterType(typeof(EDMService)).As(typeof(IEDMService));
            #endregion

            #region TWNewEgg.CacheGenerateServices
            AutofacConfig.builder.RegisterType(typeof(XMLGenerate)).As(typeof(IXMLGenerate));
            #endregion

            #region TWNewEgg.PageMgmt
            AutofacConfig.builder.RegisterType(typeof(PageMgmtAdapter)).As(typeof(IPageMgmtAdapter));
            AutofacConfig.builder.RegisterGeneric(typeof(ComponentService<>)).As(typeof(IComponentService<>));
            AutofacConfig.builder.RegisterType(typeof(ComponentDBUtil)).As(typeof(IComponentDBUtil));
            AutofacConfig.builder.RegisterType(typeof(DynamicObjectService)).As(typeof(IObjectService<DynamicObject>));
            AutofacConfig.builder.RegisterType(typeof(ImageObjectService)).As(typeof(IObjectService<ImageObject>));
            AutofacConfig.builder.RegisterType(typeof(PageDBUtil)).As(typeof(IPageDBUtil));
            AutofacConfig.builder.RegisterType(typeof(TextObjectService)).As(typeof(IObjectService<TextObject>));
            AutofacConfig.builder.RegisterType(typeof(VideoObjectService)).As(typeof(IObjectService<VideoInfo>));
            #endregion
            #region TWNewegg.Lottery
            AutofacConfig.builder.RegisterType(typeof(LotteryService)).As(typeof(ILotteryService));
            AutofacConfig.builder.RegisterType(typeof(LotteryFactory)).As(typeof(ILotteryFactory));
            AutofacConfig.builder.RegisterType(typeof(Type1Lottery)).Keyed<ILottery>("type1");
            #endregion

            #region TWNewegg.StoreRepoAdapter
            AutofacConfig.builder.RegisterType(typeof(StoreService)).As(typeof(IStoreService));
            AutofacConfig.builder.RegisterType(typeof(StoreRepoAdapter)).As(typeof(IStoreRepoAdapter));
            #endregion

            #region TWNewegg.SearchServices
            AutofacConfig.builder.RegisterType(typeof(SolrService)).As(typeof(ISearchService)).Keyed<ISearchService>("SolrService");
            #endregion
            
            #region TWNewegg.HotWords
            AutofacConfig.builder.RegisterType(typeof(HotWordsService)).As(typeof(IHotWordsService));
            AutofacConfig.builder.RegisterType(typeof(HotWordsReopAdapter)).As(typeof(IHotWordsReopAdapter));
            #endregion

            #region TWNewEgg.EventRepoAdapter
            AutofacConfig.builder.RegisterType(typeof(EventRepoAdapter)).As(typeof(IEventRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(CouponRepoAdapter)).As(typeof(ICouponRepoAdapter));
            #endregion

            #region TWNewegg.SearchServices
            AutofacConfig.builder.RegisterType(typeof(SolrService)).As(typeof(ISearchService)).Keyed<ISearchService>("SolrService");
            #endregion
            #region TWNewEgg.ItemBatchServices
            AutofacConfig.builder.RegisterType(typeof(USAImageService)).As(typeof(IUSAImageService));
            AutofacConfig.builder.RegisterType(typeof(USAUpdateItemStockService)).As(typeof(IUSAUpdateItemStockService));
            AutofacConfig.builder.RegisterType(typeof(USAUpdateItemPriceService)).As(typeof(IUSAUpdateItemPriceService));
            AutofacConfig.builder.RegisterType(typeof(USAUpdateItemDescService)).As(typeof(IUSAUpdateItemDescService));
            AutofacConfig.builder.RegisterType(typeof(USABatchService)).As(typeof(IUSABatchService));
            #endregion

            #region TWNewEgg.AdditionalItemServices
            AutofacConfig.builder.RegisterType(typeof(AIForCartService)).As(typeof(IAIForCart));
            AutofacConfig.builder.RegisterType(typeof(AIForItemService)).As(typeof(IAIForItem));
            AutofacConfig.builder.RegisterType(typeof(AdditionItemSearchService)).As(typeof(IAdditionalSearch));
            AutofacConfig.builder.RegisterType(typeof(SetAdditionItemService)).As(typeof(ISetAdditionalItem));
            #endregion
            #region TWNewEgg.BankBonusServices
            AutofacConfig.builder.RegisterType(typeof(BankBonusService)).As(typeof(IBankBonusService));
            #endregion

            #region TWNewEgg.PromoActiveService
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.PromoActiveService.PromoActiveServices)).As(typeof(TWNewEgg.PromoActiveService.Interface.IPromoActiveService));
            #endregion
            #region TWNewEgg.PromoAwardLogService
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.PromoAwardLogService.PromoAwardLogService)).As(typeof(TWNewEgg.PromoAwardLogService.Interface.IPromoAwardLogService));
            #endregion
            #region TWNewEgg.BankServices
            AutofacConfig.builder.RegisterType(typeof(BankService)).As(typeof(IBankService));
            #endregion
            #region TWNewEgg.SendMailServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SendMailServices.SendMailServices)).As(typeof(ISendMailServices));
            #endregion

            #region TWNewEgg.PurgeService
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.StorageServices.AzureCDNAdapter)).As(typeof(TWNewEgg.StorageServices.Interface.ICDNAdapter));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.PurgeService.PurgeService)).As(typeof(TWNewEgg.PurgeService.Interface.IPurgeService));
            #endregion

            #region TWNewEgg.NeweggUSARequestServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.NeweggUSARequestServices.Services.NeweggRequest)).As(typeof(TWNewEgg.NeweggUSARequestServices.Interface.INeweggRequest));
            #endregion

            #region TWNewEgg.CartLocalServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.CartLocalServices.CartLocalServices)).As(typeof(TWNewEgg.CartLocalServices.Interface.ICartLocalServices));
            #endregion


            #region TWNewEgg.FinanceServices
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.FinanceServices.CompanyFinanceDataService)).As(typeof(TWNewEgg.FinanceServices.Interface.ICompanyFinanceDataService));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.FinanceServices.FinanceDocumentService)).As(typeof(TWNewEgg.FinanceServices.Interface.IFinanceDocumentService));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.FinanceServices.SapDocumentService)).As(typeof(TWNewEgg.FinanceServices.Interface.ISapDocumentService));
            #endregion

            AutofacStart.RegisterRepoAdapters();
            AutofacConfig.builder.RegisterFilterProvider();
            AutofacConfig.builder.RegisterControllers(Assembly.GetExecutingAssembly());
            #region Register DbContext
            string twSqlConnectStr = ConfigurationManager.ConnectionStrings["TWSqlDBConnection"].ConnectionString;
            if (twSqlConnectStr != null)
            {
                AutofacConfig.builder.RegisterType(typeof(DbContextFactory<TWSqlDBContext>))
                    .WithParameter("connectionString", twSqlConnectStr)
                    .As(typeof(IDbContextFactory<TWSqlDBContext>))
                    .InstancePerLifetimeScope();
            }

            string twBackendConnectStr = ConfigurationManager.ConnectionStrings["TWBackendDBConnection"].ConnectionString;
            if (twBackendConnectStr != null)
            {
                AutofacConfig.builder.RegisterType(typeof(DbContextFactory<TWBackendDBContext>))
                    .WithParameter("connectionString", twBackendConnectStr)
                    .As(typeof(IDbContextFactory<TWBackendDBContext>))
                    .InstancePerLifetimeScope();
            }
            #endregion
 
            AutofacConfig.builder.RegisterControllers(Assembly.GetExecutingAssembly());
            AutofacConfig.SetAutofacContainer(AutofacConfig.builder.Build());
            DependencyResolver.SetResolver(new AutofacDependencyResolver(AutofacConfig.GetAutofacComtainer()));
           
        }

        private static void RegisterRepoAdapters()
        {

            #region //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720
            AutofacConfig.builder.RegisterType(typeof(SellerCorrectionPriceRepoAdapter)).As(typeof(ISellerCorrectionPriceRepoAdapter));
            #endregion


            #region //依據 BSATW-177 手機改版需求增加---------------add by bruce 20160613
            AutofacConfig.builder.RegisterType(typeof(DeviceAdSetRepoAdapter)).As(typeof(IDeviceAdSetRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(DeviceAdContentRepoAdapter)).As(typeof(IDeviceAdContentRepoAdapter));
            #endregion

            #region //依據 BSATW-173 廢四機需求增加 // 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160502
            AutofacConfig.builder.RegisterType(typeof(Discard4RepoAdapter)).As(typeof(IDiscard4RepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(Discard4ItemRepoAdapter)).As(typeof(IDiscard4ItemRepoAdapter));
            #endregion

        
            #region 問候語服務 add by bruce 20160329            
            AutofacConfig.builder.RegisterType(typeof(GreetingWordsRepoAdapter)).As(typeof(IGreetingWordsRepoAdapter));
            #endregion

         
            AutofacConfig.builder.RegisterType(typeof(AccountRepoAdapter)).As(typeof(IAccountRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(DbItemInfoRepoAdapter)).As(typeof(IDbItemInfoRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ItemRepoAdapter)).As(typeof(IItemRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(PropertyRepoAdapter)).As(typeof(IPropertyRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ProductRepoAdapter)).As(typeof(IProductRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(SellerRepoAdapter)).As(typeof(ISellerRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ItemStockRepoAdapter)).As(typeof(IItemStockRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(CartTempRepoAdapter)).As(typeof(ICartTempRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(CartRepoAdapter)).As(typeof(ICartRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(GroupBuyRepoAdapter)).As(typeof(IGroupBuyRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(PromotionRepoAdapter)).As(typeof(IPromotionRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(PromotionRecordsRepoAdapter)).As(typeof(IPromotionRecordsRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(StoreRepoAdapter)).As(typeof(IStoreRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(TrackRepoAdapter)).As(typeof(ITrackRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ImageUrlReferenceRepoAdapter)).As(typeof(IImageUrlReferenceRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(CategoryRepoAdapter)).As(typeof(ICategoryRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(DBSOInfoRepoAdapter)).As(typeof(IDBSOInfoRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(SORepoAdapter)).As(typeof(ISORepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ActivityRepoAdapter)).As(typeof(IActivityRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(MemberRepoAdapter)).As(typeof(IMemberRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(NewsMediaRepoAdapter)).As(typeof(INewsMediaRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(AnswerAdapter)).As(typeof(IAnswerAdapter));
            AutofacConfig.builder.RegisterType(typeof(MobileStoreRepoAdapter)).As(typeof(IMobileStoreRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(StarProductRepoAdapter)).As(typeof(IStarProductRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ItemDisplayPriceRepoAdapter)).As(typeof(IItemDisplayPriceRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(StoredProceduresRepoAdapter)).As(typeof(IStoredProceduresRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(EventRepoAdapter)).As(typeof(IEventRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(PayTypeRepoAdapter)).As(typeof(IPayTypeRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(PaymentTermRepoAdapter)).As(typeof(IPaymentTermRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(EDMRepoAdapter)).As(typeof(IEDMRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(VotingActivityRepo)).As(typeof(IVotingActivity));
            AutofacConfig.builder.RegisterType(typeof(VotingActivityRecRepo)).As(typeof(IVotingActivityRec));
            AutofacConfig.builder.RegisterType(typeof(ItemGroupRepoAdapter)).As(typeof(IItemGroupRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ManufactureRepoAdapter)).As(typeof(IManufactureRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ManufactureService)).As(typeof(IManufactureService));
            AutofacConfig.builder.RegisterType(typeof(HotWordsReopAdapter)).As(typeof(IHotWordsReopAdapter));
            AutofacConfig.builder.RegisterType(typeof(HotWordsService)).As(typeof(IHotWordsService));
            AutofacConfig.builder.RegisterType(typeof(PurchaseOrderRepoAdapter)).As(typeof(IPurchaseOrderRepoAdapter));  
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.PromoActiveRepoAdapters.PromoActiveRepoAdapter)).As(typeof(TWNewEgg.PromoActiveRepoAdapters.Interface.IPromoActiveRepoAdapters));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.PromoAwardLogRepoAdapters.PromoAwardLogRepoAdapters)).As(typeof(TWNewEgg.PromoAwardLogRepoAdapters.Interface.IPromoAwardLogRepoAdapters));

            
            AutofacConfig.builder.RegisterType(typeof(CategoryApiService)).As(typeof(ICategoryService));
            AutofacConfig.builder.RegisterType(typeof(CategoryRepoAdapter)).As(typeof(ICategoryRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(AdLayer3RepoAdapter)).As(typeof(IAdLayer3RepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(CategoryTopItemRepoAdapter)).As(typeof(ICategoryTopItemRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ImageUrlReferenceForProductRepoAdapter)).As(typeof(IImageUrlReferenceForProductRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ItemStockRepoAdapter)).As(typeof(IItemStockRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(StoreService)).As(typeof(IStoreService));
            AutofacConfig.builder.RegisterType(typeof(StoreRepoAdapter)).As(typeof(IStoreRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(AIForCartRepoAdapter)).As(typeof(IAIForCartRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ItemTempRepoAdapter)).As(typeof(IItemTempRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ItemSketchRepoAdapter)).As(typeof(IItemSketchRepoAdapter));

            AutofacConfig.builder.RegisterType(typeof(BankBonusRepoAdapter)).As(typeof(IBankBonusRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(BankRepoAdapter)).As(typeof(IBankRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(AuthRepoAdapter)).As(typeof(IAuthRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(RetgoodRepoAdapter)).As(typeof(IRetgoodRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(Refund2cRepoAdapter)).As(typeof(IRefund2cRepoAdapter));   
            AutofacConfig.builder.RegisterType(typeof(Datamaintain_logRepoAdapter)).As(typeof(IDatamaintain_logRepoAdapter));

            AutofacConfig.builder.RegisterType(typeof(ItemInstallmentRuleRepoAdapter)).As(typeof(IItemInstallmentRuleRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(ItemTopInstallmentRepoAdapter)).As(typeof(IItemTopInstallmentRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(BankInstallmentRepoAdapter)).As(typeof(IBankInstallmentRepoAdapter));

            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.OrderQueueAdapter.OrderQueueAdapter)).As(typeof(IOrderQueueAdapter));

            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.StorageServices.AzureChangeContentType)).As(typeof(TWNewEgg.StorageServices.Interface.IAure));

            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SalesOrderGrouprRepoAdapter.SalesordergroupRepoAdapter)).As(typeof(ISalesOrderGroupRepoAdapters)).Keyed<ISalesOrderGroupRepoAdapters>("Formal"); ;
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SalesOrderRepoAdapters.SalesOrderRepoAdapters)).As(typeof(ISalesOrderRepoAdapters)).Keyed<ISalesOrderRepoAdapters>("Formal"); ;
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SalesOrderItemRepoAdapters.SalesOrderItemRepoAdapters)).As(typeof(ISalesOrderItemRepoAdapters)).Keyed<ISalesOrderItemRepoAdapters>("Formal"); ;
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SalesOrderGrouprRepoAdapter.SalesordergroupTempRepoAdapter)).As(typeof(ISalesOrderGroupRepoAdapters)).Keyed<ISalesOrderGroupRepoAdapters>("Temp"); ;
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SalesOrderRepoAdapters.SalesOrderTempRepoAdapter)).As(typeof(ISalesOrderRepoAdapters)).Keyed<ISalesOrderRepoAdapters>("Temp"); ;
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SalesOrderItemRepoAdapters.SalesOrderItemTempRepoAdapters)).As(typeof(ISalesOrderItemRepoAdapters)).Keyed<ISalesOrderItemRepoAdapters>("Temp"); ;

            AutofacConfig.builder.RegisterType(typeof(TWNewegg.DelvTypePaymentTermRepoAdapters.DelvTypePaymentTermRepoAdapter)).As(typeof(IDelvTypePaymentTermRepoAdapter));

            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.ECService.Controllers.SampleController.StaticFlowService)).As(typeof(IFlowService));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.StorageServices.AzureCDNAdapter)).As(typeof(TWNewEgg.StorageServices.Interface.ICDNAdapter));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.PurgeQueueAdapters.PurgeQueueAdapters)).As(typeof(TWNewEgg.PurgeQueueAdapters.Interface.IPurgeQueueAdapters));
            // FlowEngine Test
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.ECService.Controllers.SampleController.FlowStep1)).As(typeof(IFlowStep)).Keyed<IFlowStep>("TestFlow1");
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.ECService.Controllers.SampleController.FlowStep2)).As(typeof(IFlowStep)).Keyed<IFlowStep>("TestFlow2");
            // end of FlowEngine Test

            #region MySQL Adapter
            AutofacConfig.builder.RegisterType(typeof(MySQLSQLCommandRepository)).As(typeof(IMySQLSQLCommandRepository));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SalesOrderGroupLocalRepoAdapters.SalesOrderGroupLocalRepoAdapters)).As(typeof(TWNewEgg.SalesOrderGroupLocalRepoAdapters.Interface.ISalesOrderGroupLocalRepoAdapters));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SalesOrderLocalRepoAdapters.SalesOrderLocalRepoAdapters)).As(typeof(TWNewEgg.SalesOrderLocalRepoAdapters.Interface.ISalesOrderLocalRepoAdapters));
            AutofacConfig.builder.RegisterType(typeof(TWNewEgg.SalesOrderItemLocalRepoAdapters.SalesOrderItemLocalRepoAdapters)).As(typeof(TWNewEgg.SalesOrderItemLocalRepoAdapters.Interface.ISalesOrderItemLocalRepoAdapters));  
            #endregion

            #region TWNewEgg.FinanceRepoAdapters
            AutofacConfig.builder.RegisterType(typeof(FinanceRepoAdapters.AccountsProfileRepoAdapter)).As(typeof(FinanceRepoAdapters.Interface.IAccountsProfileRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(FinanceRepoAdapters.FinanceRepoAdapter)).As(typeof(FinanceRepoAdapters.Interface.IFinanceRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(FinanceRepoAdapters.SapBapiAccDocumentRepoAdapter)).As(typeof(FinanceRepoAdapters.Interface.ISapBapiAccDocumentRepoAdapter));
            AutofacConfig.builder.RegisterType(typeof(FinanceRepoAdapters.SellerFinanceRepoAdapter)).As(typeof(FinanceRepoAdapters.Interface.ISellerFinanceRepoAdapter));
            #endregion
            AutofacConfig.builder.RegisterType(typeof(BeneficiaryPartyAdapter)).As(typeof(IBeneficiaryPartyAdapter));
     
        }
    }
}