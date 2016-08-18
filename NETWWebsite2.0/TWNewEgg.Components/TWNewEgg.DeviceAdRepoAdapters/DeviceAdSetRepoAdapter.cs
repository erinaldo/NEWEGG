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
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
    /// DB橋接
    /// </summary>
    public class DeviceAdSetRepoAdapter : IDeviceAdSetRepoAdapter
    {
        private IRepository<DeviceAdSetDB> _list_db;

        public DeviceAdSetRepoAdapter(IRepository<DeviceAdSetDB> list_info)
        {
            _list_db = list_info;
        }

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        public DeviceAdSetDB GetInfo(int? id)
        {
            //DeviceAdSetDB info;
            //if (id == null) return null;
            var info = _list_db.Get(x => x.ID == id);
            return info;
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="my_parent">DeviceAdSet.ID</param>
        /// <param name="my_flag">phone=手機, pad=平版, pc=桌機</param>
        /// <param name="showall">顯示:show|1, 不顯示:hide|0</param>
        /// <returns></returns>
        public IQueryable<DeviceAdSetDB> GetData(int my_parent, string my_flag, string showall)
        {
            var list_info = this.GetAll();

            if (my_parent > 0)
                list_info = list_info.Where(x => x.Parent == my_parent);

            if (!string.IsNullOrEmpty(my_flag))
                list_info = list_info.Where(x => x.Flag == my_flag);

            if (!string.IsNullOrEmpty(showall))
                list_info = list_info.Where(x => x.ShowAll == showall);            

            return list_info;

        }

        public IQueryable<DeviceAdSetDB> GetAll()
        {
            //IQueryable<DeviceAdSetDB> list_info = _list_db.GetAll();
            var list_info = _list_db.GetAll();
            return list_info;
        }

        public bool Update(DeviceAdSetDB info)
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

        public DeviceAdSetDB Add(DeviceAdSetDB info)
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

        public bool Del(DeviceAdSetDB info)
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
