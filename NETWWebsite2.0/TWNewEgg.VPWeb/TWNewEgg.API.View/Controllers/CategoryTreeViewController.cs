using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.View.Attributes;
using TWNewEgg.API.Models;
using System.IO;
using System.Drawing;

namespace TWNewEgg.API.View.Controllers
{
    public class CategoryTreeViewController : Controller
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        // GET: /TreeView/              
        //new connector
        Connector conn = new Connector();
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.CategoryTreeView)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("分類編號表")]
        public ActionResult CategoryTreeView()
        {
            return View();
        }

        public JsonResult Category(int? id)
        {

            string _strReturnMsg = string.Empty;
            bool Flag = false;
            List<TWNewEgg.API.View.categoryTemp> Category = new List<categoryTemp>();
            try
            {
                //API 拿Category資料    
                ActionResponse<List<DB.TWSQLDB.Models.Category>> result = new ActionResponse<List<DB.TWSQLDB.Models.Category>>();
                result = QueryCategory(null, null);
                if (result.IsSuccess = true && result.Code == (int)ResponseCode.Success)
                {
                    _strReturnMsg = "[Success]: " + result.Msg;

                    //select treeview所需資料
                    Category = (from e in result.Body

                                where (id.HasValue ? e.ParentID == id : e.ParentID == 0)
                                select new TWNewEgg.API.View.categoryTemp
                                {
                                    id = e.ID,
                                    Name = e.Description + "{" + e.ID + "}",
                                    hasChildren = hasChildren(e.ID, result)
                                }).ToList();
                    Flag = true;
                }
                else if (result.IsSuccess == false && result.Code == (int)ResponseCode.Error)
                {
                    _strReturnMsg = "[Error]: " + result.Msg;
                    Flag = false;
                }
                else
                {
                    _strReturnMsg = "資料錯誤";
                    Flag = false;
                }
            }
            catch (Exception error)
            {
                logger.Error(error.Message);
            }
            if (Flag == false)
            {
                return Json(new { IsSuccess = false, img = _strReturnMsg, Msg = "發生意外錯誤，請稍後再試!" });
            }
            else
            {
                return Json(Category, JsonRequestBehavior.AllowGet);
            }
        }

        //API 拿Category資料
        public ActionResponse<List<DB.TWSQLDB.Models.Category>> QueryCategory(int? layer, int? parentID)
        {
            TWNewEgg.API.View.Service.DisplayService dsService = new Service.DisplayService();

            ActionResponse<List<DB.TWSQLDB.Models.Category>> CategoryList = new ActionResponse<List<DB.TWSQLDB.Models.Category>>();

            CategoryList = dsService.GetCategoryList(null, layer, parentID);

            return CategoryList;

        }


        //判斷是否有下個孩子
        public Boolean hasChildren(int id, ActionResponse<List<DB.TWSQLDB.Models.Category>> result)
        {

            var Child = from e in result.Body
                        where (e.ParentID == id)
                        select new
                        {

                        };

            if (Child.Count() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}