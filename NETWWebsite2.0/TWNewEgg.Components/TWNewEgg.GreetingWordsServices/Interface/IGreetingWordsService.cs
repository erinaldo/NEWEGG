using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.GreetingWords;

namespace TWNewEgg.GreetingWordsServices.Interface
{
    /// <summary>
    /// -----------------------add by bruce 20160329
    ///1 登入問候語
    ///2 節日問候卡
    /// </summary>
    public interface IGreetingWordsService
    {
        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        GreetingWordsDM GetInfo(int? id);

        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="description">文字描述</param>
        /// <param name="showall">是否顯示, 1:顯示, 0:不顯示</param>
        /// <returns></returns>
        List<GreetingWordsDM> GetData(string description, int? showall);

        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        List<GreetingWordsDM> GetAll();

        /// <summary>
        /// 取得目前有效資料
        /// </summary>
        /// <param name="category_id">
        /// 分類Id
        /// 0 首頁熱門關鍵字
        /// 1 登入問候語
        /// 2 節日問候卡
        /// </param>
        /// <param name="now_date">
        /// 目前時間
        /// </param>
        /// <returns></returns>
        List<GreetingWordsDM> GetShow(DateTime now_date);

        ////IQueryable<GreetingWordsDM> GetByCategory(int category_id);
        ////IQueryable<GreetingWordsDM> GetByDescription(string description);
        ////IQueryable<GreetingWordsDM> GetByShowAll(int showall);
        ////IQueryable<GreetingWordsDM> GetByCodeText(string codetext);

        bool Update(GreetingWordsDM info);

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        GreetingWordsDM Add(GreetingWordsDM info);

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="list_info">domain model</param>
        /// <returns></returns>
        List<GreetingWordsDM> Add(List<GreetingWordsDM> list_info);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        bool Del(GreetingWordsDM info);

        //void Del(GreetingWordsDM info);
    }
}
