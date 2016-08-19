using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160615
namespace TWNewEgg.Models.ViewModels.DeviceAd
{
    /// <summary>
    /// 行動設備首頁廣告的子選單下的廣告資料
    /// EC用顯示用
    /// </summary>
    public class DeviceAdShowVM
    {
        //行動設備的廣告設定
        public DeviceAdSetVM AdSet = new DeviceAdSetVM();
        //行動設備的廣告內文
        public List<DeviceAdContentVM> ListAdContent = new List<DeviceAdContentVM>();
    }

}
