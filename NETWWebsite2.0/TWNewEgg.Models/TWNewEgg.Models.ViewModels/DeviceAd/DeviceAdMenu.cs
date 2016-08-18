using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
namespace TWNewEgg.Models.ViewModels.DeviceAd
{
    /// <summary>
    /// 行動設備首頁廣告的一個左方選單
    /// </summary>
    public class DeviceAdMenuVM
    {
        //行動設備的廣告設定
        public DeviceAdSetVM AdSet = new DeviceAdSetVM();
        //左方選單下的子選單
        public List<DeviceAdSetVM> ListAdMenu = new List<DeviceAdSetVM>();
    }

}
