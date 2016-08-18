using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.ItemService.Models;

namespace TWNewEgg.ItemService.PayTypeService
{
    public class CreditCardPayTypeCheck
    {
        /// <summary>
        /// 確認付款條件是否符合規則(十二期零利率)
        /// </summary>
        /// <param name="postData"></param>
        /// <returns>符合則回傳true，否則回傳false</returns>
        public bool CheckPayCondition(List<BuyingItems> postData)
        {
            TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();

            // 取得符合十二期分期零利率的商家SellerID
            string payConditionBySeller = System.Configuration.ConfigurationManager.AppSettings["PayConditionBySeller"];
            // 取得符合十二期分期零利率的商品ItemID
            string payConditionByItem = System.Configuration.ConfigurationManager.AppSettings["PayConditionByItem"];
            // 取得不符合十二期零利率的商品ItemID(透過ItemID來識別)
            string payConditionByItemWithout12 = System.Configuration.ConfigurationManager.AppSettings["PayConditionByItemWithout12"];

            List<string> payConditionBySellerList = payConditionBySeller.Split(',').ToList();
            List<string> payConditionByItemList = payConditionByItem.Split(',').ToList();
            List<string> payConditionByItemWithout12List = payConditionByItemWithout12.Split(',').ToList(); // (透過ItemID來識別)

            //// (透過品牌來識別)
            //// 找出此購物車中的所有華克商品的ItemID List，並經由Item Table取得所有item的ManufactureID
            //List<int> itemIDList = postData.Where(x => payConditionBySellerList.Contains(x.buyItemID_Seller.ToString())).Select(x => x.buyItemID).ToList();
            //List<int> manufactureIDList = db_before.Item.Where(x => itemIDList.Contains(x.ID)).Select(x => x.ManufactureID).ToList();
            //// 找出品牌為DELL的ManufactureID List，並檢查此購物車中是否有包含該品牌的商品
            //List<int> dellManufactureIDList = db_before.Manufacture.Where(x => x.Name.ToLower() == "dell").Select(x => x.ID).ToList();
            //// DELL商品的數量
            //int illegalItemCondition = manufactureIDList.Where(x => dellManufactureIDList.Contains(x)).ToList().Count;
            //// 若商品包含DELL品牌則回傳false
            //if (illegalItemCondition > 0)
            //{
            //    return false;
            //}

            // 核實該購物車中是否有不符合十二期零利率的旗標
            bool checkSellerAndItem = true;
            foreach (BuyingItems buying in postData)
            {
                // 檢查商品的SellerID是否有不在設定商家中的(ex. 華克之外的商家)，若有則返回false
                var searchSellerID = payConditionBySellerList.Where(x => x.Trim() == buying.buyItemID_Seller.ToString()).FirstOrDefault();
                // 檢查商品的ItemID是否有不在設定商品中的(只有部分商品允許十二期零利率)，若有則返回false
                var searchItemID = payConditionByItemList.Where(x => x.Trim() == buying.buyItemID.ToString()).FirstOrDefault();
                // 檢查商品的ItemID是否有在設定商品中(設定不可使用十二期零利率的商品)，若有則返回false(透過ItemID來識別)
                var searchItemIDWithout12 = payConditionByItemWithout12List.Where(x => x.Trim() == buying.buyItemID.ToString()).FirstOrDefault();
                // 若該商品是不符合十二期零利率的設定，則回傳false
                if (searchSellerID == null && searchItemID == null)
                {
                    checkSellerAndItem = false;
                }
                // 購物車中包含不可以使用十二期零利率的商品，則回傳false(透過ItemID來識別)
                if (searchItemIDWithout12 != null)
                {
                    checkSellerAndItem = false;
                }
            }

            return checkSellerAndItem;
        }
        
        /// <summary>
        /// 檢驗符合條件的分期付款類型
        /// </summary>
        /// <param name="payTypeList">所有付款類型清單</param>
        /// <param name="payCondition">是否符合十二期零利率資格</param>
        /// <returns>返回信用卡分期付款清單</returns>
        public List<PayType> GetCreditCardPayType(List<PayType> payTypeList, bool payCondition)
        {
            List<PayType> cardList = new List<PayType>();
            cardList = payTypeList.Where(x => (x.PayType0rateNum == (int)PayType.nPayType.三期零利率 || x.PayType0rateNum == (int)PayType.nPayType.六期零利率
                        || x.PayType0rateNum == (int)PayType.nPayType.十期零利率 || x.PayType0rateNum == (int)PayType.nPayType.十八期零利率 || x.PayType0rateNum == (int)PayType.nPayType.二十四期零利率
                        || x.PayType0rateNum == (int)PayType.nPayType.十期分期 || x.PayType0rateNum == (int)PayType.nPayType.十八期分期 || x.PayType0rateNum == (int)PayType.nPayType.二十四期分期
                        )).OrderBy(x => x.BankID).ToList<PayType>();
            // 十二期有息分期與十二期零利率互斥，當有其一種存在必不存在另一種
            if (payCondition)
            {
                // 此購物車商品符合十二期零利率規則，故撈取十二期零利率的所有付款模式
                List<PayType> getPayType012 = payTypeList.Where(x => x.PayType0rateNum == (int)PayType.nPayType.十二期零利率).ToList();
                // 含十二期零利率的銀行
                List<int> getPayType012BankID = getPayType012.Select(x => (int)x.BankID).Distinct().ToList();
                // 有十二期零利率的銀行必不存在十二期分期，所以將已包含十二期零利率的銀行排除在十二期分期撈取之外
                List<PayType> getPayType112 = payTypeList.Where(x => x.PayType0rateNum == (int)PayType.nPayType.十二期分期 && !getPayType012BankID.Contains((int)x.BankID)).ToList();
                // 加入十二期零利率的付款模式
                cardList.AddRange(getPayType012);
                // 加入十二期分期的付款模式
                cardList.AddRange(getPayType112);
                cardList = cardList.OrderBy(x => x.BankID).ToList();
            }
            else
            {
                // 此購物車商品不符合十二期零利率規則，故只撈取十二期分期付款模式
                List<PayType> getPayType112 = payTypeList.Where(x => x.PayType0rateNum == (int)PayType.nPayType.十二期分期).ToList();
                cardList.AddRange(getPayType112);
                cardList = cardList.OrderBy(x => x.BankID).ToList();
            }

            return cardList;
        }

        /// <summary>
        /// 金額小於8000無法使用12、18、24分期
        /// </summary>
        /// <param name="cardPayTypeList">信用卡刷卡清單</param>
        /// <param name="priceSum">購物車總金額</param>
        /// <param name="totalCouponPrice">折價卷總金額</param>
        /// <param name="discountAmount">活動折抵總金額</param>
        public void PayTypeAndPriceCheck(ref List<PayType> cardPayTypeList, decimal priceSum, decimal totalCouponPrice, int discountAmount)
        {
            if (priceSum - totalCouponPrice - (decimal)discountAmount < 8000m)
            {
                cardPayTypeList = cardPayTypeList.Where(x =>
                        x.PayType0rateNum != (int)PayType.nPayType.十二期分期
                        && x.PayType0rateNum != (int)PayType.nPayType.十八期分期
                        && x.PayType0rateNum != (int)PayType.nPayType.二十四期分期
                        ).ToList();
            }
        }
    }
}