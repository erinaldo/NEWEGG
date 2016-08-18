
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels.DeviceAd;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    public class DeviceAdMenuController : ApiController
    {
        /// <summary>
        /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160621
        /// 1000	手機首頁輪播
        /// 1001	生活提案-大圖
        /// 1002	全館分類
        /// 1003	促案
        /// 1004	美國直購
        /// 1005	各館輪播
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdSetVM>
        /// </returns>
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        // GET api/DeviceAdSet/1000
        public TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdMenuVM Get(int id = 1000)
        {
            TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdMenuVM result = new TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdMenuVM();
            try
            {
                result = GetData(id);
            }
            catch (Exception ex) { }
            return result;
        }


        // POST api/DeviceAdSet
        public void Post([FromBody]string value)
        {
        }

        // PUT api/DeviceAdSet/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/DeviceAdSet/5
        public void Delete(int id)
        {
        }

        private TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdMenuVM GetData(int id)
        {
            //JsonResult json = new JsonResult();
            TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdMenuVM data_result = new TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdMenuVM();

            try
            {
                int device_id = 0;
                //if (string.IsNullOrEmpty(id)) id = device_id.ToString();
                //bool is_number = int.TryParse(id, out device_id);
                device_id = id;

                TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM search_container = null;
                //取得廣告目錄
                search_container = new TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM();
                search_container.DeviceAdSetIDs.Add(device_id);
                search_container.ShowAll = "show";

                //取得資料
                var list_result = Processor.Request<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdMenuVM
                   , TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdMenuDM>("DeviceAdSetService", "GetMenu", search_container);

                data_result = list_result.results;

                //json = Json(listdata, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            { }
            return data_result;


            //return json;

        }

    }
}
