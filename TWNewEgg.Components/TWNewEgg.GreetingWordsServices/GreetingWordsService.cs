using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.GreetingWordsServices.Interface;
using TWNewEgg.GreetingWordsRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.GreetingWords;
using TWNewEgg.Models.DBModels.TWSQLDB;

using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.GreetingWordsServices
{
    /// <summary>
    /// -----------------------add by bruce 20160329
    ///1 登入問候語
    ///2 節日問候卡
    /// </summary>
    public class GreetingWordsService //: IGreetingWordsService
    {
        /// <summary>
        /// db a table adapter
        /// </summary>
        private IGreetingWordsRepoAdapter _adapter;

        /// <summary>
        /// 分類Id
        /// 0 首頁熱門關鍵字
        /// 1 登入問候語
        /// 2 節日問候卡
        /// </summary>
        private int _category_id = 9;

        public GreetingWordsService(IGreetingWordsRepoAdapter adapter)
        {
            _adapter = adapter;            
        }

        ///// <summary>
        ///// 取得一筆資料
        ///// </summary>
        ///// <param name="id">系統流水編號</param>
        ///// <returns></returns>
        //public GreetingWordsDM GetInfo(int? id)
        //{

        //    try
        //    {
        //        TWNewEgg.Models.DomainModels.GreetingWords.GreetingWordsDM domain_info = null;
        //        TWNewEgg.Models.DBModels.TWSQLDB.GreetingWordsDB db_info = this._adapter.GetInfo(id);
        //        if (db_info != null)
        //        {
        //            domain_info = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.GreetingWords.GreetingWordsDM>(db_info);
        //            return domain_info;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// 依條件取得資料
        ///// </summary>       
        ///// <param name="description">文字描述</param>
        ///// <param name="codetext"> 
        ///// 自訂代碼文字
        ///// use for 登入問候語
        ///// </param>
        ///// <param name="showall">是否顯示, 1:顯示, 0:不顯示</param>
        ///// <returns></returns>
        //public List<GreetingWordsDM> GetData( string description, string codetext, int? showall)
        //{
        //    int category_id = _category_id;

        //    List<GreetingWordsDM> list_dm = null;
        //    List<GreetingWordsDB> list_db_result = null;
        //    try
        //    {
        //        IQueryable<GreetingWordsDB> list_db = this._adapter.GetData(category_id, description, codetext, showall);

        //        list_db_result = list_db.ToList();

        //        if (list_db_result != null)
        //            list_dm = ModelConverter.ConvertTo<List<GreetingWordsDM>>(list_db_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new NotImplementedException(ex.Message, ex);
        //    }

        //    return list_dm;

        //}




        ///// <summary>
        ///// 取得全部資料
        ///// </summary>
        ///// <returns></returns>
        //public List<GreetingWordsDM> GetAll()
        //{
        //    int category_id = _category_id;
        //    List<GreetingWordsDM> list_dm = null;
        //    List<GreetingWordsDB> list_db_result = null;
        //    try
        //    {
        //        IQueryable<GreetingWordsDB> list_db = this._adapter.GetAll()
        //            .Where(q => q.CategoryId == category_id);

        //        list_db_result = list_db.ToList();
        //        if (list_db_result != null)
        //            list_dm = ModelConverter.ConvertTo<List<GreetingWordsDM>>(list_db_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new NotImplementedException(ex.Message, ex);
        //    }
        //    return list_dm;
        //}


        ///// <summary>
        ///// 取得目前有效資料
        ///// </summary>       
        ///// <param name="now_date">
        ///// 目前時間
        ///// </param>
        ///// <returns></returns>
        //public List<GreetingWordsDM> GetShow(DateTime now_date)
        //{
        //    int category_id = _category_id;
        //    //DateTime now_date = DateTime.Now;
        //    List<GreetingWordsDM> list_dm = null;
        //    List<GreetingWordsDB> list_db_result = null;
        //    try
        //    {
        //        //Available
        //        //找尋顯示 && 時間內的
        //        IQueryable<GreetingWordsDB> list_db = this._adapter.GetAll()
        //            .Where(q => q.CategoryId == category_id && now_date >= q.StartDate && now_date <= q.EndDate)
        //            .OrderBy(q => q.Showorder);

        //        //db 2 list
        //        list_db_result = list_db.ToList();

        //        if (list_db_result != null && list_db_result.Count > 0)
        //        {
        //            //db model 2 domain model
        //            list_dm = ModelConverter.ConvertTo<List<GreetingWordsDM>>(list_db_result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new NotImplementedException(ex.Message, ex);
        //    }

        //    return list_dm;
        //}

        ///// <summary>
        ///// 修改資料
        ///// </summary>
        ///// <param name="info">domain model</param>
        ///// <returns></returns>
        //public bool Update(GreetingWordsDM info)
        //{

        //    bool is_ok = false;
        //    if (info == null) return is_ok;
        //    try
        //    {
        //        int category_id = _category_id;
        //        info.UpdateDate = DateTime.UtcNow.AddHours(8);
        //        GreetingWordsDB db_info = ModelConverter.ConvertTo<GreetingWordsDB>(info);
        //        this._adapter.Update(db_info);
        //        is_ok = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        is_ok = false;
        //        throw ex;
        //    }
        //    return is_ok;

        //}

        ///// <summary>
        ///// 建立資料
        ///// </summary>
        ///// <param name="info">domain model</param>
        ///// <returns></returns>
        //public GreetingWordsDM Add(GreetingWordsDM info)
        //{

        //    bool is_ok = false;
        //    //if (info == null) return is_ok;
        //    if (info == null) return info;
        //    try
        //    {
        //        int category_id = _category_id;
        //        info.CreateDate = DateTime.UtcNow.AddHours(8);
        //        GreetingWordsDB db_info = ModelConverter.ConvertTo<GreetingWordsDB>(info);
        //        db_info = this._adapter.Add(db_info);
        //        info = ModelConverter.ConvertTo<GreetingWordsDM>(db_info);

        //        return info;

        //        //is_ok = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        is_ok = false;
        //        throw ex;
        //    }
        //    //return is_ok;
        //}

        ///// <summary>
        ///// 建立資料
        ///// </summary>
        ///// <param name="list_info"></param>
        ///// <returns></returns>
        //public List<GreetingWordsDM> Add(List<GreetingWordsDM> list_info)
        //{
        //    List<GreetingWordsDM> list_result = new List<GreetingWordsDM>();
        //    if (list_info == null) return list_result;
        //    int category_id = _category_id;
        //    foreach (GreetingWordsDM each_info in list_info)
        //    {
        //        each_info.CreateDate = DateTime.UtcNow.AddHours(8);
        //        //dm 2 db
        //        GreetingWordsDB db_info = ModelConverter.ConvertTo<GreetingWordsDB>(each_info);
        //        this._adapter.Add(db_info);
        //        //list_result.Add(each_info);
        //    }
        //    return list_result;
        //}

        ///// <summary>
        ///// 刪除
        ///// </summary>
        ///// <param name="info">domain model</param>
        ///// <returns></returns>
        //public bool Del(GreetingWordsDM info)
        //{
        //    bool is_ok = false;
        //    if (info == null) return is_ok;
        //    try
        //    {
        //        GreetingWordsDB db_info = ModelConverter.ConvertTo<GreetingWordsDB>(info);
        //        this._adapter.Del(db_info);
        //        is_ok = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        is_ok = false;
        //        throw ex;
        //    }
        //    return is_ok;
        //}
    }
}
