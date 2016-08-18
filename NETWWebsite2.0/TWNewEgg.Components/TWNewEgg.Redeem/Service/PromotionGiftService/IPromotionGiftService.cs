using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.Message;
/*using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.Redeem.Model.PromotionGift;
 * */

namespace TWNewEgg.Redeem.Service.PromotionGiftService
{
    public interface IPromotionGiftService
    {
        /// <summary>
        /// 解析滿額贈資訊與返回符合的賣場ID與折扣金額
        /// </summary>
        /// <param name="oriPriceData">商品原始金額</param>
        /// <param name="turnON">是否啟用正式機狀態的開關閥，正式機on、測試機off</param>
        /// <returns>返回符合滿額贈的賣場ID與折扣金額</returns>
        List<Models.DomainModels.Redeem.GetItemTaxDetail> PromotionGiftParsing(int accountID, Dictionary<string, List<Models.DomainModels.Redeem.GetItemTaxDetail>> oriPriceData, string turnON);

         /// <summary>
        /// 解析滿額贈資訊與返回符合的賣場ID與折扣金額
        /// </summary>
        /// <param name="PromotionInputList">PromotionInputList資訊</param>
        /// <param name="turnON">是否啟用正式機狀態的開關閥，正式機on、測試機off</param>
        /// <returns>返回符合滿額贈的賣場ID與折扣金額</returns>
        List<PromotionDetail> PromotionGiftParsingV2(int accountID, List<PromotionInput> PromotionInputList, string turnON);

        /// <summary>
        /// 傳入購物車所有的Item
        /// </summary>
        /// <param name="itemPriceData">購物車中所有Item的ID與該商品金額乘數量後的總金額</param>
        /// <param name="usedStatus">搜尋PromotionGiftBasic哪種狀態下的資訊</param>
        /// <param name="whiteListStatus">搜尋PromotionGiftWhiteList哪種狀態下的資訊</param>
        /// <param name="blackListStatus">搜尋PromotionGiftBlackList哪種狀態下的資訊</param>
        /// <returns>List<GetItemTaxDetail>物件, 可使用滿額贈的ItemIDList, 滿額贈折扣金額</returns>
        List<Models.DomainModels.Redeem.GetItemTaxDetail> GetPromotionGiftFromItems(int accountID, Dictionary<int, decimal> itemPriceData, Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus usedStatus, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus whiteListStatus, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus blackListStatus);

        /// <summary>
        /// 傳入購物車所有的Item
        /// </summary>
        /// <param name="itemPriceData">購物車中所有Item的ID與該商品金額乘數量後的總金額</param>
        /// <param name="usedStatus">搜尋PromotionGiftBasic哪種狀態下的資訊</param>
        /// <param name="whiteListStatus">搜尋PromotionGiftWhiteList哪種狀態下的資訊</param>
        /// <param name="blackListStatus">搜尋PromotionGiftBlackList哪種狀態下的資訊</param>
        /// <returns>List<GetItemTaxDetail>物件, 可使用滿額贈的ItemIDList, 滿額贈折扣金額</returns>
        List<Models.DomainModels.Redeem.PromotionDetail> GetPromotionGiftFromItemsV2(int accountID, Dictionary<int, decimal> itemPriceData, Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus usedStatus, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus whiteListStatus, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus blackListStatus);
        
        /// <summary>
        /// 組合List<GetItemTaxDetail>
        /// </summary>
        /// <param name="promotionGiftDetailList">滿額贈清單</param>
        /// <returns>返回List<GetItemTaxDetail></returns>
        List<Models.DomainModels.Redeem.GetItemTaxDetail> ConvertToItemTaxDetailList(List<Models.DomainModels.Redeem.PromotionGiftDetail> promotionGiftDetailList);

        //List<Models.DomainModels.Redeem.PromotionGiftDetail> SortItemTaxDetail(List<Models.DomainModels.Redeem.PromotionGiftDetail> sortData);

