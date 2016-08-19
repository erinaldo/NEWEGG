using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.DeviceAdRepoAdapters.Interface
{
    public interface IDeviceAdSetRepoAdapter
    {
        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        DeviceAdSetDB GetInfo(int? id);

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="my_parent">DeviceAdSet.ID</param>
        /// <param name="my_flag">phone=手機, pad=平版, pc=桌機</param>
        /// <param name="showall">顯示:show|1, 不顯示:hide|0</param>
        /// <returns></returns>
        IQueryable<DeviceAdSetDB> GetData(int my_parent, string my_flag, string showall);

        IQueryable<DeviceAdSetDB> GetAll();

        bool Update(DeviceAdSetDB info);

        DeviceAdSetDB Add(DeviceAdSetDB info);

        bool Del(DeviceAdSetDB info);
    }
}
