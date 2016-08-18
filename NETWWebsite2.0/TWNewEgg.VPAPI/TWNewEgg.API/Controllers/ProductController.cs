using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/

        public JsonResult QueryProduct(Models.QueryProductCondition condition)
        {
            Service.ProductService ps = new Service.ProductService();
            var result = ps.QueryProduct(condition);
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditProduct(DB.TWSQLDB.Models.Product product)
        {
            Service.ProductService ps = new Service.ProductService();
            var result = ps.EditProduct(product);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateProduct(DB.TWSQLDB.Models.Product product)
        {
            Service.ProductService ps = new Service.ProductService();
            var result = ps.CreateProduct(product);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteProduct(DB.TWSQLDB.Models.Product product)
        {
            Service.ProductService ps = new Service.ProductService();
            var result = ps.DeleteProduct(product);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