        /// <summary>
        /// JSON轉物件後返回所要顯示的優惠折扣資訊字串
        /// </summary>
        /// <param name="promationGiftDetail">Model資訊</param>
        /// <returns>返回所要顯示的優惠折扣資訊字串</returns>
        //string GetPromotionOfferStr(List<Models.DomainModels.Redeem.GetItemTaxDetail> promationGiftDetail);

        /// <summary>
        /// 解析Json並返回PromotionGiftDetail型態的資訊
        /// </summary>
        /// <param name="jsonData">Json格式資訊</param>
        /// <returns>返回PromotionGiftDetail型態的資訊</returns>
        //Models.DomainModels.Redeem.PromotionGiftDetail GetPromotionOfferList(string jsonData);

        /// <summary>
        /// 判定滿額贈折價金額使用何種方式取得
        /// </summary>
        /// <param name="totalPrice">可使用滿額贈商品總金額</param>
        /// <param name="promotionGiftInterval">符合的滿額贈級距資訊</param>
        /// <returns>返回滿額贈折價金額</returns>
        decimal DecideDiscountAmount(decimal totalPrice, Models.DomainModels.Redeem.PromotionGiftInterval promotionGiftInterval);

        /// <summary>
        /// 找出所有Item的資訊
        /// </summary>
        /// <param name="itemIDList">賣場ID清單</param>
        /// <returns>返回Item清單</returns>
        List<Models.DomainModels.Item.ItemDetail> FindItemData(List<int> itemIDList);
        
        /// <summary>
        /// 屬於這些類別的Item有哪些(同時檢查是否有設定全站類別 categoryID = 0)
        /// </summary>
        /// <param name="listItem">購物車中所有Item</param>
        /// <param name="categoryList">符合此滿額贈活動的類別清單</param>
        /// <returns>返回屬於這些類別的Item</returns>
        List<Models.DomainModels.Item.ItemDetail> GetLegalItem(List<Models.DomainModels.Item.ItemDetail> listItem, List<int> categoryList);

        /// <summary>
        /// 取得商品總金額
        /// </summary>
        /// <param name="item">賣場資訊</param>
        /// <param name="itemPriceData">購物車中所有賣場ID與其扣除聰明購後顯示金額</param>
        /// <returns>返回總金額</returns>
        decimal GetItemTotalPrice(Models.DomainModels.Item.ItemDetail item, Dictionary<int, decimal> itemPriceData);

        /// <summary>
        /// 取得商品清單總金額
        /// </summary>
        /// <param name="items">賣場資訊清單</param>
        /// <param name="itemPriceData">購物車中所有Item的ID與金額</param>
        /// <returns>返回總金額</returns>
        decimal GetItemTotalPrice(List<Models.DomainModels.Item.ItemDetail> items, Dictionary<int, decimal> itemPriceData);

        /// <summary>
        /// 解析分類清單
        /// </summary>
        /// <param name="categories">分類String</param>
        /// <returns>返回類別的int清單</returns>
        List<int> CategoryParsing(string categories);

        /// <summary>
        /// 取得所有可用的PromotionGiftBasic
        /// </summary>
        /// <param name="usedStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回PromotionGiftBasic清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftBasic> GetPromotionGiftBasicByStatus(Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus usedStatus);

        //List<Models.DomainModels.Redeem.PromotionGiftBasic> OrderProcess(List<Models.DomainModels.Redeem.PromotionGiftBasic> promotionGiftBasic);

        /// <summary>
        /// 根據PromotionGiftBasic Id取得該物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <returns>null or PromotionGiftBasic object</returns>
        Models.DomainModels.Redeem.PromotionGiftBasic GetPromotionGiftBasicByBasicId(int argNumPromotionGiftBasicId);

        /// <summary>
        /// 取得所有PromotionGiftBasic清單
        /// </summary>
        /// <returns>null or List of PromotionGiftBasic</returns>
        List<Models.DomainModels.Redeem.PromotionGiftBasic> GetAllPromotionGiftBasic();

