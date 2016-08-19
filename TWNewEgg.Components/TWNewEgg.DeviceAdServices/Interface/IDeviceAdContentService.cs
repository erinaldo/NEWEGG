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
    /// 行動設備的廣告內文
    /// </summary>
    public interface IDeviceAdContentService
    {
        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="list_info"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        List<bool> Save(List<DeviceAdContentDM> list_info, string user_name);

        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">系統流水編號</param>
        /// <returns></returns>
        DeviceAdContentDM GetInfo(int? id);

         /// <summary>
        /// 取得要維護的資料
         /// </summary>
        /// <param name="info">DeviceAdSearchDM</param>
        /// <returns>DeviceAdDataDM</returns>
        DeviceAdEditDM GetEdit(DeviceAdSearchDM info);

        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="info">DeviceAdSearchDM</param>
        /// <returns></returns>
        //List<DeviceAdContentDM> GetShow(DeviceAdSearchDM info);
        DeviceAdShowDM GetShow(DeviceAdSearchDM info);


        /// <summary>
        /// 依條件取得資料
        /// </summary>
        /// <param name="info">DeviceAdSearchDM</param>
        /// <returns></returns>
        List<DeviceAdContentDM> GetData(DeviceAdSearchDM info);

        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        List<DeviceAdContentDM> GetAll();

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool Update(DeviceAdContentDM info, string user_name);

        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="info">domain model</param>
        /// <returns></returns>
        DeviceAdContentDM Add(DeviceAdContentDM info, string user_name);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Del(int? id);

    }
}
