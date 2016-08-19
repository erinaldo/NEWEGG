
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
    public class DeviceAdContentController : ApiController
    {
        /// <summary>
        /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160621
        /// 1006	廣告一
        /// 1007	廣告二
        /// 1008	廣告三
        /// 1009	廣告四
        /// 1010	廣告五
        /// 1011	廣告六
        /// 
        /// 1012	生活提案一
        /// 1013	生活提案二
        /// 1014	生活提案三
        /// 1015	生活提案四
        /// 
        /// 1016	全館分類設定
        /// 1017	促案設定
        /// 1018	左方區塊
        /// 1019	右方區塊
        /// 
        /// 1020	手機版櫥窗輪播
        /// 1021	手機版分類輪播
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM>
        /// </returns>
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        // GET api/DeviceAdContent/1007
        //public List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM> Get(int id = 1007)
        public TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM Get(int id = 1007)
        {
            TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM result = new TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM();
            try
            {
                result = GetData(id);
            }
            catch (Exception ex) { }
            return result;
        }


        // POST api/DeviceAdContent
        public void Post([FromBody]string value)
        {
        }

        // PUT api/DeviceAdContent/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/DeviceAdContent/5
        public void Delete(int id)
        {
        }

        private TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM GetData(int id)
        {
            //JsonResult json = new JsonResult();

            TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM data_result = new TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM();
            try
            {

                int device_id = 0;
                //if (string.IsNullOrEmpty(id)) id = device_id.ToString();
                //bool is_number = int.TryParse(id, out device_id);
                device_id = id;

                TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM search_container = null;
                //取得廣告內文
                search_container = new TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM();
                search_container.DeviceAdSetIDs.Add(device_id);
                search_container.ShowAll = "show";

                var info_result = TWNewEgg.Framework.ServiceApi.Processor.Request<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM
                    , TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdShowDM>("DeviceAdContentService", "GetShow", search_container);

                data_result.AdSet = info_result.results.AdSet;
                data_result.ListAdContent = info_result.results.ListAdContent;

                //json = Json(infodata, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //json = Json(ex, JsonRequestBehavior.AllowGet);
            }

            return data_result;

            //return json;

        }


    }
}
