
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
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160622
    /// 1021	手機版分類頁輪播
    /// </summary>
    public class HomeSubStoreSliderController : ApiController
    {

        //[AllowNonSecures]
        //[AllowAnonymous]
        //[HttpGet]
        //// GET api/HomeSubStoreSlider
        //public List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM> Get()
        //{
        //    List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM> result = null;
        //    result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM>();
        //    try
        //    {
        //        result = get_data();
        //    }
        //    catch (Exception ex) { }
        //    return result;
        //}

        ///737	美國新蛋直購	1
        ///2509	尋找禮品	2
        ///1279	設計風尚	3
        ///734	國際名品	4
        ///1929	美妝保養	5
        ///1930	保健養生	6
        ///736	樂活食尚	7
        ///735	運動健身	8
        ///7	戶外休旅	9
        ///1928	親子寵物	10
        ///6	居家用品	11
        ///3	生活家電	12
        ///5	服飾配件	13
        ///264	數位3C	14
        ///1	電腦週邊	15
        ///2505	世界城市好好逛	16
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        public List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM> Get([FromUri]List<int> categoryids)
        {
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM> result = null;
            result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM>();
            try
            {
                result = get_data(categoryids);
            }
            catch (Exception ex) { }
            return result;
        }

        private List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM> get_data(List<int> categoryids)
        {
            //JsonResult json = new JsonResult();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM> list_result = null;
            list_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM>();

            List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM> data_result = null;
            data_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM>();

            try
            {

                TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM search_info = null;
                //取得廣告內文
                search_info = new TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM();
                //search_info.DescByCreateDate = true; //DescByCreateDate -------------------add by bruce 20160616
                search_info.IsBetweenDate = false; //手機版分類頁輪播不需日期起迄檢查-------------add by bruce 20160705
                search_info.OrderByShoworder = true;
                //search_info.DescByCreateDate = true;
                search_info.MaxCount = 5;

                int[] device_ids = new int[] { 1021 };

                int[] category_ids = new int[] { 737 }; //美國新蛋直購

                if (categoryids.Count > 0) 
                    category_ids = categoryids.ToArray<int>(); //-------------------add by bruce 20160616

                foreach (int each_id in category_ids)
                {
                    search_info.DeviceAdSetIDs = device_ids.ToList<int>();
                    search_info.CategoryIDs.Clear();
                    search_info.CategoryIDs.Add(each_id);

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
                list_result = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM>>(data_result);

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

        private List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM> get_data()
        {
            //JsonResult json = new JsonResult();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM> list_result = null;
            list_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM>();

            List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM> data_result = null;
            data_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM>();

            try
            {

                TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM search_info = null;
                //取得廣告內文
                search_info = new TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM();
                //search_info.DescByCreateDate = true; //DescByCreateDate -------------------add by bruce 20160616
                search_info.IsBetweenDate = true;
                search_info.OrderByShoworder = true;
                //search_info.DescByCreateDate = true;
                search_info.MaxCount = 5;

                int[] device_ids = new int[] { 1021 };

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
                list_result = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<TWNewEgg.Models.ViewModels.DeviceAd.HomeSubStoreSilderVM>>(data_result);

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
