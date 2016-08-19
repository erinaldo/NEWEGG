using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.API.Controllers
{
    public class TransactionDetailsController : Controller
    {
        #region 交易明細SP_RPT_TransDetails
        [HttpGet]      
        public JsonResult Index(string inputOrderNumber,string inputInvoiceNumber, string inputSettlementID, string inputSellerPartNum, string inputNewEggItemNum)
        {
            Service.TransactionDetailsService TransactionDetailsService = new Service.TransactionDetailsService();
            Models.ActionResponse<List<Models.TransactionSPResult>> result = TransactionDetailsService.GetDataTransactionSP(inputOrderNumber, inputInvoiceNumber,inputSettlementID, inputSellerPartNum, inputNewEggItemNum);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// exec SP_RPT_TransDetails
        /// </summary>
        /// <param name="TransactionSPSearch"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult transactionDetailsReport(TWNewEgg.API.Models.TransactionSPSearch TransactionSPSearch)
        {
            Service.TransactionDetailsService TransactionDetailsService =new Service.TransactionDetailsService();
            Models.ActionResponse<List<Models.TransactionSPResult>> result = TransactionDetailsService.PostDataTransactionSP(TransactionSPSearch);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion




        // GET: /TransactionDetails/
        //public JsonResult Index(Models.LoginInfo loginInfo)
        //{
        //    Models.ActionResponse<Models.LoginResult> result = new Models.ActionResponse<Models.LoginResult>();
        //    result.Code = 0;
        //    result.IsSuccess = true;
        //    result.Msg = "Success";
        //    result.Body = new Models.LoginResult();
        //    result.Body.UserName = loginInfo.UserName;
        //    result.Body.Token = "TestToken_" + loginInfo.Password;
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult Index(Models.TransactionResult TransactionResult)
        //{
        //    //Models.ActionResponse<List<Models.TransactionResult>> result = new Models.ActionResponse<List<Models.TransactionResult>>();
        //    //result.Body = new List<Models.TransactionResult>();
        //    Models.ActionResponse<List<Models.TransactionSPResult>> result = new Models.ActionResponse<List<Models.TransactionSPResult>>();
        //    result.Body = new List<Models.TransactionSPResult>();

        //    DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
        //    //SP EXEC
        //    var spResult = backendDB.Database.SqlQuery<Models.TransactionSPResult>("exec UP_EC_RPT_TransDetails '','',''").ToList();
        //    result.Body.AddRange(spResult);

            /* ex1.增加單筆假資料 //
            Models.TransactionResult tr = new Models.TransactionResult();
            //tr.InvoiceNo = "AP123456";
            tr.Data = DateTime.UtcNow.AddHours(8);
            result.Body.Add(tr);
            */

            /*ex2.從資料庫撈資料出來，Where(x=>x.ID<5)
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            //DB.TWSQLDB.Models.Product p = db.Product.FirstOrDefault();
            List<DB.TWSQLDB.Models.Product> p = db.Product.Where(x=>x.ID<5).ToList();
            //Models.TransactionResult tr2 = new Models.TransactionResult();
            //tr2.InvoiceNo = p.ID.ToString()+"_"+p.Name;
            //tr2.InvoiceNo = p[10].ID.ToString() + "_" + p[10].Name;
            //tr2.Data = DateTime.UtcNow.AddHours(8);
            //result.Body.Add(tr2);
            */

            //DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            //List<DB.TWSQLDB.Models.PurchaseOrder> p = db.PurchaseOrder.Take(5).ToList();

            //ex3.Join 兩個表做Join
            
            //var joinResult = db.PurchaseOrder.Take(10).Join(db.PurchaseOrderItem, x => x.Code, y => y.PurchaseorderCode, (x, y) => new { PO_Code = x.Code, POI_PID = y.ProductID }).ToList();

            //foreach (var o in joinResult)
            //{
                //Models.TransactionResult trTemp = new Models.TransactionResult();
                //trTemp.InvoiceNo = product.ID.ToString() + "_" + product.Name;
                //trTemp.Data =DateTime.Now;
                //trTemp.TransactionType = "order";
                
                //PO 主單編號
                //trTemp.PurchaseorderCode = o.PO_Code;

                //PO 子單內的產品編號
                //trTemp.SellerProductNO = o.POI_PID.ToString();
                //result.Body.Add(trTemp);
           // }

            //ex4.多次查詢 組合
            /*var pos = db.PurchaseOrder.Take(10).ToList();
            foreach(var po in pos)
            {
                var pois = db.PurchaseOrderItem.Where(x => x.PurchaseorderCode == po.Code).ToList();
                foreach (var poi in pois)
                {
                    Models.TransactionResult trTemp = new Models.TransactionResult();
                    //trTemp.InvoiceNo = product.ID.ToString() + "_" + product.Name;
                    trTemp.Data = DateTime.Now;
                    trTemp.TransactionType = "order";

                    //PO 主單編號
                    trTemp.PurchaseorderCode = po.Code;

                    //PO 子單內的產品編號
                    trTemp.SellerProductNO = poi.ProductID.ToString();
                    result.Body.Add(trTemp);
                }
            }*/
            
            //return Json(result, JsonRequestBehavior.AllowGet);
        //}
    }
}
