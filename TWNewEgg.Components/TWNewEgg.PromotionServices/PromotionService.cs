using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PromotionServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemRepoAdapters;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.PromotionRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.PromotionServices
{
    public class PromotionService : IPromotionService
    {
        private IPromotionRepoAdapter _promotionRepoAdapter;
        private IItemRepoAdapter _itemRepoAdapter;
        // 不參與優惠活動的Item.ShowOrder狀態清單
        private List<int> notToParticipateList = new List<int> { (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart };

        public PromotionService(IPromotionRepoAdapter promotionRepoAdapter, IItemRepoAdapter itemRepoAdapter)
        {
            this._promotionRepoAdapter = promotionRepoAdapter;
            this._itemRepoAdapter = itemRepoAdapter;
        }

        /// <summary>
        /// 透過賣場ID取得該賣場可享有的折價活動清單與優惠折扣級距清單
        /// </summary>
        /// <param name="itemID">查詢的賣場id</param>
        /// <returns>返回查詢結果(不論活動是否衝突，所有可參與的活動皆顯示)</returns>
        public ResponseMessage<DbPromotionInfo> HasOverPurchaseDiscount(int itemID, string turnOn)
        {
            //ResponseMessage<DbPromotionInfo> result = new ResponseMessage<DbPromotionInfo>();
            turnOn = turnOn.ToLower();
            // 返回的結果資訊
            DbPromotionInfo result = new DbPromotionInfo();
            // 滿足條件下所有優惠折扣級距清單
            List<PromotionGiftInterval> queryPromotionGiftInterval = null;
            // 黑名單清單
            List<PromotionGiftBlackList> queryPromotionGiftBlackList = null;
            // 白名單清單
            List<PromotionGiftWhiteList> queryPromotionGiftWhiteList = null;
            // 所有需要查詢的PromotionGiftBasic.ID
            List<int> allQueryPromotionGiftBasicID = new List<int>();
            Item queryItem = this._itemRepoAdapter.GetIfAvailable(itemID);
            if (queryItem == null)
            {
                return ReturnMessage(false, result, "查無此賣場資訊");
            }

            // 找出不得參與活動的Item
            //if (IsNotToParticipateItem(queryItem))
            //{
            //    return ReturnMessage(false, result, "此商品不得參與優惠活動");
            //}
            // 找出所有在活動時間內且符合條件的優惠折扣基本資訊清單
            //List<PromotionGiftBasic> queryPromotionGiftBasic = this._promotionRepoAdapter.GetPromotionGiftBasic(queryItem.CategoryID).ToList();
            List<PromotionGiftBasic> queryPromotionGiftBasicByDate = null;
            if (turnOn == "on")
            {
                queryPromotionGiftBasicByDate = this._promotionRepoAdapter.GetPromotionGiftBasicByDate(PromotionGiftBasic.UsedStatus.Used).ToList();
            }
            else
            {
                queryPromotionGiftBasicByDate = this._promotionRepoAdapter.GetPromotionGiftBasicByDate(PromotionGiftBasic.UsedStatus.TempUsed).ToList();
            }
            if (queryPromotionGiftBasicByDate == null || queryPromotionGiftBasicByDate.Count == 0)
            {
                return ReturnMessage(false, result, "查無此賣場優惠活動資訊");
            }
            // 使用CategoryID篩選出的優惠折扣活動
            string cidString = ";" + queryItem.CategoryID.ToString() + ";";
            // 使用黑名單清單且以CategoryID篩選出來的活動清單(只有黑名單使用CategoryID篩選，白名單不看CategoryID而是直接使用白名單清單中的itemID為依據)
            List<PromotionGiftBasic> queryPromotionGiftBasicCheckBlack = queryPromotionGiftBasicByDate.Where(x => 
                ((";" + x.Categories + ";").Contains(cidString) || (";" + x.Categories + ";").Contains(";0;")) 
                && x.ReferencesList == "black").ToList();
            // 使用黑名單的PromotionGiftBasic.ID清單
            List<int> blackPromotionGiftBasicID = queryPromotionGiftBasicCheckBlack.Select(x => x.ID).ToList();
            // 使用白名單的活動清單
            List<PromotionGiftBasic> queryPromotionGiftBasicCheckWhite = queryPromotionGiftBasicByDate.Where(x => x.ReferencesList == "white").ToList();
            // 使用白名單PromotionGiftBasic.ID清單
            List<int> whitePromotionGiftBasicID = queryPromotionGiftBasicCheckWhite.Select(x => x.ID).ToList();
            // 所有需要查詢的PromotionGiftBasic.ID
            allQueryPromotionGiftBasicID.AddRange(blackPromotionGiftBasicID);
            allQueryPromotionGiftBasicID.AddRange(whitePromotionGiftBasicID);
            // 滿足條件下所有優惠折扣級距清單
            queryPromotionGiftInterval = this._promotionRepoAdapter.GetPromotionGiftInterval(allQueryPromotionGiftBasicID).ToList();
            if (turnOn == "on")
            {
                // 黑名單清單
                queryPromotionGiftBlackList = this._promotionRepoAdapter.GetPromotionGiftBlackList(blackPromotionGiftBasicID).Where(x => x.Status == (int)TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.Used).ToList();
                // 白名單清單
                queryPromotionGiftWhiteList = this._promotionRepoAdapter.GetPromotionGiftWhiteList(whitePromotionGiftBasicID).Where(x => x.Status == (int)TWNewEgg.Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus.Used).ToList();
            }
            else
            {
                // 黑名單清單
                queryPromotionGiftBlackList = this._promotionRepoAdapter.GetPromotionGiftBlackList(blackPromotionGiftBasicID).Where(x => x.Status == (int)TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.TempUsed).ToList();
                // 白名單清單
                queryPromotionGiftWhiteList = this._promotionRepoAdapter.GetPromotionGiftWhiteList(whitePromotionGiftBasicID).Where(x => x.Status == (int)TWNewEgg.Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus.TempUsed).ToList();
            }
            // 不在篩選用CategoryID外的白名單清單資格檢查
            queryPromotionGiftBasicCheckWhite.ForEach(subPB =>
            {
                List<PromotionGiftWhiteList> searchWhitList = null;
                if (turnOn == "on")
                {
                    searchWhitList = queryPromotionGiftWhiteList.Where(x =>
                        x.PromotionGiftBasicID == subPB.ID
                        && x.ItemID == itemID
                        && x.Status == (int)PromotionGiftWhiteList.WhiteListStatus.Used).ToList();
                }
                else
                {
                    searchWhitList = queryPromotionGiftWhiteList.Where(x =>
                        x.PromotionGiftBasicID == subPB.ID
                        && x.ItemID == itemID
                        && x.Status == (int)PromotionGiftWhiteList.WhiteListStatus.TempUsed).ToList();
                }
                if (searchWhitList == null || searchWhitList.Count > 0)
                {
                    InsertPromotionInfo(ref result, subPB, queryPromotionGiftInterval);
                }
            });
            // 透過Category篩選出的優惠折扣活動資格檢查
            queryPromotionGiftBasicCheckBlack.ForEach(subPB =>
            {
                // 若該活動參考依據為黑名單則需檢查該賣場ID是否在活動的黑名單中
                List<PromotionGiftBlackList> searchBlackList = null;
                if (turnOn == "on")
                {
                    searchBlackList = queryPromotionGiftBlackList.Where(x =>
                        x.PromotionGiftBasicID == subPB.ID
                        && x.ItemID == itemID
                        && x.Status == (int)PromotionGiftBlackList.BlackListStatus.Used).ToList();
                }
                else
                {
                    searchBlackList = queryPromotionGiftBlackList.Where(x =>
                        x.PromotionGiftBasicID == subPB.ID
                        && x.ItemID == itemID
                        && x.Status == (int)PromotionGiftBlackList.BlackListStatus.TempUsed).ToList();
                }
                // 若該賣場ID不在該優惠活動的黑名單中，則加入返回結果
                if (searchBlackList == null || searchBlackList.Count == 0)
                {
                    InsertPromotionInfo(ref result, subPB, queryPromotionGiftInterval);
                }
            });

            return ReturnMessage(true, result, "優惠活動查詢成功");
        }
        
        /// <summary>
        /// 將符合條件的活動清單與優惠折扣級距清單存入結果資訊Model中
        /// </summary>
        /// <param name="dbPromotionInfo">欲存返回的結果資訊Model</param>
        /// <param name="basic">優惠活動資訊</param>
        /// <param name="queryPromotionGiftInterval">優惠折扣級距清單</param>
        private void InsertPromotionInfo(ref DbPromotionInfo dbPromotionInfo, PromotionGiftBasic basic, List<PromotionGiftInterval> queryPromotionGiftInterval)
        {
            List<PromotionGiftInterval> searchPromotionGiftInterval = queryPromotionGiftInterval.Where(x => x.PromotionGiftBasicID == basic.ID).ToList();
            bool isDiscountAmount = searchPromotionGiftInterval.Where(x => x.DiscountAmount > 0 && x.DiscountPercent == 0).ToList().Count > 0 ? true : false;
            bool isDiscountPercent = searchPromotionGiftInterval.Where(x => x.DiscountAmount == 0 && x.DiscountPercent > 0).ToList().Count > 0 ? true : false;
            dbPromotionInfo.isDiscountAmount = dbPromotionInfo.isDiscountAmount == false ? isDiscountAmount : true;
            dbPromotionInfo.isDiscountPercent = dbPromotionInfo.isDiscountPercent == false ? isDiscountPercent : true;
            // 將該活動加入返回結果的優惠活動清單中
            dbPromotionInfo.promotionGiftBasicList.Add(basic);
            // 將該活動的優惠級距清單加入返回結果的優惠級距清單中
            dbPromotionInfo.promotionGiftIntervalList.AddRange(searchPromotionGiftInterval);
        }

        

        /// <summary>
        /// 返回結果funciton
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="isSuccess">成功與否</param>
        /// <param name="data">欲傳遞的資訊</param>
        /// <param name="message">執行結果訊息</param>
        /// <returns>返回結果</returns>
        private ResponseMessage<T> ReturnMessage<T>(bool isSuccess, T data, string message)
        {
            ResponseMessage<T> result = new ResponseMessage<T>();
            result.IsSuccess = isSuccess;
            if (isSuccess == true)
            {
                result.Data = data;
                result.Message = message;
            }
            else
            {
                result.Message = message;
            }

            return result;
        }

    }
}
