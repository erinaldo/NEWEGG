
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
    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160621
    /// 1002	全館分類
    /// </summary>
    public class HomeCategoryMapController : ApiController
    {
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        // GET api/HomeCategoryMap
        public List<TWNewEgg.Models.ViewModels.DeviceAd.HomeCategoryMapVM> Get()
        {
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeCategoryMapVM> result = null;
            result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeCategoryMapVM>();
            try
            {
                result = get_data();
            }
            catch (Exception ex) { }
            return result;
        }

        private List<TWNewEgg.Models.ViewModels.DeviceAd.HomeCategoryMapVM> get_data()
        {
            //JsonResult json = new JsonResult();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeCategoryMapVM> list_result = null;
            list_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeCategoryMapVM>();

            List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM> data_result = null;
            data_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM>();

            try
            {
                /// 1016	全館分類設定
                TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM search_info = null;
                //取得廣告內文
                search_info = new TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM();
                search_info.OrderByShoworder = true;
                search_info.IsBetweenDate = false; //起迄日期不作查詢條件-----------add by bruce 20160704
                //search_info.MaxCount = 15;

                int[] device_ids = new int[] { 1016 };

                /// 1016	全館分類設定
                foreach (int device_id in device_ids)
                {
                    search_info.DeviceAdSetIDs.Clear();
                    search_info.DeviceAdSetIDs.Add(device_id);
                    var info_result = TWNewEgg.Framework.ServiceApi.Processor.Request<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdShowVM
                    , TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdShowDM>("DeviceAdContentService", "GetShow", search_info);

                    foreach (TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM each_info in info_result.results.ListAdContent)
                    {
                        data_result.Add(each_info);
                        //break; //取一筆就好
                    }

                }

                //轉成client 需要的json model
                //https://bitbucket.org/vincent0406/mobile-newegg/src/36f661ad362b6b9c7cdc6098121c31e4a4eaec7c/app/jsx/actions/home/BannerSlideAction.jsx?at=react-dev&fileviewer=file-view-default
                list_result = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<TWNewEgg.Models.ViewModels.DeviceAd.HomeCategoryMapVM>>(data_result);


                //json = Json(infodata, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //json = Json(ex, JsonRequestBehavior.AllowGet);
            }

            //return data_result;
            return list_result;

            //return json;

        }


        // POST api/
        public void Post([FromBody]string value) { }

        // PUT api/
        public void Put(int id, [FromBody]string value) { }

        // DELETE api/
        public void Delete(int id) { }

        


    }
}
