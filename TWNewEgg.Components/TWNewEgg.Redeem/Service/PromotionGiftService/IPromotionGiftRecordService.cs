using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TWNewEgg.DB;
//using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.Models.DomainModels.Redeem;

namespace TWNewEgg.Redeem.Service.PromotionGiftService
{
    public interface IPromotionGiftRecordService
    {
        /// <summary>
        /// 新增PromotionGiftRecord
        /// </summary>
        /// <param name="argObjGiftRecord">欲新增的PromotionGiftRecord物件</param>
        /// <returns>新增成功:true, 新增失敗:false</returns>
        bool CreatePromotionGiftRecord(Models.DomainModels.Redeem.PromotionGiftRecords argObjGiftRecord);

        /// <summary>
        /// 新增List內的所有PromotionGiftRecord
        /// </summary>
        /// <param name="argListGiftRecord">List of PromotionGiftRecord</param>
        /// <returns>新增成功:true, 新增失敗:false</returns>
        bool CreateRangePromotionGiftRecord(List<Models.DomainModels.Redeem.PromotionGiftRecords> argListGiftRecord);

        /// <summary>
        /// 修改PromotionGiftRecord
        /// </summary>
        /// <param name="argObjGiftRecord">PromotionGiftRecord物件</param>
        /// <returns>修改成功:true, 修改失敗:false</returns>
        bool UpdatePromotionGiftRecord(Models.DomainModels.Redeem.PromotionGiftRecords argObjGiftRecord);

        /// <summary>
        /// 修改List裡所有的PromotionGiftRecord
        /// </summary>
        /// <param name="argListGiftRecords">List of PromotionGiftRecord</param>
        /// <returns>修改成功:true, 修改失敗:false</returns>
        bool UpdatePromotionGiftRecordByList(List<Models.DomainModels.Redeem.PromotionGiftRecords> argListGiftRecords);

        /// <summary>
        /// 根據購物車ID, 取得相關PromotionGiftRecord
        /// </summary>
        /// <param name="argSalesOrderGroupId">購物車ID</param>
        /// <returns>null或是PromotionGiftRecord List</returns>
        List<Models.DomainModels.Redeem.PromotionGiftRecords> GetGiftRecordsBySalesOrderGroupId(int argSalesOrderGroupId);

        /// <summary>
        /// 根據SalesOrderItemCode List, 取得相關PromotionGiftRecord
        /// </summary>
        /// <param name="argListSalesOrderItemCode">SalesOrderItemCode List</param>
        /// <returns>null或是PromotionGiftRecord List</returns>
        List<Models.DomainModels.Redeem.PromotionGiftRecords> GetGiftRecordsByListSalesOrderItemCode(List<string> argListSalesOrderItemCode);

        /// <summary>
        /// 根據SalesOrderItemCode取得PromotionGiftRecord
        /// </summary>
        /// <param name="argStrSalesOrderItemCode">SalesOrderItemCode</param>
        /// <returns>null或是PromotionGiftRecord物件</returns>
        Models.DomainModels.Redeem.PromotionGiftRecords GetGiftRecordsBySalesOrderItemCode(string argStrSalesOrderItemCode);
    }
}