        /// <summary>
        /// 根據PromotionGiftBasicID(滿額贈ID)取得該級距清單
        /// </summary>
        /// <param name="basicIDList">PromotionGiftBasicID(滿額贈ID)清單</param>
        /// <returns>返回級距清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftInterval> GetPromotionGiftIntervalList(List<int> basicIDList);

        /// <summary>
        /// 取得滿額贈級距資訊
        /// </summary>
        /// <param name="totalPrice">購物車可用滿額贈商品總額</param>
        /// <param name="promotionGiftIntervals">滿額贈級距清單</param>
        /// <returns>返回滿額贈級距資訊</returns>
        Models.DomainModels.Redeem.PromotionGiftInterval GetPromotionGiftInterval(decimal totalPrice, List<Models.DomainModels.Redeem.PromotionGiftInterval> promotionGiftIntervals);

        /// <summary>
        /// 取據級距ID取得該資料
        /// </summary>
        /// <param name="argNumIntervalId">級距Id</param>
        /// <returns>nuul or PromotionGiftInterval object</returns>
        Models.DomainModels.Redeem.PromotionGiftInterval GetPromotionGiftIntervalByIntervalId(int argNumIntervalId);

        /// <summary>
        /// 根據itemID取得白名單清單
        /// </summary>
        /// <param name="itemID">所要取得的itemID</param>
        /// <param name="whiteListStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回白名單清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftWhiteList> GetPromotionGiftWhiteListByItemAndStatus(List<int> basicIDList, int itemID, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus whiteListStatus);

        /// <summary>
        /// 根據itemID取得黑名單清單
        /// </summary>
        /// <param name="itemID">所要取得的itemID</param>
        /// <param name="blackListStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回黑名單清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftBlackList> GetPromotionGiftBlackListByItemAndStatus(List<int> basicIDList, int itemID, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus blackListStatus);

        /// <summary>
        /// 根據itemID清單取得白名單清單
        /// </summary>
        /// <param name="itemIDs">所要取得的itemID清單</param>
        /// <param name="blackListStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回白名單清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftWhiteList> GetPromotionGiftWhiteListByBasicAndItemAndStatus(List<int> basicIDList, List<int> itemIDs, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus whiteListStatus);

        /// <summary>
        /// 根據itemID清單取得黑名單清單
        /// </summary>
        /// <param name="itemIDs">所要取得的itemID清單</param>
        /// <param name="blackListStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回黑名單清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftBlackList> GetPromotionGiftBlackListByBasicAndItemAndStatus(List<int> basicIDList, List<int> itemIDs, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus blackListStatus);

        /// <summary>
        /// 傳入購物車ID與商品資訊，會自動拆單並建立滿額贈的拆單記錄
        /// </summary>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <param name="oriPriceData">初始金額資訊</param>
        /// <param name="turnOn">是否啟動正式機的Status</param>
        /// <returns>拆單成功:true, 拆單失敗:false</returns>
        bool CreatePromotionGiftRecord(int salesOrderGroupID, Dictionary<string, List<Models.DomainModels.Redeem.GetItemTaxDetail>> oriPriceData, string turnOn);

        /// <summary>
        /// 傳入購物車ID與商品資訊，會自動拆單並建立滿額贈的拆單記錄
        /// </summary>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <param name="promotionGiftDetailList">promotionGiftDetailList資訊</param>
        /// <param name="turnOn">是否啟動正式機的Status</param>
        /// <returns>拆單成功:true, 拆單失敗:false</returns>
        bool CreatePromotionGiftRecordV2(int salesOrderGroupID, List<Models.DomainModels.Redeem.PromotionDetail> promotionGiftDetailList, string turnOn);

