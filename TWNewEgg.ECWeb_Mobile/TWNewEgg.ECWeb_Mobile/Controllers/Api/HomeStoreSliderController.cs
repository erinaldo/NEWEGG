
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
    /// 1020	手機版首頁櫥窗輪播
    /// </summary>
    public class HomeStoreSliderController : ApiController
    {
        
        //[AllowNonSecures]
        //[AllowAnonymous]
        //[HttpGet]
        //// GET api/HomeStoreSlider
        //public List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM> Get()
        //{
        //    List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM> result = null;
        //    result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM>();
        //    try
        //    {
        //        result = get_data();
        //    }
        //    catch (Exception ex) { }
        //    return result;
        //}

        ///95	生活家電qq	1
        ///33	設計風尚	2
        ///34	國際名品	3
        ///35	美妝保養	4
        ///94	戶外休旅	5
        ///100	運動健身	6
        ///101	電腦週邊	7
        ///99	數位３Ｃ	8
        ///96	居家用品	9
        ///97	親子寵物	10
        ///36	保健養生	11
        ///37	樂活食尚	12
        ///38	美國新蛋直購	13
        ///131	玩轉世界生活誌	14
        ///137	尋找禮品	15
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        public List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM> Get([FromUri]List<int> index)
        {
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM> result = null;
            result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM>();
            try
            {
                result = get_data2(index);
            }
            catch (Exception ex) { }
            return result;
        }

        private List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM> get_data2(List<int> index)
        {
            //JsonResult json = new JsonResult();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM> list_result = null;
            list_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM>();

            List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM> data_result = null;
            data_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM>();

            try
            {
                /// 1020	手機版櫥窗輪播
                TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM search_info = null;
                //取得廣告內文
                search_info = new TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM();
                //search_info.DescByCreateDate = true; //DescByCreateDate -------------------add by bruce 20160616
                search_info.IsBetweenDate = false; //手機版分類頁輪播不需日期起迄檢查-------------add by bruce 20160705
                search_info.OrderByShoworder = true;
                //search_info.DescByCreateDate = true;
                search_info.MaxCount = 3;

                int[] device_ids = new int[] { 1020 };

                int[] index_ids = new int[] { 1 }; //預設第一個

                if(index.Count>0)
                    index_ids = index.ToArray<int>();

                foreach (int each_index in index_ids)
                {
                    search_info.DeviceAdSetIDs = device_ids.ToList<int>();
                    search_info.CategoryIDs.Clear();
                    search_info.Flag = each_index.ToString(); //指定第幾個手機版首頁櫥窗的資料

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
                list_result = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM>>(data_result);

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

        private List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM> get_data()
        {
            //JsonResult json = new JsonResult();
            List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM> list_result = null;
            list_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM>();

            List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM> data_result = null;
            data_result = new List<TWNewEgg.Models.ViewModels.DeviceAd.DeviceAdContentVM>();

            try
            {
                /// 1020	手機版櫥窗輪播
                TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM search_info = null;
                //取得廣告內文
                search_info = new TWNewEgg.Models.DomainModels.DeviceAd.DeviceAdSearchDM();
                //search_info.DescByCreateDate = true; //DescByCreateDate -------------------add by bruce 20160616
                search_info.IsBetweenDate = true;
                search_info.OrderByShoworder = true;
                //search_info.DescByCreateDate = true;
                search_info.MaxCount = 3;

                int[] device_ids = new int[] { 1020 };                                
                
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
                list_result = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<TWNewEgg.Models.ViewModels.DeviceAd.HomeStoreSilderVM>>(data_result);

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
