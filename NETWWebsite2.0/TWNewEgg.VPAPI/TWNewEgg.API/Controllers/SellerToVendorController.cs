using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.API.Service;
using log4net;

namespace TWNewEgg.API.Controllers
{
    public class SellerToVendorController : Controller
    {
        //
        // GET: /Commission/
        private DB.TWBackendDBContext backend = new DB.TWBackendDBContext();
        private DB.TWSqlDBContext frontend = new DB.TWSqlDBContext();
        private DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

        private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        private int GetNewSellerID(int oldSellerID)
        {
            DB.TWSellerPortalDBContext sellerPortalDB = new DB.TWSellerPortalDBContext();

            string oldSellerEmail = sellerPortalDB.Seller_BasicInfo.Where(x => x.SellerID == oldSellerID).Select(r => r.SellerEmail).FirstOrDefault();

            int newSellerID = sellerPortalDB.Seller_BasicInfo.Where(x => x.SellerEmail == oldSellerEmail).Where(y => y.AccountTypeCode == "V").Select(r => r.SellerID).FirstOrDefault();

            return newSellerID;
        }


        [HttpGet]
        public JsonResult SellerItemandProductChangeToVendor(int oldSellerID)
        {
            TWNewEgg.API.Service.SellerItemToVendor updateService = new SellerItemToVendor();

            return Json(updateService.ItemUpdate(oldSellerID), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SellerChangeToVendor(string oldSellerID)
        {
            TWNewEgg.API.Service.SellerToVendorService updateService = new SellerToVendorService();

            return Json(updateService.UpdateSellerBasicInfo(oldSellerID), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BatchSellerChangeToVendor(string[] BatchSellerID)
        {
            Dictionary<string, List<string>> changeResult = new Dictionary<string, List<string>>();
            foreach (string seller_index in BatchSellerID)
            {
                TWNewEgg.API.Service.SellerToVendorService updateService = new SellerToVendorService();

                changeResult.Add(seller_index, updateService.UpdateSellerBasicInfo(seller_index));
            }

            return Json(changeResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BatchSellerItemChangeToVendor(string[] BatchSellerID)
        {
            Dictionary<string, List<string>> changeResult = new Dictionary<string, List<string>>();
            foreach (string seller_index in BatchSellerID)
            {
                int sellerid = Convert.ToInt32(seller_index);
                TWNewEgg.API.Service.SellerItemToVendor updateService = new SellerItemToVendor();

                changeResult.Add(seller_index, updateService.ItemUpdate(sellerid));
            }

            return Json(changeResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 供 3/31 更改 Seller 權限
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SellerAuthChange(string closeSeller)
        {
            // 需要關閉上下架權限的SellerIDs
            //List<int> oldSellerIDs = sellerPortalDB.Seller_ChangeToVendor.Select(x => x.OldSellerID.Value).ToList();

            // 需要開啟開啟上下架權限的SellerIDs
            //List<int> newSellerIDs = sellerPortalDB.Seller_ChangeToVendor.Select(x => x.NewSellerID.Value).ToList();
            //List<int> closeList = new List<int>();
            //string[] closeArray = closeSeller.Split(',');

            //foreach (var closeIndex in closeArray)
            //{
            //    int closeID = Convert.ToInt32(closeIndex);
            //    closeList.Add(closeID);    
            //}

            string result = "Batch Change OK!";

            List<int> closeList = closeSeller.Split(',').Select(Int32.Parse).ToList();
            var authChangeIDs = sellerPortalDB.Seller_BasicInfo.Where(x => closeList.Contains(x.SellerID)).ToList();

            //var closeSellers = sellerPortalDB.Seller_BasicInfo.Where(x => authChangeIDs.Contains(x.SellerID)
            //    && x.AccountTypeCode == "S"
            //    && x.SellerStatus != "C").ToList();

            foreach (var closeSellerindex in authChangeIDs)
            {
                closeSellerindex.SellerStatus = "I";
            }

            try
            {
                sellerPortalDB.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message + ex.StackTrace;
            }
            /*
             1. 修改old Seller權限為Inactive
                update Seller_BasicInfo
                set SellerStatus = 'I'
                where /*sellerID = @old_sellerID
                //and AccountTypeCode = 'S'
                //and SellerStatus <> 'C'
                //and SellerID > 6
            2. 修改Seller_User.SellerID
                //a. Seller_User.GroupID = 1(Seller Owner)
                //修改GroupID = 7, PurviewType = S, SellerID = -1
                //複製Group_Purview ==> Seller_Purview
                //b. Seller_User.GroupID = 2(Normal User)
                //修改SellerID = @new_sellerID
             */

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 供 3/31 更改 Seller 賣場
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SellerItemChange(string closeSeller)
        {
            DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
            string result = "Batch Change OK!";
            List<int> oldSellerIDs = closeSeller.Split(',').Select(Int32.Parse).ToList();
            List<int> newSellerIDs = new List<int>();
            //var authChangeIDs = sellerPortalDB.Seller_BasicInfo.Where(x => oldSellerIDs.Contains(x.SellerID)).ToList();

            // 需要設定隱形賣場並且下架的SellerIDs
            //List<int> oldSellerIDs = sellerPortalDB.Seller_ChangeToVendor.Select(x => x.OldSellerID.Value).ToList();

            // 賣場需要設定上架的SellerIDs
            //List<int> newSellerIDs = sellerPortalDB.Seller_ChangeToVendor.Select(x => x.NewSellerID.Value).ToList();

            var closeItems = db_before.Item.Where(x => oldSellerIDs.Contains(x.SellerID)).ToList();

            // 舊 Seller 商品下架，並且設定為隱形賣場
            foreach (var close_index in closeItems)
            {
                close_index.Status = 1;
                close_index.ShowOrder = -1;
            }

            // 找到新的 SellerIDs
            foreach (int oldSeller_index in oldSellerIDs)
            {
                int newSellerID = this.GetNewSellerID(oldSeller_index);
                if (newSellerID != 0)
                {
                    newSellerIDs.Add(newSellerID);
                }
            }

            var sellingProduct = db_before.Product.Where(x => newSellerIDs.Contains(x.SellerID) && x.Cost != 0).Select(y => y.ID).ToList();

            var readySellingItem = db_before.Item.Where(x => sellingProduct.Contains(x.ProductID)).ToList();

            // 新 Seller 商品成本不為 0 的上架
            foreach (var selling_index in readySellingItem)
            {
                selling_index.Status = 0;
                selling_index.ShowOrder = 0;
            }

            var disableshoworderItem = db_before.Item.Where(x => newSellerIDs.Contains(x.SellerID)).ToList();

            // 將屬於新Seller的商品全部改成非隱形賣場
            foreach (var change_index in disableshoworderItem)
            {
                change_index.ShowOrder = 0;
            }

            try
            {
                db_before.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message + ex.StackTrace;
            }

            /*  update item
                set item.status = 0
                where sellerID = @new_sellerID
                and item.status = -1
                update item
                set item.status = -1
                where sellerID = @old_sellerID
                and item.status = 0
            */

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult batchPriceUpdate()
        {
            TWService updateservice = new TWService();

            var updateItems = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.NewItemID.HasValue && x.Exception == null).Select(x => x.NewItemID.Value).ToList();

            var result = updateservice.PriceAPI(updateItems);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult batchUpdateProperty()
        {
            TWService updateservice = new TWService();
            Dictionary<int, string> changeResult = new Dictionary<int, string>();
            var updateProducts = sellerPortalDB.Seller_ChangeToVendor.Where(x => x.NewItemID.HasValue && x.Exception == null).Select(x => x.NewProductID.Value).ToList();

            foreach (var index in updateProducts)
            {
                var result = updateservice.PropertyXMLAPI(index);
                changeResult.Add(index, result.Msg);
            }

            return Json(changeResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BatchProcessImages(string oldItemID, int mode)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            TWNewEgg.API.Service.SellerItemToVendor itemchange = new SellerItemToVendor();
            List<int> oldItemIDs = new List<int>();

            if (!string.IsNullOrWhiteSpace(oldItemID))
            {
                oldItemIDs = oldItemID.Split(',').Select(Int32.Parse).ToList();
            }
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            List<DB.TWSELLERPORTALDB.Models.Seller_ChangeToVendor> images = new List<DB.TWSELLERPORTALDB.Models.Seller_ChangeToVendor>();

            switch (mode)
            {
                // 處理全部圖片資料
                case 0:
                    images = spdb.Seller_ChangeToVendor.Where(x => x.OldItemID.HasValue
                            && x.NewItemID.HasValue
                            && x.OldProductID.HasValue
                            && x.Exception == null && x.NewProductID.HasValue).ToList();
                    break;
                case 1:
                    images = spdb.Seller_ChangeToVendor.Where(x => x.OldItemID.HasValue
                            && x.NewItemID.HasValue
                            && x.OldProductID.HasValue
                            && x.Exception == null && x.NewProductID.HasValue).Where(y => oldItemIDs.Contains(y.OldItemID.Value)).ToList();
                    break;
            }

            foreach (var itemimages in images)
            {
                result.Add(itemimages.NewItemID.Value.ToString(), itemchange.GetitemImage(itemimages.OldItemID.Value, itemimages.NewItemID.Value.ToString(), itemimages.NewProductID.Value.ToString()));
                Thread.Sleep(2000);
            }

            //result.Add("111781", itemchange.GetitemImage(85128, "111781", "105299"));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Process640Images(List<string> PictureURL, string ProductID, string ItemID)
        {
            ActionResponse<bool> result = null;
            if (PictureURL != null)
            {
                TWNewEgg.API.Service.SellerItemToVendor imageService = new SellerItemToVendor();
                result = imageService.PostImageToDB640(PictureURL, ProductID, ItemID);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Process300Images(List<string> PictureURL, string ProductID, string ItemID)
        {
            ActionResponse<bool> result = null;
            if (PictureURL != null)
            {
                TWNewEgg.API.Service.SellerItemToVendor imageService = new SellerItemToVendor();
                result = imageService.PostImageToDB300(PictureURL, ProductID, ItemID);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Process125Images(List<string> PictureURL, string ProductID, string ItemID)
        {
            ActionResponse<bool> result = null;
            if (PictureURL != null)
            {
                TWNewEgg.API.Service.SellerItemToVendor imageService = new SellerItemToVendor();
                result = imageService.PostImageToDB125(PictureURL, ProductID, ItemID);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Process60Images(List<string> PictureURL, string ProductID, string ItemID)
        {
            ActionResponse<bool> result = null;
            if (PictureURL != null)
            {
                TWNewEgg.API.Service.SellerItemToVendor imageService = new SellerItemToVendor();
                result = imageService.PostImageToDB60(PictureURL, ProductID, ItemID);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangeToTemp(int sellerid)
        {
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();

            var sellerItem = db.Item.Where(x => x.SellerID == sellerid && (x.Status == 0 || x.Status == 1)).ToList();

            List<int> productIDs = db.Item.Where(x => x.SellerID == sellerid && (x.Status == 0 || x.Status == 1)).Select(y => y.ProductID).ToList();

            List<DB.TWSQLDB.Models.ProductTemp> sellerProduct = db.ProductTemp.Where(x => productIDs.Contains(x.ID)).ToList();

            List<DB.TWSQLDB.Models.ItemStock> selleritemstock = db.ItemStock.Where(x => productIDs.Contains(x.ProductID)).ToList();

            changetoItemTemp(sellerItem);
            changeToproductTemp(sellerProduct);
            //changeToItemStockTemp();
            //changeToProductPropertyTemp();


            return Json(null, JsonRequestBehavior.AllowGet);
        }

        private void changeToItemStockTemp(List<DB.TWSQLDB.Models.ItemStocktemp> vendoritemstock)
        {
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            List<DB.TWSQLDB.Models.ItemStocktemp> itemStockTempCell = new List<DB.TWSQLDB.Models.ItemStocktemp>();

            AutoMapper.Mapper.CreateMap<DB.TWSQLDB.Models.ItemStock, DB.TWSQLDB.Models.ItemStocktemp>();

            try
            {
                // 從 DB 讀取 ItemStock 並轉為 ItemStockTemp
                List<DB.TWSQLDB.Models.ItemStock> itemStockCell = dbFront.ItemStock.ToList();
                itemStockTempCell = AutoMapper.Mapper.Map<List<DB.TWSQLDB.Models.ItemStocktemp>>(itemStockCell);
            }
            catch (Exception ex)
            {
                log.Info(string.Format("ChangeItemStockTemp Get ItemStock Error; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            // 從 DB 讀取 ProductTemp
            List<DB.TWSQLDB.Models.ProductTemp> productTempCell = new List<DB.TWSQLDB.Models.ProductTemp>();

            try
            {
                // 僅讀出 ProductTempID、ProductID 欄位
                dbFront.ProductTemp.ToList().ForEach(x =>
                {
                    DB.TWSQLDB.Models.ProductTemp productTemp = new DB.TWSQLDB.Models.ProductTemp();
                    productTemp.ID = x.ID;
                    productTemp.ProductID = x.ProductID;
                    productTempCell.Add(productTemp);
                });
            }
            catch (Exception ex)
            {
                log.Info(string.Format("ChangeItemStockTemp Get ProductTemp Error; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            foreach (DB.TWSQLDB.Models.ItemStocktemp itemStockTemp in itemStockTempCell)
            {
                if (itemStockTemp.ProductID != null && itemStockTemp.ProductID != 0)
                {
                    itemStockTemp.producttempID = productTempCell.Where(x => x.ProductID == itemStockTemp.ProductID).Select(x => x.ID).FirstOrDefault();
                    dbFront.ItemStocktemp.Add(itemStockTemp);
                }
                else
                {
                    log.Info(string.Format("ChangeItemStockTemp ItemStock ProductID Error."));
                }
            }

            try
            {
                dbFront.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Info(string.Format("ChangeItemStockTemp Save ItemStocktemp Error; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }
        }

        private void changeToproductTemp(List<DB.TWSQLDB.Models.ProductTemp> vendorproduct)
        {
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            List<DB.TWSQLDB.Models.ProductTemp> productTempCell = new List<DB.TWSQLDB.Models.ProductTemp>();

            AutoMapper.Mapper.CreateMap<DB.TWSQLDB.Models.Product, DB.TWSQLDB.Models.ProductTemp>()
                .ForMember(x => x.ProductID, x => x.MapFrom(src => src.ID));

            try
            {
                // 從 DB 讀取 Product 並轉為 ProductTemp
                List<DB.TWSQLDB.Models.Product> productCell = dbFront.Product.ToList();
                productTempCell = AutoMapper.Mapper.Map<List<DB.TWSQLDB.Models.ProductTemp>>(productCell);
            }
            catch (Exception ex)
            {
                log.Info(string.Format("ChangeProductTemp Get Product Error; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }


            foreach (DB.TWSQLDB.Models.ProductTemp productTemp in productTempCell)
            {
                dbFront.ProductTemp.Add(productTemp);
            }

            try
            {
                dbFront.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Info(string.Format("ChangeProductTemp Save ProductTemp Error; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }
        }

        private void changetoItemTemp(List<DB.TWSQLDB.Models.Item> vendoritem)
        {
            DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            List<DB.TWSQLDB.Models.ItemTemp> itemTempCell = new List<DB.TWSQLDB.Models.ItemTemp>();

            AutoMapper.Mapper.CreateMap<DB.TWSQLDB.Models.Item, DB.TWSQLDB.Models.ItemTemp>()
                .ForMember(x => x.ItemID, x => x.MapFrom(src => src.ID))
                .ForMember(x => x.ApproveDate, x => x.MapFrom(src => DateTime.Now))
                .ForMember(x => x.ItemTempDesc, x => x.MapFrom(src => src.ItemDesc))
                .ForMember(x => x.DelvData, x => x.MapFrom(src => src.DelvDate))
                .ForMember(x => x.Showorder, x => x.MapFrom(src => src.ShowOrder))
                .ForMember(x => x.Pricehpinst1, x => x.MapFrom(src => src.PricehpInst1))
                .ForMember(x => x.Pricehpinst2, x => x.MapFrom(src => src.PricehpInst2))
                .ForMember(x => x.Status, x => x.MapFrom(src => 0))
                .ForMember(x => x.ItemStatus, x => x.MapFrom(src => src.Status))
                .ForMember(x => x.ApproveMan, x => x.MapFrom(src => 61751))
                .ForMember(x => x.SubmitMan, x => x.MapFrom(src => 61751))
                .ForMember(x => x.GrossMargin, x => x.MapFrom(src => 0));
            
            try
            {
                // 從 DB 讀取 Item 並轉為 ItemTemp
                List<DB.TWSQLDB.Models.Item> itemCell = dbFront.Item.ToList();
                itemTempCell = AutoMapper.Mapper.Map<List<DB.TWSQLDB.Models.ItemTemp>>(itemCell);
            }
            catch (Exception ex)
            {
                log.Info(string.Format("ChangeItemTemp Get Item Error; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            // 從 DB 讀取 ProductTemp
            List<DB.TWSQLDB.Models.ProductTemp> productTempCell = new List<DB.TWSQLDB.Models.ProductTemp>();

            try
            {
                // 僅讀出 ProductTempID、ProductID、Cost 欄位
                dbFront.ProductTemp.ToList().ForEach(x =>
                {
                    DB.TWSQLDB.Models.ProductTemp productTemp = new DB.TWSQLDB.Models.ProductTemp();
                    productTemp.ID = x.ID;
                    productTemp.ProductID = x.ProductID;
                    productTemp.Cost = x.Cost;
                    productTempCell.Add(productTemp);
                });
            }
            catch (Exception ex)
            {
                log.Info(string.Format("ChangeItemTemp Get ProductTemp Error; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            foreach (DB.TWSQLDB.Models.ItemTemp itemTemp in itemTempCell)
            {
                itemTemp.ProduttempID = productTempCell.Where(x => x.ProductID == itemTemp.ProductID).Select(x => x.ID).FirstOrDefault();
                
                if (itemTemp.PriceCash > 0 && productTempCell.Where(x => x.ProductID == itemTemp.ProductID).Select(x => x.Cost).FirstOrDefault() != null)
                { 
                    itemTemp.GrossMargin = ((itemTemp.PriceCash - productTempCell.Where(x => x.ProductID == itemTemp.ProductID).Select(x => x.Cost).FirstOrDefault().Value) / itemTemp.PriceCash) * 100m; 
                }
                
                dbFront.ItemTemp.Add(itemTemp);
            }

            try
            {
                dbFront.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Info(string.Format("ChangeItemTemp Save ItemTemp Error; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }
        }

        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

    }
}
