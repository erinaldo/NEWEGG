using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TWNewEgg.Framework.AutoMapper;

using TWNewEgg.Models.DBModels.TWSQLDB;

using TWNewEgg.DeviceAdServices.Interface;
using TWNewEgg.DeviceAdRepoAdapters.Interface;

using TWNewEgg.Models.DomainModels.DeviceAd;

namespace TWNewEgg.DeviceAdServices
{
    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
    /// 行動設備的廣告設定
    /// </summary>
    public class DeviceAdSetService : IDeviceAdSetService
    {
        private IDeviceAdSetRepoAdapter _adapter; //行動設備的廣告設定

        public DeviceAdSetService(IDeviceAdSetRepoAdapter adapter)
        {
            _adapter = adapter;
        }

        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public DeviceAdSetDM GetInfo(int? id)
        {


            try
            {
                DeviceAdSetDM domain_info = null;
                DeviceAdSetDB db_info = this._adapter.GetInfo(id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<DeviceAdSetDM>(db_info);
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
        /// 取得左選單或子選單
        /// </summary>
        /// <param name="search_info">DeviceAdSetDM</param>
        /// <returns>DeviceAdMenuDM</returns>
        public DeviceAdMenuDM GetMenu(DeviceAdSearchDM search_info)
        {
            DeviceAdMenuDM rtn_info = new DeviceAdMenuDM();
            try
            {
                rtn_info.ListAdMenu = this.GetData(search_info);
                foreach (int deviceAdSetID in search_info.DeviceAdSetIDs) rtn_info.AdSet = this.GetInfo(deviceAdSetID);
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
        /// <param name="search_info">DeviceAdSetDM
        /// 1000	輪播
        /// 1001	生活提案-大圖
        /// 1002	全館分類
        /// 1003	促案
        /// 1004	美國直購
        /// </param>
        /// <returns></returns>
        public List<DeviceAdSetDM> GetShow(DeviceAdSearchDM search_info)
        {
            DeviceAdMenuDM rtn_info = new DeviceAdMenuDM();
            List<DeviceAdSetDM> list_dm = new List<DeviceAdSetDM>();
            try
            {
                rtn_info.ListAdMenu = this.GetData(search_info);
                foreach (int deviceAdSetID in search_info.DeviceAdSetIDs) rtn_info.AdSet = this.GetInfo(deviceAdSetID);
                //rtn_info = this.GetMenu(search_info);
                list_dm = rtn_info.ListAdMenu;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return list_dm;
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="search_info">DeviceAdSetDM</param>
        /// <returns></returns>
        public List<DeviceAdSetDM> GetData(DeviceAdSearchDM search_info)
        {

            List<DeviceAdSetDM> list_dm = null;
            List<DeviceAdSetDB> list_db_result = null;
            try
            {
                if (search_info == null) return list_dm;

                IQueryable<DeviceAdSetDB> list_db = this._adapter.GetAll();

                /// 用在查詢目錄時是, phone=手機, pad=平版, pc=桌機
                /// 用在查詢子目錄時是, 填空值
                if (!string.IsNullOrEmpty(search_info.Flag))
                    search_info.Flag = search_info.Flag.Trim().ToLower();
                else
                    search_info.Flag = string.Empty;

                if (!string.IsNullOrEmpty(search_info.Flag))
                    list_db = list_db.Where(x => x.Flag == search_info.Flag);

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

                foreach (int deviceAdSetID in search_info.DeviceAdSetIDs)
                    list_db = list_db.Where(x => x.Parent == deviceAdSetID);

                //排序Showorder
                list_db = list_db.OrderBy(x => x.Showorder);

                //以建檔日期排序降序-------------------add by bruce 20160616
                if (search_info.DescByCreateDate)
                    list_db = list_db.OrderByDescending(x => x.CreateDate);

                list_db_result = list_db.ToList();

                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<DeviceAdSetDM>>(list_db_result);

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
        /// <returns>List<DeviceAdSetDM></returns>
        public List<DeviceAdSetDM> GetAll()
        {
            List<DeviceAdSetDM> list_dm = null;
            List<DeviceAdSetDB> list_db_result = null;
            try
            {
                IQueryable<DeviceAdSetDB> list_db = this._adapter.GetAll();
                list_db_result = list_db.OrderBy(x => x.Showorder).ToList();
                if (list_db_result != null)
                    list_dm = ModelConverter.ConvertTo<List<DeviceAdSetDM>>(list_db_result);
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
        /// <param name="info">DeviceAdSetDM</param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public bool Update(DeviceAdSetDM info, string user_name)
        {

            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {
                info.UpdateDate = DateTime.UtcNow.AddHours(8);
                info.UpdateUser = user_name;

                if (string.IsNullOrEmpty(info.ShowAll))
                    info.ShowAll = "show";
                else
                    info.ShowAll = info.ShowAll.ToLower();

                DeviceAdSetDB db_info = ModelConverter.ConvertTo<DeviceAdSetDB>(info);
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
        /// <param name="info">DeviceAdSetDM</param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public DeviceAdSetDM Add(DeviceAdSetDM info, string user_name)
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

                if (string.IsNullOrEmpty(info.ShowAll))
                    info.ShowAll = "show";
                else
                    info.ShowAll = info.ShowAll.ToLower();

                DeviceAdSetDB db_info = ModelConverter.ConvertTo<DeviceAdSetDB>(info);
                db_info = this._adapter.Add(db_info);
                info = ModelConverter.ConvertTo<DeviceAdSetDM>(db_info);

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
                DeviceAdSetDM domain_info = null;
                var db_info = this._adapter.GetAll().FirstOrDefault(x => x.ID == id);
                if (db_info != null)
                {
                    domain_info = ModelConverter.ConvertTo<DeviceAdSetDM>(db_info);
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
