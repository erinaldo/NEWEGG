using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.DeviceAdRepoAdapters.Interface
{
    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
    /// </summary>
    public interface IDeviceAdContentRepoAdapter
    {
        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        DeviceAdContentDB GetInfo(int? id);

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="deviceAdID">DeviceAdSet.ID</param>
        /// <param name="my_flag">phone=手機, pad=平版, pc=桌機</param>
        /// <param name="showall">顯示:show|1, 不顯示:hide|0</param>
        /// <returns></returns>
        IQueryable<DeviceAdContentDB> GetData(int deviceAdID, string my_flag, string showall);

        IQueryable<DeviceAdContentDB> GetAll();

        bool Update(DeviceAdContentDB info);

        DeviceAdContentDB Add(DeviceAdContentDB info);

        bool Del(DeviceAdContentDB info);
    }
}