        /// <summary>
        /// 解析promotionData並執行拆單與儲存動作
        /// </summary>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <param name="oriPriceData">初始金額資訊</param>
        /// <param name="turnOn">是否啟動正式機的Status</param>
        /// <returns>滿額贈紀錄清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftRecords> DecompositionAndStorage(int salesOrderGroupID, Dictionary<string, List<Models.DomainModels.Redeem.GetItemTaxDetail>> oriPriceData, string turnOn);

        /// <summary>
        /// 解析promotionData並執行拆單與儲存動作
        /// </summary>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <param name="promotionGiftDetailList">promotionGiftDetailList資訊</param>
        /// <param name="turnOn">是否啟動正式機的Status</param>
        /// <returns>滿額贈紀錄清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftRecords> DecompositionAndStorageV2(int salesOrderGroupID, List<Models.DomainModels.Redeem.PromotionDetail> promotionGiftDetailList, string turnOn);

        /// <summary>
        /// 自動分配與拆解滿額贈金額
        /// </summary>
        /// <param name="totalPrice">符合滿額贈的商品總金額</param>
        /// <param name="promotionGiftPrice">滿額贈折價級距金額</param>
        /// <param name="salesOrderList">SalesOrder清單</param>
        /// <param name="salesOrderItemList">SalesOrderItem清單</param>
        /// <param name="itemTaxDetail">商品稅金與金額細項</param>
        /// <param name="promotionGiftRecords">滿額贈紀錄表</param>
        /// <param name="promotionGiftBasicID">滿額贈活動ID</param>
        /// <param name="promotionGiftIntervalID">滿額贈級距ID</param>
        /// <returns>滿額贈紀錄清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftRecords> CombinePromotionGiftRecords(
            decimal totalPrice
            , decimal promotionGiftPrice
            , List<Models.DomainModels.Cart.SOBase> salesOrderList
            , List<Models.DomainModels.Cart.SOItemBase> salesOrderItemList
            , List<Models.DomainModels.Redeem.GetItemTaxDetail> itemTaxDetail
            , List<Models.DomainModels.Redeem.PromotionGiftRecords> promotionGiftRecords
            , int promotionGiftBasicID
            , int promotionGiftIntervalID);

