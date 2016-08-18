using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.PromotionRepoAdapters.Interface
{
    public interface IPromotionRecordsRepoAdapter
    {
        /// <summary>
        /// 新增Records
        /// </summary>
        /// <param name="argObjGiftRec"></param>
        void CreatePromotionGiftRecords(PromotionGiftRecords argObjGiftRec);

        /// <summary>
        /// 新增Records
        /// </summary>
        /// <param name="argListGiftRec"></param>
        void CreateRangePromotionGiftRecords(List<PromotionGiftRecords> argListGiftRec);

        /// <summary>
        /// 修改Records
        /// </summary>
        /// <param name="argObjGiftRec"></param>
        /// <returns></returns>
        bool UpdatePromotionGiftRecords(PromotionGiftRecords argObjGiftRec);

        /// <summary>
        /// 修改Records
        /// </summary>
        /// <param name="argListGiftRec"></param>
        /// <returns></returns>
        bool UpdateRangePromotionGiftRecords(List<PromotionGiftRecords> argListGiftRec);

        /// <summary>
        /// 取得所有的Records
        /// </summary>
        /// <returns></returns>
        IQueryable<PromotionGiftRecords> GetAllPromotionGiftRecords();
    }
}
