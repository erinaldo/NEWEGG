using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Discard4;

namespace TWNewEgg.Discard4Services.Interface
{
    /// <summary>
    /// 依據 BSATW-173 廢四機需求增加 癈四機賣場商品, 1=是癈四機 ---------------add by bruce 20160429
    /// 廢四機同意
    /// </summary>
    public interface IDiscard4Service
    {

        /// <summary>
        /// 初始化廢四機同意
        /// </summary>
        /// <param name="salesOrderGroupID"></param>
        /// <param name="agreedDiscard4"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        Discard4DM InitData(int salesOrderGroupID, string agreedDiscard4, string user_name);


        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        Discard4DM GetInfo(int? id);


        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        List<Discard4DM> GetData(Discard4DM info);

        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        List<Discard4DM> GetAll();

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool Update(Discard4DM info, string user_name);

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        Discard4DM Add(Discard4DM info, string user_name);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Del(int? id);

    }
}
