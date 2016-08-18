using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;

namespace TWNewEgg.API.Controllers
{
    public class SellerRelationshipManageController : Controller
    {
        //
        // GET: /SellerRelationshipManage/

        private Service.SellerRelationshipManageService service = new Service.SellerRelationshipManageService();

        //public ActionResult Seller_BasicInfo()
        //{
        //    return View();
        //}

        #region 資料取得
        
        //全部查詢.
        public JsonResult GetAllSeller_BasicInfos()
        {
            return Json(service.GetAll(), JsonRequestBehavior.AllowGet);
        }

        //Key值查詢
        public JsonResult GetSeller_BasicInfobyID(int id)
        {
            return Json(service.Get(id), JsonRequestBehavior.AllowGet);
        }

        //條件查詢物件.
        public JsonResult GetSeller_BasicInfosbyQuery(SellerRMQuery sQuery)
        {
            return Json(service.GetbyQuery(sQuery), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 搜尋商家關係清單
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>商家清單</returns>
        public JsonResult SearchSellerForRelationship(SellerRelationshipSearchCondition searchCondition)
        {
            Service.SellerRelationshipManageService sellerRelationshipManageService = new Service.SellerRelationshipManageService();

            API.Models.ActionResponse<List<VM_Seller_BasicInfo>> result = sellerRelationshipManageService.SearchSellerForRelationship(searchCondition);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public API.Models.ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>> GetSeller_BasicInfos()
        {
            Service.SellerRelationshipManageService sellerRelationshipManageService = new Service.SellerRelationshipManageService();

            API.Models.ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>> result = sellerRelationshipManageService.getAll_SellerBasic();

            return result;
        }

        #endregion

        #region 資料異動
        public JsonResult AddSeller_BasicInfo(Seller_BasicInfo item)
        {
            API.Models.ActionResponse<Seller_BasicInfo> ar = new API.Models.ActionResponse<Seller_BasicInfo>();
            if (ModelState.IsValid)
            {
                ar = service.Add(item);
            }
            return Json(ar, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditSeller_BasicInfo(Seller_BasicInfo sinfo)
        {
            API.Models.ActionResponse<bool> ar = new  API.Models.ActionResponse<bool>();
            //sinfo.SellerID = id;
            if (ModelState.IsValid)
            {
                ar = service.Update(sinfo);
            }
            else
            {
                ar.Code = 1;
                ar.IsSuccess = false;
                ar.Msg = new Exception("Model State Invalid").Message;
                ar.Body = false;               
            }
            return Json(ar, JsonRequestBehavior.AllowGet);
        }

        //批次異動.
        public JsonResult EditSeller_BasicInfoList(List<Seller_BasicInfo> sinfoList)
        {
            API.Models.ActionResponse<bool> ar = new API.Models.ActionResponse<bool>();
            //sinfo.SellerID = id;
            if (ModelState.IsValid)
            {
                    ar = service.UpdateList(sinfoList);
            }
            else
            {
                ar.Code = 1;
                ar.IsSuccess = false;
                ar.Msg = new Exception("Model State Invalid").Message;
                ar.Body = false;
            }
            return Json(ar, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSeller_BasicInfo(int? id)
        {
            API.Models.ActionResponse<bool> ar = null;
            ar = service.Delete(id);
            return Json(ar, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SelectItemList
        public JsonResult GetStatusList()
        {
            return Json(service.GetStatusList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTypeList()
        {
            return Json(service.GetTypeList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFTPList()
        {
            return Json(service.GetFTPList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDateList()
        {
            return Json(service.GetDateList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRegionList()
        {
            return Json(service.GetRegionList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMList()
        {
            return Json(service.GetCMList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 商家關係維護 grid 修改(20150907)
        public JsonResult sellerrelationshipmanagement_Edit(List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> editData, string userid)
        {
            TWNewEgg.API.Service.SellerRelationshipManageService sellerRelationManagerService = new Service.SellerRelationshipManageService();
            var result = sellerRelationManagerService.sellerrelationshipmanagement_Edit(editData, userid);
            return Json(result);
        }
        #endregion
       

        
        
        
        
    }
}
