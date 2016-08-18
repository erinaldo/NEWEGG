using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Discard4;

namespace TWNewEgg.Discard4Services.Interface
{
    /// <summary>
    /// 依據 BSATW-173 廢四機需求增加 廢四機賣場商品, 1=是廢四機 ---------------add by bruce 20160502
    /// 廢四機四聯單
    /// </summary>
    public interface IDiscard4ItemService
    {
         /// <summary>
        /// 初始化癈四機回收四聯單
        /// </summary>
        /// <param name="salesorderCode"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        List<Discard4ItemDM> InitData(string salesorderCode, string user_name);

        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="list_info"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        List<bool> Save(List<Discard4ItemDM> list_info, string user_name);

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        Discard4ItemDM GetInfo(int? id);


        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        List<Discard4ItemDM> GetData(Discard4ItemDM info);

        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        List<Discard4ItemDM> GetAll();

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool Update(Discard4ItemDM info, string user_name);

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        Discard4ItemDM Add(Discard4ItemDM info, string user_name);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Del(int? id);

    }
}
