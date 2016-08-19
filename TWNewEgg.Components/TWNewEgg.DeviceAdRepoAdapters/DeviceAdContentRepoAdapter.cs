using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

using TWNewEgg.DeviceAdRepoAdapters.Interface;

namespace TWNewEgg.DeviceAdRepoAdapters
{
    /// <summary>
    /// DB橋接GreetingWords-------------add by bruce 20160330
    /// </summary>
    public class DeviceAdContentRepoAdapter : IDeviceAdContentRepoAdapter
    {
        private IRepository<DeviceAdContentDB> _list_db;

        public DeviceAdContentRepoAdapter(IRepository<DeviceAdContentDB> list_info)
        {
            _list_db = list_info;
        }

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public DeviceAdContentDB GetInfo(int? id)
        {
            //DeviceAdContentDB info;
            //if (id == null) return null;
            var info = _list_db.Get(x => x.ID == id);
            return info;
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="deviceAdID">DeviceAdSet.ID</param>
        /// <param name="my_flag">phone=手機, pad=平版, pc=桌機</param>
        /// <param name="showall">顯示:show|1, 不顯示:hide|0</param>
        /// <returns></returns>
        public IQueryable<DeviceAdContentDB> GetData(int deviceAdID, string my_flag, string showall)
        {
            //IQueryable<DeviceAdContentDB> list_info = _list_db.GetAll().Where(x => x.CategoryId == category_id);

            var list_info = _list_db.GetAll();

            if (!string.IsNullOrEmpty(my_flag))
                list_info = list_info.Where(x => x.Flag == my_flag);

            if (!string.IsNullOrEmpty(showall))
                list_info = list_info.Where(x => x.ShowAll == showall);

            //if (deviceAdID > 0)
            //    list_info = list_info.Where(x => x.DeviceAdID == deviceAdID);

            return list_info;

        }

        public IQueryable<DeviceAdContentDB> GetAll()
        {
            //IQueryable<DeviceAdContentDB> list_info = _list_db.GetAll();
            var list_info = _list_db.GetAll();
            return list_info;
        }

        public bool Update(DeviceAdContentDB info)
        {
            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {
                _list_db.Update(info);
                is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            return is_ok;

        }

        public DeviceAdContentDB Add(DeviceAdContentDB info)
        {
            bool is_ok = false;
            if (info == null) return info;
            try
            {
                _list_db.Create(info);
                is_ok = true;
            }
            catch (Exception ex)
            {
                is_ok = false;
                throw ex;
            }
            return info;
        }

        public bool Del(DeviceAdContentDB info)
        {
            bool is_ok = false;
            if (info == null) return is_ok;
            try
            {
                _list_db.Delete(info);
                is_ok = true;
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
