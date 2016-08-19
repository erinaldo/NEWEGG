using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.DeviceAd;

namespace TWNewEgg.DeviceAdServices.Interface
{
    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
    /// 行動設備的廣告設定
    /// </summary>
    public interface IDeviceAdSetService
    {
        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        DeviceAdSetDM GetInfo(int? id);

        /// <summary>
        /// 取得左選單或子選單
        /// </summary>
        /// <param name="info">DeviceAdSetDM</param>
        /// <returns>DeviceAdMenuDM</returns>
        DeviceAdMenuDM GetMenu(DeviceAdSearchDM info);

        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        List<DeviceAdSetDM> GetShow(DeviceAdSearchDM info);

        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        List<DeviceAdSetDM> GetData(DeviceAdSearchDM info);

        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        List<DeviceAdSetDM> GetAll();

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool Update(DeviceAdSetDM info, string user_name);

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        DeviceAdSetDM Add(DeviceAdSetDM info, string user_name);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Del(int? id);

    }
}
