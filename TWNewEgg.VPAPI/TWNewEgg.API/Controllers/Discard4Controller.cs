using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Service;
//using TWNewEgg.DB.TWSQLDB.Models;
//using TWNewEgg.API.Attributes;
//using System.Transactions;
//using TWNewEgg.API.Models;

using TWNewEgg.Models.DomainModels;

namespace TWNewEgg.API.Controllers
{
    /// <summary>
    /// 初始化癈四機回收四聯單
    /// </summary>
    public class Discard4Controller : Controller
    {

        /// <summary>
        /// 初始化癈四機回收四聯單
        /// </summary>
        /// <param name="salesorderCode">LBO</param>
        /// <param name="user_name">建立者</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InitData(string salesorderCode, string user_name)
        {
            JsonResult json_data = new JsonResult();
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>> list_result = new TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>();
            
            //try
            //{
            //    list_result.Body = Discard4Service.InitData(salesorderCode, user_name);
            //    //json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            //    list_result.IsSuccess = true;
            //}
            //catch (Exception ex)
            //{
            //    json_data.Data = ex.Message;
            //    list_result.Msg = ex.Message;
            //}
            //finally
            //{
            //    json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            //}
            list_result.IsSuccess = true;
            list_result.Body = Discard4Service.InitData(salesorderCode, user_name);
            json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            return json_data;
        }

        /// <summary>
        /// 儲存即有的或新的資料
        /// </summary>
        /// <param name="list_info">
        /// List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>
        /// </param>
        /// <param name="user_name">建立者</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM> list_info, string user_name)
        {
            JsonResult json_data = new JsonResult();
            TWNewEgg.API.Models.ActionResponse<List<bool>> list_result = new TWNewEgg.API.Models.ActionResponse<List<bool>>();
            
            //try
            //{
            //    list_result.Body = Discard4Service.Save(list_info, user_name);
            //    //json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            //    list_result.IsSuccess = true;
            //}
            //catch (Exception ex)
            //{
            //    json_data.Data = ex.Message;
            //    list_result.Msg = ex.Message;
            //}
            //finally
            //{
            //    json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            //}
            list_result.IsSuccess = true;
            list_result.Body = Discard4Service.Save(list_info, user_name);
            json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);

            return json_data;
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="salesorderCode">LBO</param>
        /// <param name="user_name">建立者</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetData(string salesorderCode, string user_name)
        {
            JsonResult json_data = new JsonResult();
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>> list_result = new TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.Models.DomainModels.Discard4.Discard4ItemDM>>();
            
            //try
            //{
            //    list_result.Body = Discard4Service.GetData(salesorderCode, user_name);
            //    list_result.IsSuccess = true;
            //}
            //catch (Exception ex)
            //{
            //    json_data.Data = ex.Message;
            //    list_result.Msg = ex.Message;
            //}
            //finally {
            //    json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            //}

            list_result.IsSuccess = true;
            list_result.Body = Discard4Service.GetData(salesorderCode, user_name);
            json_data = this.Json(list_result, JsonRequestBehavior.AllowGet);
            
            return json_data;
            //return list_info;

        }

    }
}
