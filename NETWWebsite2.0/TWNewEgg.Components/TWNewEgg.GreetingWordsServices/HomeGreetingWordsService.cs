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

using TWNewEgg.Models.WebsiteMappingRules;

namespace TWNewEgg.GreetingWordsServices
{
    /// <summary>
    ///1 登入問候語 -----------------------add by bruce 20160329
    /// </summary>
    public class HomeGreetingWordsService : IHomeGreetingWordsService
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
        private int _category_id = 1;

        public HomeGreetingWordsService(IGreetingWordsRepoAdapter adapter)
        {
            _adapter = adapter;
        }

        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public HomeGreetingWordsDM GetSingle(int? id)
        {
            try
            {
                HomeGreetingWordsDM domain_info = null;
                var db_info = this._adapter.GetAll().FirstOrDefault(x => x.ID == id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<HomeGreetingWordsDM>(db_info);
                    return domain_info;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public HomeGreetingWordsDM GetInfo(int? id)
        {

            try
            {
                HomeGreetingWordsDM domain_info = null;
                GreetingWordsDB db_info = this._adapter.GetInfo(id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<HomeGreetingWordsDM>(db_info);
                    return domain_info;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="description">活動名稱</param>
        /// <param name="showall">是否顯示, 1:顯示, 0:不顯示</param>
        /// <returns></returns>
        public List<HomeGreetingWordsDM> GetData(string description, int? showall)
        {
            //int category_id = _category_id;

            //List<HomeGreetingWordsDM> list_dm = null;
            //List<GreetingWordsDB> list_db_result = null;
            //try
            //{
            //    string codetext = string.Empty;
            //    IQueryable<GreetingWordsDB> list_db = this._adapter.GetData(category_id, description, codetext, showall);

            //    list_db_result = list_db.ToList();

            //    if (list_db_result != null)
            //        list_dm = ModelConverter.ConvertTo<List<HomeGreetingWordsDM>>(list_db_result);
            //}
            //catch (Exception ex)
            //{
            //    throw new NotImplementedException(ex.Message, ex);
            //}

            //return list_dm;


            int category_id = _category_id;
            List<HomeGreetingWordsDM> list_dm = null;
            List<GreetingWordsDB> list_db_result = null;
            try
            {
                string codetext = string.Empty;
                IQueryable<GreetingWordsDB> list_db = this._adapter.GetAll()
                    .Where(x => x.CategoryId == category_id)
                    .OrderBy(x => x.Showorder);

                if (!string.IsNullOrEmpty(codetext))
                    list_db = list_db.Where(x => x.CodeText == codetext);

                if (showall != null)
                    list_db = list_db.Where(x => x.ShowAll == showall);

                list_db_result = list_db.ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<HomeGreetingWordsDM>>(list_db_result);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return list_dm;


        }

        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        public List<HomeGreetingWordsDM> GetAll()
        {
            int category_id = _category_id;
            List<HomeGreetingWordsDM> list_dm = null;
            List<GreetingWordsDB> list_db_result = null;
            try
            {
                IQueryable<GreetingWordsDB> list_db = this._adapter.GetAll()
                    .Where(x => x.CategoryId == category_id)
                    //.OrderBy(x => x.Showorder);
                    .OrderBy(x => x.ID);

                list_db_result = list_db.ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<HomeGreetingWordsDM>>(list_db_result);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return list_dm;
        }


        /// <summary>
        /// 取得目前有效資料
        /// </summary>       
        /// <param name="now_date">
        /// 目前時間
        /// </param>
        /// <returns></returns>
        public List<HomeGreetingWordsDM> GetShow(DateTime now_date)
        {
            int category_id = _category_id;
            //DateTime now_date = DateTime.Now;
            List<HomeGreetingWordsDM> list_dm = null;
            List<GreetingWordsDB> list_db_result = null;
            try
            {
                //Available
                //找尋顯示 && 時間內的
                IQueryable<GreetingWordsDB> list_db = this._adapter.GetAll()
                    .Where(x => x.CategoryId == category_id && now_date >= x.StartDate && now_date <= x.EndDate)
                    .OrderBy(x => x.Showorder);

                //db 2 list
                list_db_result = list_db.ToList();

                if (list_db_result != null && list_db_result.Count > 0)
                {
                    //db model 2 domain model
                    list_dm = ModelConverter.ConvertTo<List<HomeGreetingWordsDM>>(list_db_result);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return list_dm;
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        public bool Update(HomeGreetingWordsDM info, string user_name)
        {

            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {
                int category_id = _category_id;

                info.UpdateDate = DateTime.UtcNow.AddHours(8);

                info.UpdateUser = user_name;

                info.StartDate = new DateTime(info.StartDate.Year, info.StartDate.Month, info.StartDate.Day, 0, 0, 0);
                info.EndDate = new DateTime(info.EndDate.Year, info.EndDate.Month, info.EndDate.Day, 23, 59, 59);

                GreetingWordsDB db_info = ModelConverter.ConvertTo<GreetingWordsDB>(info);
                this._adapter.Update(db_info);
                is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            return is_ok;

        }

        private string change_path_to_pic(HomeGreetingWordsDM info)
        {
            //圖片儲存至Pic底下
            //string strfileName = System.Configuration.ConfigurationManager.AppSettings["HttpBlockImagePath"];
            //if (!string.IsNullOrEmpty(info.ImageUrl))
                //圖檔更改檔名存置資料庫
                //info.ImageUrl = strfileName + info.CategoryId + "_" + info.ID + "-" + info.Showorder + ".jpg";
                //info.ImageUrl = strfileName + "GreetingWorlds_" + info.CategoryId + "_" + info.ID + ".jpg";

            //將圖片的路徑增加時間戳記------------add by bruce 20160308
            //http://jira/browse/WMTWNOR-3104
            if (!string.IsNullOrEmpty(info.ImageUrl))
                info.ImageUrl = ImagesUrlChangerules.ImagesUrladdrandomResolver(info.ImageUrl);

            //bill說明,存入資料前轉小寫,與cdn圖檔機存入時一致--------------add by bruce 20160318
            //Info.ImageURL = Info.ImageURL.ToLower(); //又不要了XD------add by bruce 20160323

            //只有路徑轉小寫-----------------add by bruce 20160323
            info.ImageUrl = ImagesUrlChangerules.ImagesUrlPathToLower(info.ImageUrl);

            return info.ImageUrl;
        }

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        public HomeGreetingWordsDM Add(HomeGreetingWordsDM info, string user_name)
        {

            bool is_ok = false;
            //if (info == null) return is_ok;
            if (info == null) return info;
            try
            {

                int category_id = _category_id;
                info.CategoryId = category_id;

                info.CreateDate = DateTime.UtcNow.AddHours(8);
                info.CreateUser = user_name;

                info.UpdateDate = null;
                info.UpdateUser = string.Empty;

                if (string.IsNullOrEmpty(info.ImageUrl)) info.ImageUrl = string.Empty;
                info.ShowAll = 0;
                //info.Showorder = 0;

                //新增時計算並填入順序值----------------add by bruce 20160418
                var data =  this.GetAll();
                int show_order = 0;
                foreach (HomeGreetingWordsDM each_info in data)
                    show_order = each_info.Showorder + 1;
                if (show_order > 0) info.Showorder = show_order;
                if (info.Showorder == 0) info.Showorder = 1;
                //新增時計算並填入順序值----------------add by bruce 20160418

                if (string.IsNullOrEmpty(info.Description)) info.Description = string.Empty;
                info.CodeText = string.Empty;
                if (string.IsNullOrEmpty(info.Clickpath)) info.Clickpath = string.Empty;

                if (info.StartDate.Year == 1) info.StartDate = DateTime.Now;
                if (info.EndDate.Year == 1) info.EndDate = DateTime.Now;

                info.StartDate = new DateTime(info.StartDate.Year, info.StartDate.Month, info.StartDate.Day, 0, 0, 0);
                info.EndDate = new DateTime(info.EndDate.Year, info.EndDate.Month, info.EndDate.Day, 23, 59, 59);

                GreetingWordsDB db_info = ModelConverter.ConvertTo<GreetingWordsDB>(info);
                db_info = this._adapter.Add(db_info);
                info = ModelConverter.ConvertTo<HomeGreetingWordsDM>(db_info);

                return info;

                //is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            //return is_ok;
        }

        ///// <summary>
        ///// 建立資料
        ///// </summary>
        ///// <param name="list_info"></param>
        ///// <returns></returns>
        //public List<HomeGreetingWordsDM> Add(List<HomeGreetingWordsDM> list_info)
        //{
        //    List<HomeGreetingWordsDM> list_result = new List<HomeGreetingWordsDM>();
        //    if (list_info == null) return list_result;
        //    int category_id = _category_id;
        //    foreach (HomeGreetingWordsDM each_info in list_info)
        //    {
        //        each_info.CreateDate = DateTime.UtcNow.AddHours(8);
        //        //dm 2 db
        //        GreetingWordsDB db_info = ModelConverter.ConvertTo<GreetingWordsDB>(each_info);
        //        this._adapter.Add(db_info);
        //        //list_result.Add(each_info);
        //    }
        //    return list_result;
        //}

        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="list_info"></param>
        /// <returns>List<bool></returns>
        public List<bool> Save(List<HomeGreetingWordsDM> list_info, string user_name)
        {
            List<bool> list_result = new List<bool>();
            if (list_info == null) return list_result;
            int category_id = _category_id;
            foreach (HomeGreetingWordsDM each_info in list_info)
            {
                if (each_info.ID == 0)
                {
                    each_info.CreateDate = DateTime.UtcNow.AddHours(8);
                    HomeGreetingWordsDM info = null;
                    //DateTime now_date = DateTime.Now;
                    info = this.Add(each_info, user_name);
                    if (info != null) list_result.Add(true);
                }
                if (each_info.ID > 0)
                {
                    each_info.UpdateDate = DateTime.UtcNow.AddHours(8);
                    list_result.Add(this.Update(each_info, user_name));
                }
            }
            return list_result;
        }


        ///// <summary>
        ///// 刪除
        ///// </summary>
        ///// <param name="info">domain model</param>
        ///// <returns></returns>
        //public bool Del(HomeGreetingWordsDM info)
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

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        public bool Del(int? id)
        {
            bool is_ok = false;
            try
            {
                HomeGreetingWordsDM domain_info = null;
                var db_info = this._adapter.GetAll().FirstOrDefault(x => x.ID == id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<HomeGreetingWordsDM>(db_info);
                    this._adapter.Del(db_info);
                    is_ok = true;
                }
                else
                    is_ok = false;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            return is_ok;
        }
    }
}
