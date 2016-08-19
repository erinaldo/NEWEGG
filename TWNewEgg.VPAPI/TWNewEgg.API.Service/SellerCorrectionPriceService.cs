using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using TWNewEgg.Models.DomainModels;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.API.Service
{
    // BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160721
    public class SellerCorrectionPriceService
    {
        // 記錄訊息
        //private static ILog log = LogManager.GetLogger(typeof(Discard4Service));
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        /// <summary>
        /// GetGroupBy
        /// </summary>
        /// <param name="finanStatus"></param>
        /// <param name="sellerID"></param>
        /// <param name="settlementID"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public static List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> GetGroupBy(string finanStatus, int sellerID, string settlementID, string user_name)
        {
            List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> list_info = new List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>();
            try
            {

                TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM search_info = new TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM();
                //search_info.SellerIDs.Add(407);
                search_info.SellerIDs.Add(sellerID);
                //search_info.FinanStatus = "V"; //對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票                           
                //search_info.SettlementIDs.Clear();
                search_info.SettlementIDs.Add(settlementID);
                //傳入狀態-----------------------------------------add by bruce 20160729
                search_info.FinanStatus = finanStatus;

                ////先找調整項有沒有押過對帳單號-----------add by bruce 20160726
                //var list_result3 = Processor.Request<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>
                //    , List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>>("SellerCorrectionPriceService", "GetGroupBy", search_info);
                //if (list_result3.results.Count == 0)
                //{
                //    search_info.FinanStatus = "I"; //對帳單調整項狀態：I=已匯入, V=Vendor押發票時變動;V=已開發票 
                //    search_info.SettlementIDs.Clear();
                //}

                //var list_result = Processor.Request<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>
                //    , List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>>("SellerCorrectionPriceService", "GetGroupBy", search_info);

                ////有可能是已存檔過,未取到就取有對帳單號碼的
                //if (list_result.results.Count == 0)
                //{
                //    search_info.FinanStatus = "V";
                //    search_info.SettlementIDs.Clear();
                //    search_info.SettlementIDs.Add(settlementID);
                //    list_result = Processor.Request<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>
                //    , List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>>("SellerCorrectionPriceService", "GetGroupBy", search_info);
                //}

                var list_result = Processor.Request<List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>
                        , List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM>>("SellerCorrectionPriceService", "GetGroupBy", search_info);

                list_info = list_result.results;

            }
            catch (Exception ex)
            {
                log.Error("預覽 Msg: " + ex.Message + ", Stacktrace: " + ex.StackTrace);
            }
            return list_info;
        }

        /// <summary>
        /// Save1
        /// </summary>
        /// <param name="finanStatus"></param>
        /// <param name="sellerID"></param>
        /// <param name="settlementID"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public static List<bool> Save1(string finanStatus, int sellerID, string settlementID, string user_name)
        {
            List<bool> list_result = new List<bool>();
            try
            {

                TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM search_info = new TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM();
                search_info.SettlementIDs.Add(settlementID);
                search_info.SellerIDs.Add(sellerID);
                search_info.FinanStatus = finanStatus;
                //var result = Processor.Request<List<bool>, List<bool>>("SellerCorrectionPriceService", "Save1", sellerID,settlementID, user_name);
                //改為以model方式-----------------------add by bruce 20160729
                var result = Processor.Request<List<bool>, List<bool>>("SellerCorrectionPriceService", "Save1", search_info, user_name);

                if (result.results != null)
                    list_result = result.results;

            }
            catch (Exception ex)
            {
                log.Error("預覽 Msg: " + ex.Message + ", Stacktrace: " + ex.StackTrace);
            }
            return list_result;
        }


    }

}