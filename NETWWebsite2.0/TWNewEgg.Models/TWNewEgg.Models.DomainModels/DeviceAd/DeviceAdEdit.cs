using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
namespace TWNewEgg.Models.DomainModels.DeviceAd
{
    /// <summary>
    /// IPP維護
    /// 行動設備首頁廣告的子選單下的廣告資料
    /// </summary>
    public class DeviceAdEditDM
    {
        //行動設備的廣告設定
        public DeviceAdSetDM AdSet = new DeviceAdSetDM();
        //行動設備的廣告內文
        public List<DeviceAdContentDM> ListAdContent = new List<DeviceAdContentDM>();
    }

}
