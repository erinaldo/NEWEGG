using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using System.Data.Entity;

namespace TWNewEgg.PromotionRepoAdapters.Interface
{
    public interface IPromotionRepoAdapter
    {
        Database GetDatabase();

        #region PromotionGiftBasic優惠活動基本資料

        /// <summary>
        /// 新增優惠活動
        /// </summary>
        /// <param name="argObjGiftBasic">優惠活動基本資料</param>
        void CreatePromotionGiftBasic(Models.DBModels.TWSQLDB.PromotionGiftBasic argObjGiftBasic);

        /// <summary>
        /// 修改優惠活動基本資料
        /// </summary>
        /// <param name="argObjGiftBasic">優惠活動基本資料</param>
        bool UpdatePromotionGiftBasic(Models.DBModels.TWSQLDB.PromotionGiftBasic argObjGiftBasic);

        /// <summary>
        /// 取得PromotionGiftBasic所有資訊
        /// </summary>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftBasic> GetAllPromotionGiftBasic();

        /// <summary>
        /// 以日期取出所有可使用的優惠活動有哪些
        /// </summary>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftBasic> GetPromotionGiftBasicByDate(PromotionGiftBasic.UsedStatus usedStatus);

        /// <summary>
        /// 透過CategoryID取得該CategoryID符合條件的活動優惠有哪些
        /// </summary>
        /// <param name="categoryId">所需檢驗的CategoryID</param>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftBasic> GetPromotionGiftBasicByCategoryID(int categoryId);

        #endregion

        #region PromotionGiftBlackList優惠活動黑名單

        /// <summary>
        /// 新增優惠活動黑名單
        /// </summary>
        /// <param name="argObjGiftBlackList"></param>
        void CreatePromotionGiftBlackList(Models.DBModels.TWSQLDB.PromotionGiftBlackList argObjGiftBlackList);

        /// <summary>
        /// 新增優惠活動黑名單
        /// </summary>
        /// <param name="argListGiftBlackList"></param>
        void CreateRangePromotionGiftBlackList(List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> argListGiftBlackList);

        /// <summary>
        /// 修改優惠活動黑名單
        /// </summary>
        /// <param name="argListGiftBlackList"></param>
        /// <returns></returns>
        bool UpdateRangePromotionGiftBlackList(List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> argListGiftBlackList);

        /// <summary>
        /// 根據PromotionBasicId刪除黑名單
        /// </summary>
        /// <param name="argNumBasicId"></param>
        /// <returns></returns>
        bool DeletePromotionGiftBlackListByBasicId(int argNumBasicId);

        /// <summary>
        /// 修改優惠活動黑名單
        /// </summary>
        /// <param name="argObjGiftBlackList">優惠活動黑名單</param>
        bool UpdatePromotionGiftBlackList(Models.DBModels.TWSQLDB.PromotionGiftBlackList argObjGiftBlackList);

        /// <summary>
        /// 取得PromotionGiftBlackList所有資訊
        /// </summary>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftBlackList> GetAllPromotionGiftBlackList();

        /// <summary>
        /// 取得PromotionGiftBasic.ID底下所有黑名單
        /// </summary>
        /// <param name="basicID">PromotionGiftBasic.ID</param>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftBlackList> GetPromotionGiftBlackList(int basicID);

        /// <summary>
        /// 取得PromotionGiftBasic.ID清單底下所有黑名單
        /// </summary>
        /// <param name="basicIDList">PromotionGiftBasic.ID清單</param>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftBlackList> GetPromotionGiftBlackList(List<int> basicIDList);

        #endregion

        #region PromotionGiftWhiteList優惠活動白名單

        /// <summary>
        /// 新增優惠活動白名單
        /// </summary>
        /// <param name="argObjGiftWhiteList">優惠活動白名單</param>
        void CreatePromotionGiftWhiteList(Models.DBModels.TWSQLDB.PromotionGiftWhiteList argObjGiftWhiteList);

