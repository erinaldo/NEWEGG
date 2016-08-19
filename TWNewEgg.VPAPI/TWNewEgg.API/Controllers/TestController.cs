using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Service;
using System.Threading;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Reflection;

namespace TWNewEgg.API.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        private DB.TWBackendDBContext backend = new DB.TWBackendDBContext();
        private DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
        private DB.TWSellerPortalDBContext sellerPortal = new DB.TWSellerPortalDBContext();

        [HttpGet]
        public JsonResult QueryOrderDetail()
        {
            var result = backend.Process.GroupBy(x => x.CartID);

            //// 某Seller 名下的所有Process, 包括相同訂單的子商品。之後需要做Group by所以只能先存成List Jack.W.Wu 0626
            //List<DB.TWBACKENDDB.Models.Process> sellersProcessOriginal = (from pp in SellersProcessJoinCart select pp.proc).ToList();
            //// 同訂單的子單Group by, 只需要一筆之後再計算總量 add by Jack.W.Wu 0625
            //IQueryable<DB.TWBACKENDDB.Models.Process> sellersProcess = sellersProcessOriginal.GroupBy(x => x.CartID).Select(g => g.First()).AsQueryable();

            //IQueryable<DB.TWBACKENDDB.Models.Process> processOfSingleOrder = sellersProcess.Where(x => x.CartID == singleCart.ID).AsQueryable();
            DB.TWSQLDB.Models.Manufacture Info = new DB.TWSQLDB.Models.Manufacture();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult queryManufacture(string manufacture)
        {
           
            //// 某Seller 名下的所有Process, 包括相同訂單的子商品。之後需要做Group by所以只能先存成List Jack.W.Wu 0626
            //List<DB.TWBACKENDDB.Models.Process> sellersProcessOriginal = (from pp in SellersProcessJoinCart select pp.proc).ToList();
            //// 同訂單的子單Group by, 只需要一筆之後再計算總量 add by Jack.W.Wu 0625
            //IQueryable<DB.TWBACKENDDB.Models.Process> sellersProcess = sellersProcessOriginal.GroupBy(x => x.CartID).Select(g => g.First()).AsQueryable();

            //IQueryable<DB.TWBACKENDDB.Models.Process> processOfSingleOrder = sellersProcess.Where(x => x.CartID == singleCart.ID).AsQueryable();
            DB.TWSQLDB.Models.Manufacture Info = new DB.TWSQLDB.Models.Manufacture();

            Info = frontend.Manufacture.Where(x => x.WebAddress == manufacture).SingleOrDefault();
            TWNewEgg.API.Service.TWService service = new Service.TWService();

            Info = service.TWManufacture(manufacture);


            return Json(Info, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult dictest()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            result.Add(1, null);
            result.Add(2, null);
            result.Add(100, "test");

            var test = result.Keys.ToList();

            return Json(test, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult querysellertrackingno()
        {
            Dictionary<int, int> tmp_result = new Dictionary<int, int>();
            Dictionary<string,int> cart_productID = new Dictionary<string,int>();
            
            List<int> productIDList = new List<int>();
            List<string> tmp_cartID = new List<string>();
            List<preshipInfo> shipList = new List<preshipInfo>();

            DateTime today = new DateTime();
            
            today = DateTime.Now;

            // 取得今日時間
            //DateTime.TryParse(DateTime.Now.ToString("yyyy-MM-dd"), out today);
            // 比對 "已成立" 及 "待出貨"
            var tmp_preshipInfo = backend.Cart.Where(x => (x.DelvStatus == 0 || x.DelvStatus == 6) && x.DelivNO == null && x.Status != 1).Select(r => new { r.ID, r.CreateDate }).ToList();

            // 找出建立日期 >= 2 的訂單
            //tmp_preshipInfo = tmp_preshipInfo.Where(x => TimeSpan(today - x.CreateDate.Value) >= 0).ToList();
            foreach (var cartInfo in tmp_preshipInfo)
            {
                TimeSpan t1 = new TimeSpan(cartInfo.CreateDate.Value.Ticks);
                TimeSpan t2 = new TimeSpan(today.Ticks);

                double ts = (t2 - t1).TotalDays;

                //if( DateTime.Compare(today,cartInfo.CreateDate.Value)>0)
                //    tmp_cartID.Add(cartInfo.ID);

                if (ts >= 2)
                    tmp_cartID.Add(cartInfo.ID);
                
            }

            // 利用 CartID 至 Process 內找出 ProductID 
            foreach (var cartID in tmp_cartID)
            {
                //int? productID = backend.Process.Where(x => x.CartID == cartID).Select(r => r.ProductID).FirstOrDefault();
                var test = backend.Process.Where(x => x.CartID == cartID).Select(r => new { r.CartID, r.ProductID }).FirstOrDefault();

                if (cart_productID.Keys.Contains(test.CartID) == false && test.ProductID != 13189 && test.ProductID != 13190)
                {
                    cart_productID.Add(test.CartID, test.ProductID.Value);
                }

                //if (productID.HasValue)
                //    productIDList.Add(productID.Value);
            }
            // 排除 Process 內重複的 ProductID 
            //productIDList = productIDList.Distinct().ToList();


            foreach (var id in cart_productID)
            {
                int sellerID = frontend.Product.Where(x => x.ID == id.Value).Select(r => r.SellerID).FirstOrDefault();
                // 利用 productID 來 count seller 有多少訂單未押入 Tracking No.
                if (tmp_result.Keys.Contains(sellerID) == true)
                {
                    tmp_result[sellerID]++;
                }
                else
                {
                    tmp_result.Add(sellerID, 1);
                }
            }


            foreach (var item in tmp_result)
            {
                preshipInfo shipInfo = new preshipInfo();

                var tmp_sellerInfo = sellerPortal.Seller_BasicInfo.Where(x => x.SellerID == item.Key).Select(r => new { r.SellerEmail, r.SellerName, r.SellerID }).FirstOrDefault();

                if (tmp_sellerInfo != null)
                {
                    shipInfo.SellerID = tmp_sellerInfo.SellerID;
                    shipInfo.SellerEmail = tmp_sellerInfo.SellerEmail;
                    shipInfo.SellerName = tmp_sellerInfo.SellerName;
                    shipInfo.Unshipcount = tmp_result[item.Key];

                    shipList.Add(shipInfo);
                }


            }
           
            return Json(shipList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult testlist()
        {
            List<int> test = new List<int>();

            int a = test.Count();

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult testDistinct()
        {
            TWNewEgg.API.Models.Connector conn = new Models.Connector();

            List<string> test_distinct = new List<string>();
            string a = "RICO@ARECP.COM.TW";
            test_distinct.Add("rico@arecp.com.tw");
            test_distinct.Add(a.ToLower());
            test_distinct.Add("rico@arecp.com.tw");

            test_distinct = test_distinct.Distinct().ToList();

            string processVoidEmail = conn.GetAPIWebConfigSetting("VoidProcessEmailAddress");
            var voidemail = processVoidEmail.ToLower().Split(',').ToList();

            return Json(voidemail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult testreadexcel(string fileName,string sheetName,string sellerID)
        { 
            TWNewEgg.API.Models.Connector conn = new Models.Connector();

            TWNewEgg.API.Service.BatchItemUpdate test = new Service.BatchItemUpdate();
            test.LinqFromExcel(fileName, sheetName, sellerID);

            return null;
        }

        [HttpPost]
        [Filters.LoginAuthorize]
        public ActionResult testRegular(string url)
        {
            string pat = @"/(\w)+/(\w)+";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            Match m = r.Match(url);

            string ResultURL;

            // 利用 raw URL 判斷 User 權限
            if (m.Value == "")
            {
                ResultURL = url + "/Index";
            }
            else
            {
                ResultURL = m.Value;
            }

            return View();
        }

        [HttpGet]
        [Filters.LoginAuthorize]
        public ActionResult testRegular()
        {
            return View();
        }

        

    }

    public class preshipInfo
    {
        public int SellerID { get; set; }

        public string SellerEmail {get;set;}

        public string SellerName {get;set;}

        public int Unshipcount {get;set;}

        public string CartID { get; set; }
    }
}