        /// <summary>
        /// 自動分配與拆解滿額贈金額
        /// </summary>
        /// <param name="totalPrice">符合滿額贈的商品總金額</param>
        /// <param name="promotionGiftPrice">滿額贈折價級距金額</param>
        /// <param name="salesOrderList">SalesOrder清單</param>
        /// <param name="salesOrderItemList">SalesOrderItem清單</param>
        /// <param name="itemDisplayPriceList">itemDisplayPriceList細項</param>
        /// <param name="promotionGiftRecords">滿額贈紀錄表</param>
        /// <param name="promotionGiftBasicID">滿額贈活動ID</param>
        /// <param name="promotionGiftIntervalID">滿額贈級距ID</param>
        /// <returns>滿額贈紀錄清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftRecords> CombinePromotionGiftRecordsV2(
            decimal totalPrice
            , decimal promotionGiftPrice
            , List<Models.DomainModels.Cart.SOBase> salesOrderList
            , List<Models.DomainModels.Cart.SOItemBase> salesOrderItemList
            , List<Models.DomainModels.Item.ItemPrice> itemDisplayPriceList
            , List<Models.DomainModels.Redeem.PromotionGiftRecords> promotionGiftRecords
            , int promotionGiftBasicID
            , int promotionGiftIntervalID);

        /// <summary>
        /// 取得紀錄PromotionGift的資訊
        /// </summary>
        /// <param name="oriPriceData">初始金額資訊</param>
        /// <returns>返回滿額贈資訊清單</returns>
        List<Models.DomainModels.Redeem.GetItemTaxDetail> GetPromotionGiftData(Dictionary<string, List<Models.DomainModels.Redeem.GetItemTaxDetail>> oriPriceData);

        /// <summary>
        /// 根據整個購物車ID, 找出旗下滿額贈的記錄, 修改其PromotionGiftRecord的狀態
        /// </summary>
        /// <param name="argSalesOrderGroupId">購物車的ID</param>
        /// <param name="argUsedStatus">欲修改的狀態</param>
        /// <returns>修改成功:true; 修改失敗:false</returns>
        bool UpdatePromotionGiftRecordBySOGroupId(int argSalesOrderGroupId, Models.DomainModels.Redeem.PromotionGiftRecords.UsedStatusOption argUsedStatus);

        /// <summary>
        /// 根據SalesOrderItemCode, 修改其PromotionGiftRecord的狀態
        /// </summary>
        /// <param name="argSoItemCode">SalesOrderItemCode</param>
        /// <param name="argUsedStatus">欲修改的狀態</param>
        /// <returns>修改成功:true; 修改失敗:false</returns>
        bool UpdatePromotionGiftRecordBySoItemCode(string argSoItemCode, Models.DomainModels.Redeem.PromotionGiftRecords.UsedStatusOption argUsedStatus);

        /// <summary>
        /// 訂單填入買一送一優惠訊息
        /// </summary>
        /// <param name="salesOrderGroupID">購物車編號</param>
        /// <param name="buyingItemIDs">購物車商品賣場ID清單</param>
        /// <returns>返回執行結果execResult，若execResult為空字串則表示執行成功，若失敗則返回執行失敗原因</returns>
        string SalesOrderComment(int salesOrderGroupID, List<int> buyingItemIDs);

        /// <summary>
        /// 新增PromotionGiftBasic
        /// </summary>
        /// <param name="argObjPromotionGiftBasic">新增的PromotionGiftBasic物件</param>
        /// <returns>create success return new PromotionGiftBasic ID, else return 0</returns>
        int CreatePromotionGiftBasic(Models.DomainModels.Redeem.PromotionGiftBasic argObjPromotionGiftBasic);

        /// <summary>
        /// 修改PromotionGiftBasic
        /// </summary>
        /// <param name="argObjPromotionGiftBasic"></param>
        /// <returns>update success return true, else return false</returns>
        ActionResponse<bool> UpdatePromotionGiftBasic(Models.DomainModels.Redeem.PromotionGiftBasic argObjPromotionGiftBasic);

        /// <summary>
        /// 根據PromotionGiftBasic取得白名單清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic.ID</param>
        /// <returns>返回白名單清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftWhiteList> GetPromotionGiftWhiteListByBasicId(int argNumPromotionGiftBasicId);

        /// <summary>
        /// 根據PromotionGiftBasic Id 及PromotionGiftWhiteList Id取得黑名單物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argNumPromotionGiftWhiteListId">PromotionGiftBasicList Id</param>
        /// <returns>null or PromotionGiftWhiteList object</returns>
        Models.DomainModels.Redeem.PromotionGiftWhiteList GetPromotionGiftWhiteList(int argNumPromotionGiftBasicId, int argNumPromotionGiftWhiteListId);

        /// <summary>
        /// 新增一個WhiteList物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftWhiteList">object of PromotionGiftWhiteList</param>
        /// <returns>created success return WhiteList Id, else return 0</returns>
        int CreatePromotionGiftWhiteList(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftWhiteList argObjPromotionGiftWhiteList);

        /// <summary>
        /// 更新該PromotionGifbtBasic下的White清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argListPromotionGiftWhiteList">White List</param>
        /// <returns>update success return true, else return false</returns>
        bool UpdatePromotionGiftWhiteListByList(int argNumPromotionGiftBasicId, List<Models.DomainModels.Redeem.PromotionGiftWhiteList> argListPromotionGiftWhiteList);

        /// <summary>
        /// 更新該PromotionGifbtBasic下的White清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftWhiteList">White List object</param>
        /// <returns>updated success return true, else return false</returns>
        bool UpdatePromotionGiftWhiteList(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftWhiteList argObjPromotionGiftWhiteList);

        /// <summary>
        /// 根據PromotionGiftBasic清單取得黑名單清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic.ID</param>
        /// <returns>返回黑名單清單</returns>
        List<Models.DomainModels.Redeem.PromotionGiftBlackList> GetPromotionGiftBlackListByBasicId(int argNumPromotionGiftBasicId);

        /// <summary>
        /// 根據PromotionGiftBasic Id 及PromotionGiftBlackList Id取得黑名單物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argNumPromotionGiftBlackListId">PromotionGiftBasicList Id</param>
        /// <returns>null or PromotionGiftBlackList object</returns>
        Models.DomainModels.Redeem.PromotionGiftBlackList GetPromotionGiftBlackList(int argNumPromotionGiftBasicId, int argNumPromotionGiftBlackListId);

        /// <summary>
        /// 新增一個BlackList物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftBlackList">object of PromotionGiftBlackList</param>
        /// <returns>created success return BlackList Id, else return 0</returns>
        int CreatePromotionGiftBlackList(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftBlackList argObjPromotionGiftBlackList);

        /// <summary>
        /// 更新該PromotionGifbtBasic下的Black清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argListPromotionGiftBlackList">Black List</param>
        /// <returns></returns>
        bool UpdatePromotionGiftBlackListByList(int argNumPromotionGiftBasicId, List<Models.DomainModels.Redeem.PromotionGiftBlackList> argListPromotionGiftBlackList);

        /// <summary>
        /// 更新該PromotionGifbtBasic下的Black清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftBlackList">Black List object</param>
        /// <returns>updated success return true, else return false</returns>
        bool UpdatePromotionGiftBlackList(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftBlackList argObjPromotionGiftBlackList);

        /// <summary>
        /// 新增級距
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftInterval">新級的級距物件</param>
        /// <returns>if create success return interval object id , else return 0</returns>
        int CreatePromotionGiftInterval(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftInterval argObjPromotionGiftInterval);

        /// <summary>
        /// 修改級距
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftInterval">要修改的級距</param>
        /// <returns>if updated success return true, else return false</returns>
        bool UpdatePromotionGiftInterval(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftInterval argObjPromotionGiftInterval);

        /// <summary>
        /// 修改級距
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argListPromotionGiftInterval">要修改的級距列表</param>
        /// <returns>updated success return true, else return false</returns>
        bool UpdatePromotionGiftIntervalList(int argNumPromotionGiftBasicId, List<Models.DomainModels.Redeem.PromotionGiftInterval> argListPromotionGiftInterval);

        /// <summary>
        /// 根據Basic Id取得其相關的級距列表
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <returns>null or List of PromotionGiftInterval</returns>
        List<Models.DomainModels.Redeem.PromotionGiftInterval> GetPromotionGiftIntervalListByBasicId(int argNumPromotionGiftBasicId);

        /// <summary>
        /// 根據Basic Id刪除其名單
        /// </summary>
        ActionResponse<string> DeleteList(int argNumPromotionGiftBasicId, string flag);

        /// <summary>
        /// 根據Basic Id取得其相關的級距列表
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <returns>null or List of PromotionGiftExportToExcel</returns>
        /// 
        ActionResponse<List<Models.DomainModels.Redeem.PromotionGiftExportToExcel>> PromotionGiftBasicIdToExcel(int PromotionGiftBasicId, string ListType);

        ActionResponse<string> BatchCreatePomoteBasicPromotionGift(int basicID, List<int> ItemID, List<int> Status, string list,string UserName);

        List<Models.DomainModels.Redeem.PromotionGiftBasic> GetPromotionGiftBasicByIdList(List<int> argListBasicId);

        /// <summary>
        /// 透過Item.ID List取得該Item所有可參與的PromotionGift
        /// </summary>
        /// <param name="itemIDList">賣場ID清單</param>
        /// <returns>返回結果</returns>
        Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>> getItemPromotionGiftListInfo(int accountID, List<int> itemIDList, string turnON);
    }
}