        /// <summary>
        /// 新增優惠活動白名單
        /// </summary>
        /// <param name="argListGiftWhiteList"></param>
        void CreateRangePromotionGiftWhiteList(List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> argListGiftWhiteList);

        /// <summary>
        /// 修改優惠活動白名單
        /// </summary>
        /// <param name="argListGiftWhiteList"></param>
        /// <returns></returns>
        bool UpdateRangePromotionGiftWhiteList(List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> argListGiftWhiteList);

        /// <summary>
        /// 根據PromotionBasicId刪除白名單
        /// </summary>
        /// <param name="argNumBasicId"></param>
        /// <returns></returns>
        bool DeletePromotionGiftWhiteListByBasicId(int argNumBasicId);

        /// <summary>
        /// 修改優惠活動的白名單
        /// </summary>
        /// <param name="argObjGiftWhiteList">優惠活動白名單</param>
        bool UpdatePromotionGiftWhiteList(Models.DBModels.TWSQLDB.PromotionGiftWhiteList argObjGiftWhiteList);

        /// <summary>
        /// 取得PromotionGiftWhiteList所有資訊
        /// </summary>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftWhiteList> GetAllPromotionGiftWhiteList();

        /// <summary>
        /// 取得PromotionGiftBasic.ID底下所有白名單
        /// </summary>
        /// <param name="basicID">PromotionGiftBasic.ID</param>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftWhiteList> GetPromotionGiftWhiteList(int basicID);

        /// <summary>
        /// 取得PromotionGiftBasic.ID清單底下所有白名單
        /// </summary>
        /// <param name="basicIDList">PromotionGiftBasic.ID清單</param>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftWhiteList> GetPromotionGiftWhiteList(List<int> basicIDList);

        #endregion

        #region PromctionGiftInterval優惠活動金額間距
        /// <summary>
        /// 新增優惠活動金額間距
        /// </summary>
        /// <param name="argObjGiftInterval">優惠活動金額間距</param>
        void CreatePromotionGiftInterval(Models.DBModels.TWSQLDB.PromotionGiftInterval argObjGiftInterval);

        /// <summary>
        /// 新增優惠活動金額間距
        /// </summary>
        /// <param name="argListGiftInterval"></param>
        void CreateRangePromotionGiftInterval(List<Models.DBModels.TWSQLDB.PromotionGiftInterval> argListGiftInterval);

        /// <summary>
        /// 修改優惠活動金額間距
        /// </summary>
        /// <param name="argListGiftInterval"></param>
        bool UpdateRangePromotionGiftInterval(List<Models.DBModels.TWSQLDB.PromotionGiftInterval> argListGiftInterval);

        /// <summary>
        /// 修改優惠活動金額間距
        /// </summary>
        /// <param name="argObjGiftInterval">優惠活動金額間距</param>
        bool UpdatePromotionGiftInterval(Models.DBModels.TWSQLDB.PromotionGiftInterval argObjGiftInterval);

        /// <summary>
        /// 刪除優惠活動金額間距
        /// </summary>
        /// <param name="argObjGiftInterval"></param>
        /// <returns></returns>
        bool DeletePromotionGiftInterval(Models.DBModels.TWSQLDB.PromotionGiftInterval argObjGiftInterval);

        /// <summary>
        /// 取得PromotionGiftInterval所有資訊
        /// </summary>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftInterval> GetAllPromotionGiftInterval();

        /// <summary>
        /// 取得在PromotionGiftInterval Table內所有PromotionGiftBasic.ID的優惠折扣名單
        /// </summary>
        /// <param name="basicID">PromotionGiftBasic.ID</param>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftInterval> GetPromotionGiftInterval(int basicID);

        /// <summary>
        /// 取得在PromotionGiftInterval Table內所有PromotionGiftBasic.ID清單的優惠折扣名單
        /// </summary>
        /// <param name="basicIDList">PromotionGiftBasic.ID清單</param>
        /// <returns>返回查詢結果</returns>
        IQueryable<PromotionGiftInterval> GetPromotionGiftInterval(List<int> basicIDList);

        #endregion
    }
}
