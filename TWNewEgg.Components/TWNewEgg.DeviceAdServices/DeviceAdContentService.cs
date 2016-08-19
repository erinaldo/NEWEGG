using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.WebsiteMappingRules;
using TWNewEgg.Models.DBModels.TWSQLDB;

using TWNewEgg.DeviceAdServices.Interface;
using TWNewEgg.DeviceAdRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.DeviceAd;

namespace TWNewEgg.DeviceAdServices
{

    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
    /// 行動設備的廣告內文
    /// </summary>
    public class DeviceAdContentService : IDeviceAdContentService
    {
        private IDeviceAdContentRepoAdapter _adapter; //行動設備的廣告內文
        private IDeviceAdSetService _IDeviceAdSetService; //行動設備的廣告設定

        public DeviceAdContentService(IDeviceAdContentRepoAdapter myAdapter, IDeviceAdSetService myIDeviceAdSetService)
        {
            ///qqqqqqqqqqqqqqqqqqq
            this._adapter = myAdapter;
            this._IDeviceAdSetService = myIDeviceAdSetService;
        }

        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="list_info"></param>
        /// <returns>List<bool></returns>
        public List<bool> Save(List<DeviceAdContentDM> list_info, string user_name)
        {
            List<bool> list_result = new List<bool>();
            if (list_info == null) return list_result;

            foreach (DeviceAdContentDM each_info in list_info)
            {
                if (each_info.ID == 0)
                {
                    DeviceAdContentDM info = null;
                    info = this.Add(each_info, user_name);
                    if (info != null) list_result.Add(true);
                }
                if (each_info.ID > 0)
                {
                    list_result.Add(this.Update(each_info, user_name));
                }
            }
            return list_result;
        }

        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public DeviceAdContentDM GetInfo(int? id)
        {

            try
            {
                DeviceAdContentDM domain_info = null;
                TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdContentDB db_info = this._adapter.GetInfo(id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<DeviceAdContentDM>(db_info);
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
        /// 取得要維護的資料
        /// </summary>
        /// <param name="search_info">行動設備的廣告資料查詢DeviceAdSearchDM</param>
        /// <returns>DeviceAdDataDM</returns>
        public DeviceAdEditDM GetEdit(DeviceAdSearchDM search_info)
        {
            DeviceAdEditDM rtn_info = new DeviceAdEditDM();
            try
            {
                //search_info.DescByCreateDate = false;
                //search_info.IsBetweenDate = false;
                rtn_info.ListAdContent = this.GetData(search_info);
                foreach (int deviceAdSetID in search_info.DeviceAdSetIDs)
                    rtn_info.AdSet = this._IDeviceAdSetService.GetInfo(deviceAdSetID);

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return rtn_info;
        }


        /// <summary>
        /// 取得資料
        /// EC使用
        /// </summary>
        /// <param name="search_info">行動設備的廣告資料查詢DeviceAdSearchDM</param>
        /// <returns></returns>
        public DeviceAdShowDM GetShow(DeviceAdSearchDM search_info)
        {
            DeviceAdShowDM rtn_info = new DeviceAdShowDM();
            try
            {
                search_info.ShowAll = "show";
                //search_info.DescByCreateDate = true; //DescByCreateDate -------------------add by bruce 20160616
                //search_info.IsBetweenDate = true;                
                rtn_info.ListAdContent = this.GetData(search_info);
                foreach (int deviceAdSetID in search_info.DeviceAdSetIDs)
                    rtn_info.AdSet = this._IDeviceAdSetService.GetInfo(deviceAdSetID);

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return rtn_info;
        }


        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="search_info">行動設備的廣告資料查詢DeviceAdSearchDM</param>
        /// <returns></returns>
        public List<DeviceAdContentDM> GetData(DeviceAdSearchDM search_info)
        {
            List<DeviceAdContentDM> list_dm = new List<DeviceAdContentDM>();
            List<TWNewEgg.Models.DBModels.TWSQLDB.DeviceAdContentDB> list_db_result = null;

            try
            {
                if (search_info == null) return list_dm;

                IQueryable<DeviceAdContentDB> list_db = this._adapter.GetAll();

                list_db = list_db.Where(x => x.Flag != "del");

                if (!string.IsNullOrEmpty(search_info.ShowAll))
                {
                    string showall = search_info.ShowAll.ToLower();
                    if (showall == "1") showall = "show";
                    if (showall == "0") showall = "hide";
                    search_info.ShowAll = showall;
                }
                else
                    search_info.ShowAll = "show";

                if (search_info.ShowAll == "show")
                    list_db = list_db.Where(x => x.ShowAll == "1" || x.ShowAll == "show");

                if (search_info.ShowAll == "hide")
                    list_db = list_db.Where(x => x.ShowAll == "0" || x.ShowAll == "hide");

                //在有效的日期內的資料---------------------add by bruce 20160616
                if (search_info.IsBetweenDate)
                {
                    DateTime now_date = DateTime.Now;
                    list_db = list_db.Where(x => x.StartDate <= now_date && x.EndDate >= now_date);
                }

                foreach (int each_id in search_info.DeviceAdSetIDs)
                    list_db = list_db.Where(x => x.DeviceAdSetID == each_id);


                /// ---------------------------------------------add by bruce 20160623
                /// 來自SubCategory_NormalStore與Category的ID
                /// 若有值DeviceAdSetID會為0
                foreach (int each_id in search_info.CategoryIDs)
                    list_db = list_db.Where(x => x.CategoryID == each_id);

                
                /// ---------------------------------------------add by bruce 20160623
                /// 若CategoryID有值這裡代表是屬於這個CategoryID的index
                //if (search_info.CategoryIDs.Count > 0)
                //{
                //    if (!string.IsNullOrEmpty(search_info.Flag))
                //        list_db = list_db.Where(x => x.Flag == search_info.Flag);
                //}

                if (!string.IsNullOrEmpty(search_info.Flag))
                    list_db = list_db.Where(x => x.Flag == search_info.Flag);

                //排序Showorder
                if (search_info.OrderByShoworder)
                    list_db = list_db.OrderBy(x => x.Showorder);

                //以建檔日期排序降序-------------------add by bruce 20160616
                if (search_info.DescByCreateDate)
                    list_db = list_db.OrderByDescending(x => x.CreateDate);

                list_db_result = list_db.ToList();

                //資料最大筆數--------------------------------add by bruce 20160622
                if (search_info.MaxCount > 0)
                {
                    int max_count = search_info.MaxCount;
                    int data_count = 0;

                    list_db_result = new List<DeviceAdContentDB>();
                    foreach (DeviceAdContentDB each_info in list_db.ToList())
                    {
                        list_db_result.Add(each_info);
                        if (list_db_result.Count >= max_count) break;
                        //break; //取一筆就好
                        data_count += 1;
                    }
                }
                //資料最大筆數--------------------------------add by bruce 20160622


                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<DeviceAdContentDM>>(list_db_result);
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
        public List<DeviceAdContentDM> GetAll()
        {
            List<DeviceAdContentDM> list_dm = null;
            List<DeviceAdContentDB> list_db_result = null;
            try
            {
                IQueryable<DeviceAdContentDB> list_db = this._adapter.GetAll();
                list_db_result = list_db.OrderBy(x => x.Showorder).ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<DeviceAdContentDM>>(list_db_result);
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
        public bool Update(DeviceAdContentDM info, string user_name)
        {

            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {
                info.UpdateDate = DateTime.UtcNow.AddHours(8);

                info.UpdateUser = user_name;

                info.StartDate = new DateTime(info.StartDate.Year, info.StartDate.Month, info.StartDate.Day, 0, 0, 0);
                info.EndDate = new DateTime(info.EndDate.Year, info.EndDate.Month, info.EndDate.Day, 23, 59, 59);

                //info.ImageUrl = change_path_to_pic(info);

                //將圖片的路徑增加時間戳記------------add by bruce 20160308
                //http://jira/browse/WMTWNOR-3104
                if (!string.IsNullOrEmpty(info.ImageUrl))
                    info.ImageUrl = ImagesUrlChangerules.ImagesUrladdrandomResolver(info.ImageUrl);

                //bill說明,存入資料前轉小寫,與cdn圖檔機存入時一致--------------add by bruce 20160318
                //Info.ImageURL = Info.ImageURL.ToLower(); //又不要了XD------add by bruce 20160323

                //只有路徑轉小寫-----------------add by bruce 20160323
                if (!string.IsNullOrEmpty(info.ImageUrl))
                    info.ImageUrl = ImagesUrlChangerules.ImagesUrlPathToLower(info.ImageUrl);

                DeviceAdContentDB db_info = ModelConverter.ConvertTo<DeviceAdContentDB>(info);

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

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">DeviceAdContentDM</param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public DeviceAdContentDM Add(DeviceAdContentDM info, string user_name)
        {

            bool is_ok = false;
            //if (info == null) return is_ok;
            if (info == null) return info;
            try
            {

                info.CreateDate = DateTime.UtcNow.AddHours(8);
                info.CreateUser = user_name;
                info.UpdateDate = null;
                info.UpdateUser = string.Empty;
                if (string.IsNullOrEmpty(info.ImageUrl)) info.ImageUrl = string.Empty;
                info.ShowAll = "show";
                //info.Showorder = 0;
                if (string.IsNullOrEmpty(info.Name)) info.Name = string.Empty;
                if (string.IsNullOrEmpty(info.Name2)) info.Name2 = string.Empty;
                if (string.IsNullOrEmpty(info.Clickpath)) info.Clickpath = string.Empty;

                if (info.StartDate.Year == 1) info.StartDate = DateTime.Now;
                if (info.EndDate.Year == 1) info.EndDate = DateTime.Now;

                info.StartDate = new DateTime(info.StartDate.Year, info.StartDate.Month, info.StartDate.Day, 0, 0, 0);
                info.EndDate = new DateTime(info.EndDate.Year, info.EndDate.Month, info.EndDate.Day, 23, 59, 59);

                //info.ImageUrl = change_path_to_pic(info);

                //將圖片的路徑增加時間戳記------------add by bruce 20160308
                //http://jira/browse/WMTWNOR-3104
                if (!string.IsNullOrEmpty(info.ImageUrl))
                    info.ImageUrl = ImagesUrlChangerules.ImagesUrladdrandomResolver(info.ImageUrl);

                //bill說明,存入資料前轉小寫,與cdn圖檔機存入時一致--------------add by bruce 20160318
                //Info.ImageURL = Info.ImageURL.ToLower(); //又不要了XD------add by bruce 20160323

                //只有路徑轉小寫-----------------add by bruce 20160323
                if (!string.IsNullOrEmpty(info.ImageUrl))
                    info.ImageUrl = ImagesUrlChangerules.ImagesUrlPathToLower(info.ImageUrl);

                DeviceAdContentDB db_info = ModelConverter.ConvertTo<DeviceAdContentDB>(info);
                db_info = this._adapter.Add(db_info);
                info = ModelConverter.ConvertTo<DeviceAdContentDM>(db_info);

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

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Del(int? id)
        {
            bool is_ok = false;
            try
            {
                DeviceAdContentDM domain_info = null;
                var db_info = this._adapter.GetAll().FirstOrDefault(x => x.ID == id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<DeviceAdContentDM>(db_info);
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


        public bool Empty()
        {
            bool is_ok = false;

            try
            {

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return is_ok;
        }





    }
}
