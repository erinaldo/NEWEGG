using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    public class SketchPropertyController : Controller
    {
        //
        // GET: /SketchProperty/

        public ActionResult Index()
        {
            return View();
        }
        #region 草稿區查詢
        public JsonResult ItemSketchSearch(ItemSketchSearchCondition itemSkSearCondition)
        {
            TWNewEgg.API.Service.ItemSketchPropertyService IsPropertySer = new Service.ItemSketchPropertyService();
            var result = IsPropertySer.ItemSketchSearch(itemSkSearCondition);
            return Json(result);
        }
        #endregion
        public JsonResult ItemSketchEdit(ItemSketchEditType editType, List<ItemSketch> itemSketchCell)
        {
            TWNewEgg.API.Service.ItemSketchPropertyService IsPropertySer = new Service.ItemSketchPropertyService();
            var result = IsPropertySer.ItemSketchEdit(editType, itemSketchCell);
            return Json(result);
        }
        public JsonResult ItemSketchDelete(int toDeleteId)
        {
            TWNewEgg.API.Service.ItemSketchPropertyService IsPropertySer = new Service.ItemSketchPropertyService();
            var result = IsPropertySer.ItemSketchDelete(toDeleteId);
            return Json(result);
        }
        public JsonResult ItemSketchExamine(List<int> toExamineId, int userid = 0, int sellerid = 0)
        {
            TWNewEgg.API.Service.ItemSketchPropertyService IsPropertySer = new Service.ItemSketchPropertyService();
            var result = IsPropertySer.ItemSketchExamine(toExamineId, userid, sellerid);
            return Json(result);
        }
        #region 屬性商品批次送審(包含一維跟二維)
        public JsonResult ItemSketchBatchExamine(List<TWNewEgg.API.Models.BatchExamineModel> BatchExamineModel, string userEmail, string password)
        {
            TWNewEgg.API.Service.PropertyBatchExamineService ProBatchExSer = new Service.PropertyBatchExamineService();
            var result = ProBatchExSer.propertyExamine(BatchExamineModel, userEmail, password);
            return Json(result);
        }
        #endregion


        public JsonResult ItemSketchPropertyListSearch(ItemSketchSearchCondition itemsketchListData, bool isSearch = true)
        {
            TWNewEgg.API.Service.ItemSketchPropertyService IsPropertySer = new Service.ItemSketchPropertyService();
            var result = IsPropertySer.ItemSketchPropertyListSearch(itemsketchListData, isSearch);
            return Json(result);
        }

        public JsonResult ItemSketchPropertyListDelete(List<int> deleteID)
        {

            TWNewEgg.API.Service.ItemSketchPropertyService IsPropertySer = new Service.ItemSketchPropertyService();
            var result = IsPropertySer.ItemSketchPropertyListDelete(deleteID);
            return Json(result);
        }
        public JsonResult ItemSketchPropertyListUpdate(List<TWNewEgg.API.Models.ItemSketch> itemsketckDeleteModel)
        {
            TWNewEgg.API.Service.ItemSketchPropertyService IsPropertySer = new Service.ItemSketchPropertyService();
            var result = IsPropertySer.ItemSketchPropertyListUpdate(itemsketckDeleteModel);
            return Json(result);
        }
        public JsonResult ItemPropertyOpenViewEdit(TWNewEgg.API.Models.ItemSketch itemsketch)
        {
            TWNewEgg.API.Service.ItemSketchPropertyService IsPropertySer = new Service.ItemSketchPropertyService();
            //var result = IsPropertySer.ItemPropertyOpenViewEdit(itemsketch);
            var result = IsPropertySer.propertyDetailEdit(itemsketch);
            return Json(result);
        }
        #region 規格品待審區匯出 Excel
        public JsonResult excelSearchProperty(ItemSketchSearchCondition itemsketchListData)
        {
            TWNewEgg.API.Service.ItemSketchPropertyService IsPropertySer = new Service.ItemSketchPropertyService();
            var result = IsPropertySer.excelSearchProperty(itemsketchListData);
            return Json(result);
        }
        #endregion
    }
}
