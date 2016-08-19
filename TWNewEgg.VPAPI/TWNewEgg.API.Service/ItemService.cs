using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using System.Data;
using TWNewEgg.API.Models;
using System.Web;
using log4net;
using log4net.Config;
using System.Transactions;
using System.Threading;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// 查詢 編輯 刪除商品
    /// </summary>
    public class ItemService
    {
        #region Private Parameter & Enum

        // 圖片機網址
        private string images = System.Configuration.ConfigurationManager.AppSettings["Images"];
        // 賣場網址
        private string webSite = System.Configuration.ConfigurationManager.AppSettings["WebSite"];
        private API.Service.TWService frontendService = new TWService();

        /// <summary>
        /// API ActionResponse Code
        /// </summary>
        private enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        /// <summary>
        /// AddProductDetail判斷結果
        /// </summary>
        private enum MessageCode
        {
            Success = 0,
            ProductExist = 1,
            InvalidQty = 2,
        }

        #endregion

        #region Private function

        private static ILog log = LogManager.GetLogger(typeof(ItemService));

        #region Quert ItemList FBNList Model
        /// <summary>
        /// 3.丟入搜尋結果Model (item 及 product 綁定SellerID Inner Join)
        /// </summary>
        /// <param name="sellerID">sellerID</param>
        /// <returns>IQueryable</returns>
        private IQueryable<ItemInfoList> QueryItemList(int sellerID)
        {
            //// 使用商品類別 layer2 搜尋 商品子類別 layer1
            //itemInfo.SubcategoryID = db.Category.Where(x => x.ID == itemInfo.ItemCategoryID).Select(x => x.ParentID).FirstOrDefault();
            //// 使用商品子類別 layer1 搜尋 商品主類別 layer0
            //itemInfo.Industry = db.Category.Where(x => x.ID == itemInfo.SubcategoryID).Select(x => x.ParentID).FirstOrDefault();
            //// 類別ID 轉成 類別名稱
            //itemInfo.ItemCategoryName = this.CategoryID2Title(itemInfo.ItemCategoryID);
            //itemInfo.SubcategoryName = this.CategoryID2Title(db.Category.Where(x => x.ID == itemInfo.ItemCategoryID).Select(x => x.ParentID).FirstOrDefault());
            //itemInfo.IndustryName = this.CategoryID2Title(db.Category.Where(x => x.ID == itemInfo.SubcategoryID).Select(x => x.ParentID).FirstOrDefault());


            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var itemList = (from p in db.Product
                            join i in db.Item on p.ID equals i.ProductID

                            //add join ItemTemp by Angela 
                            join t in db.ItemTemp on i.ID equals t.ItemID
                            join temp in db.ItemTemp on i.ID equals temp.ItemID
                            join s in db.ItemStock on p.ID equals s.ProductID
                            join n in db.Seller on i.SellerID equals n.ID
                            join m in db.Manufacture on temp.ManufactureID equals m.ID
                            join c in db.Category on i.CategoryID equals c.ID
                            // join subcatrgory
                            join sub in db.Category on c.ParentID equals sub.ID
                            join cross in db.ItemCategorytemp on i.ID equals cross.ItemID into cross_category
                            from cross in cross_category.DefaultIfEmpty()
                            join csub in db.Category on cross.CategoryID equals csub.ID into csub_category
                            from csub in csub_category.DefaultIfEmpty()
                            select new Models.ItemInfoList
                            {
                                // SellerName, SubCategoryID, IndustryID, CategoryName*3, ImageSource
                                SellerName = n.Name,
                                ItemShipType = i.ShipType,
                                ItemID = i.ID,
                                ItemQty = i.Qty,

                                Status = t.Status,
                                // ItemStatus = i.status,
                                // 商品狀態 edit by Angela
                                ItemStatus = t.ItemStatus.HasValue ? t.ItemStatus.Value : 1,

                                //判斷是否為規格品
                                ItemTempID = t.ID,

                                ItemPriceCash = i.PriceCash,
                                ItemShipFee = i.PriceLocalship,
                                ItemManufacturerID = i.ManufactureID,
                                ItemSellerID = i.SellerID,
                                ProductUPC = p.UPC,
                                ProductManufacturerPartNum = p.MenufacturePartNum,
                                ProductSellerProductID = p.SellerProductID,
                                ManufacturerName = m.Name,
                                ItemName = i.Name,
                                ProductNameUS = p.Name,
                                ItemCategoryID = i.CategoryID,
                                ItemCategoryName = c.Description,
                                SubcategoryID = c.ParentID,

                                SubcategoryName = sub.Description,
                                Industry = sub.ParentID,
                                IndustryName = db.Category.Where(x => x.ID == sub.ParentID).Select(y => y.Description).FirstOrDefault(),

                                // itemList only
                                ItemMarketPrice = i.MarketPrice,
                                ProductCost = p.Cost,
                                // 先抓取，需再調較時區
                                ItemCreateDate = i.CreateDate,
                                ItemDateStart = i.DateStart,
                                ItemDateEnd = i.DateEnd,
                                ProductID = p.ID,

                                ItemReg = i.QtyReg,
                                ItemInventory = i.Qty - i.QtyReg,
                                ProductQty = s.Qty,
                                ProductReg = s.QtyReg,
                                ProductInventory = s.Qty - s.QtyReg,
                                // 安全庫存量(ItemStock.SafeQty)
                                ProductSafeQty = s.SafeQty,
                                ProductStatusInt = p.Status,

                                // 商品成色 2014.10.27 add by Smoke
                                IsNew = i.IsNew,

                                // FBN only
                                ProductDimension = p.Length * p.Width * p.Height
                            }).Where(x => x.ItemSellerID == sellerID && x.ItemStatus != 99).Distinct().AsQueryable();

            var ItemTempGroup = db.ItemGroupDetailProperty.Where(x => x.SellerID == sellerID && x.ItemTempID.HasValue).Select(r => r.ItemTempID.Value);

            itemList = itemList.Where(p => !ItemTempGroup.Any(q => p.ItemTempID == q)).AsQueryable();
            return itemList;
            //return itemList.Where(x => !ItemTempGroup.Contains(x.ItemTempID));
        }

        #endregion

        #region Query ItemCreation Model
        /// <summary>
        /// 搜尋創建商品之所有欄位
        /// </summary>
        /// <param name="sellerID">賣家</param>
        /// <param name="itemID">商品編號</param>
        /// <returns>ItemModel</returns>
        private APIItemModel QueryItemModel(int sellerID, int itemID)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var itemList = (from i in db.Item
                            join p in db.Product on i.ProductID equals p.ID
                            join s in db.ItemStock on p.ID equals s.ProductID
                            join displayprice in db.ItemDisplayPrice on i.ID equals displayprice.ItemID into tmp
                            from ds in tmp.DefaultIfEmpty()
                            select new APIItemModel
                            {
                                API_Product_BarCode = p.BarCode,
                                //API_Product_CreateDate = p.CreateDate,
                                API_Product_CreateUser = p.CreateUser,
                                API_Product_DelvType = p.DelvType,
                                API_Product_Description = p.Description,
                                API_Product_DescriptionTW = p.DescriptionTW,
                                API_Product_Height = p.Height,
                                //API_Product_InvoiceType = p.InvoiceType,
                                //API_Product_IsMarket = p.IsMarket,
                                API_Product_IsShipDanger = p.IsShipDanger,
                                API_Product_Length = p.Length,
                                API_Product_ManufacturerID = p.ManufactureID,
                                API_Product_MenufacturePartNum = p.MenufacturePartNum,
                                API_Product_Model = p.Model,
                                API_Product_NameTW = p.NameTW,
                                API_Product_Note = p.Note,
                                //API_Product_PicEnd = p.PicEnd,
                                //API_Product_PicStart = p.PicStart,
                                API_Product_productName = p.Name,
                                //API_Product_SaleType = p.SaleType,
                                API_Product_SellerID = p.SellerID,
                                API_Product_SellerProductID = p.SellerProductID,
                                //API_Product_SourceTable = p.SourceTable,
                                //API_Product_Spec = p.SPEC,
                                API_Product_Status = p.Status,
                                //API_Product_Tax = p.Tax,
                                API_Product_UPC = p.UPC,
                                API_Product_Updated = p.Updated,
                                API_Product_UpdateDate = p.UpdateDate,
                                API_Product_UpdateUser = p.UpdateUser,
                                API_Product_Warranty = p.Warranty,
                                API_Product_Weight = p.Weight,
                                API_Product_Width = p.Width,
                                API_Item_Name = i.Name,
                                API_Item_DateDel = i.DateDel,
                                API_Item_DelvType = i.DelvType,
                                API_Item_ItemDesc = i.DescTW,
                                API_Item_ManufactureID = i.ManufactureID,
                                API_Item_Note = i.Note,
                                API_Item_PicEnd = i.PicEnd,
                                API_Item_PriceCard = i.PriceCard,
                                API_Item_PicStart = i.PicStart,
                                API_Item_ProductID = i.ProductID,
                                API_Item_SellerID = i.SellerID,
                                API_Item_CategoryID = i.CategoryID,
                                API_Item_DateEnd = i.DateEnd,
                                //API_Item_DateStart = i.DateStart,
                                API_Item_DelvDate = i.DelvDate,
                                API_Item_DescTW = i.DescTW,
                                API_Item_Inventory = s.Qty - s.QtyReg,
                                //API_Item_IsLimit = i.LimitRule,
                                API_Item_ItemPackage = i.ItemPackage,
                                API_Item_MarketPrice = i.MarketPrice,
                                API_Item_Model = i.Model,
                                API_Item_PriceCash = i.PriceCash,
                                API_Item_Sdesc = i.Sdesc,
                                API_Item_ShipType = i.ShipType,
                                API_Item_Spechead = i.Spechead,
                                API_Item_PriceLocalship = i.PriceLocalship,
                                API_Item_Qty = i.Qty,
                                API_Item_QtyLimit = i.QtyLimit,
                                API_Item_UpdateUser = i.UpdateUser,
                                API_Product_IsChokingDanger = p.IsChokingDanger,
                                API_Product_Cost = p.Cost,
                                API_Product_Is18 = p.Is18,
                                API_Item_ID = i.ID,
                                API_Record_IndustryID = i.CategoryID,
                                API_Item_Status = i.Status,
                                API_Item_IsNew = i.IsNew,

                                API_Item_ItemStockSafeQty = s.SafeQty,
                                API_Itemdisplayprice_GrossMargin = ds.ItemProfitPercent.HasValue ? ds.ItemProfitPercent.Value * 100 : ds.ItemProfitPercent.Value,
                            }).Where(x => x.API_Item_ID == itemID && x.API_Item_Status != 99).Distinct().FirstOrDefault();
            return itemList;
        }
        #endregion

        /// <summary>
        /// 檢查Seller_BasicInfo是否有輸入的SellerID
        /// </summary>
        /// <param name="sellerID">sellerID</param>
        /// <returns>bool</returns>
        private bool CheckSellerExist(int sellerID)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            bool sellerExist = spdb.Seller_BasicInfo.Where(x => x.SellerID == sellerID).Any();
            return sellerExist;
        }

        /// <summary>
        /// CategoryID對應的名稱
        /// </summary>
        /// <param name="categoryID">categoryID</param>
        /// <returns>string</returns>
        private string CategoryID2Title(int categoryID)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            string title = db.Category.Where(x => x.ID == categoryID).Select(x => x.Description).FirstOrDefault();
            return title;
        }

        /// <summary>
        /// 判斷帳戶類型是Seller 還是Vendor
        /// </summary>
        /// <param name="sellerID">由SellerID判斷帳戶類型</param>
        /// <returns>回傳結果S/V</returns>
        private string QueryAccountType(int sellerID)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            return spdb.Seller_BasicInfo.Where(x => x.SellerID == sellerID).Select(x => x.AccountTypeCode).FirstOrDefault();
        }

        /// <summary>
        /// 查詢庫存量
        /// </summary>
        /// <param name="itemID">itemID</param>
        /// <param name="productID">productID</param>
        /// <returns>int</returns>
        private int QueryQty(int itemID, int productID)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            int inventory = 0;
            int tempProductID = 0;
            int itemQty = db.Item.Where(x => x.ID == itemID).Select(x => x.Qty).FirstOrDefault();

            if (productID == 0)
            {
                tempProductID = db.Item.Where(y => y.ID == itemID).Select(y => y.ProductID).FirstOrDefault();
            }
            else
            {
                tempProductID = productID;
            }

            if (itemQty == 0)
            {
                inventory = db.ItemStock.Where(x => x.ProductID == tempProductID).Select(x => x.Qty - x.QtyReg).FirstOrDefault();
            }
            else
            {
                inventory = db.Item.Where(x => x.ID == itemID).Select(x => x.Qty - x.QtyReg).FirstOrDefault();
            }

            return inventory;
        }

        /// <summary>
        /// AddProductDetail判斷商品是否已存在
        /// </summary>
        /// <param name="checkProducts">checkProducts</param>
        /// <returns>bool</returns>
        private bool IsProductDetailExist(List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail> checkProducts)
        {
            bool isProductDetailExist = true;
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_ProductDetail tempList = new DB.TWSELLERPORTALDB.Models.Seller_ProductDetail();

            for (int i = 0; i < checkProducts.Count; i++)
            {
                tempList = checkProducts[i];
                isProductDetailExist = spdb.Seller_ProductDetail.Where(x => x.ProductID == tempList.ProductID
                    && x.SellerProductID == tempList.SellerProductID
                    && x.ManufactureID == tempList.ManufactureID
                    && x.Condition == tempList.Condition
                    && x.ShipType == tempList.ShipType).Any();

                if (isProductDetailExist)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 判斷Qty-QtyReg是否小於0
        /// </summary>
        /// <param name="checkProducts">checkProducts</param>
        /// <returns>bool</returns>
        private bool IsInvalidQty(List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail> checkProducts)
        {
            return checkProducts.Any(x => x.Qty - x.QtyReg < 0);
        }

        /// <summary>
        /// 上架商品售價修改通知PM，接收參數為多筆
        /// </summary>
        /// <param name="itemInfoList">參數為多筆，型別為ItemInfoList</param>
        private void SendMailForPriceChanged(List<ItemInfoList> itemInfoList)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            List<string> mailReceiver = spdb.Seller_User.Where(x => x.GroupID == 5).Select(x => x.UserEmail).ToList();

            foreach (var email in mailReceiver)
            {
                Models.Mail mail = new Models.Mail();
                Models.Connector connector = new Models.Connector();

                mail.UserEmail = email;
                mail.MailType = Models.Mail.MailTypeEnum.PriceChangedMail;
                mail.ItemInfoList = itemInfoList.ToList();
                connector.SendMail(string.Empty, string.Empty, mail);
            }
        }

        /// <summary>
        /// 上架商品售價修改通知PM，接收參數為單筆
        /// </summary>
        /// <param name="itemModel">參數為單筆，型別為APIItemModel</param>
        private void SendMailForPriceChanged(APIItemModel itemModel)
        {
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            List<string> mailReceiver = spdb.Seller_User.Where(x => x.GroupID == 5).Select(x => x.UserEmail).ToList();

            List<ItemInfoList> itemInfoList = new List<ItemInfoList>();
            ItemInfoList itemInfo = new ItemInfoList();

            itemInfo.SellerName = spdb.Seller_BasicInfo.Where(x => x.SellerID == itemModel.API_Item_SellerID).Select(x => x.SellerName).FirstOrDefault();
            itemInfo.ItemName = itemModel.API_Item_Name;
            itemInfo.ItemID = itemModel.API_Item_ID;
            itemInfo.OriginalItemPriceCash = itemModel.OriginalItemPriceCash;
            itemInfo.ItemPriceCash = itemModel.API_Item_PriceCash;
            itemInfoList.Add(itemInfo);

            foreach (var email in mailReceiver)
            {
                Models.Mail mail = new Models.Mail();
                Models.Connector connector = new Models.Connector();

                mail.UserEmail = email;
                mail.MailType = Models.Mail.MailTypeEnum.PriceChangedMail;
                mail.ItemInfoList = itemInfoList.ToList();
                connector.SendMail(string.Empty, string.Empty, mail);
            }
        }

        #endregion

        #region ItemList
        /// <summary>
        /// 輸入查詢條件查詢ItemList
        /// </summary>
        /// <param name="itemSearch">itemSearch</param>
        /// <returns>ActionResponse</returns>
        public Models.ActionResponse<List<Models.ItemInfoList>> ItemList(TWNewEgg.API.Models.ItemSearchCondition itemSearch)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            Models.ActionResponse<List<Models.ItemInfoList>> result = new Models.ActionResponse<List<Models.ItemInfoList>>();

            // 檢查輸入的Seller是否存在
            if (this.CheckSellerExist(itemSearch.SellerID))
            {
                // 搜尋條件搜尋結果
                var itemQueryable = this.ItemSearchResult(itemSearch);
                List<Models.ItemInfoList> itemInfoList = new List<ItemInfoList>();

                itemInfoList = itemQueryable.ToList();

                // 紀錄符合條件總筆數(實際回傳筆數只有PageSize筆)
                int totalCount = itemInfoList.Count();

                // 若有分頁資訊則進行API分頁功能
                if (itemSearch.PageInfo != null)
                {
                    itemQueryable = itemQueryable.Skip(itemSearch.PageInfo.PageIndex * itemSearch.PageInfo.PageSize).Take(itemSearch.PageInfo.PageSize).AsQueryable();
                }
                else
                {
                    itemQueryable = itemQueryable.AsQueryable();
                }

                // 進行分頁語法產生後才實際從資料庫撈回資料
                itemInfoList = itemQueryable.ToList();

                foreach (var itemInfo in itemInfoList)
                {
                    // 商品成色
                    int status = itemInfo.ProductStatusInt;
                    switch (status)
                    {
                        case 0:
                            itemInfo.ProductStatus = "未標";
                            break;
                        case 1:
                            itemInfo.ProductStatus = "新品";
                            break;
                        case 2:
                            itemInfo.ProductStatus = "拆封";
                            break;
                    }

                    #region 轉換商品成色的值

                    // 將前台商品成色的值，轉換為 Seller Portal 商品成色的值
                    switch (itemInfo.IsNew)
                    {
                        case "Y":
                            {
                                itemInfo.Condition = 1;
                                break;
                            }
                        case "N":
                            {
                                itemInfo.Condition = 2;
                                break;
                            }
                        default:
                            {
                                itemInfo.Condition = 0;
                                break;
                            }
                    }

                    #endregion 轉換商品成色的值

                    // SellerID=>Name
                    //itemInfo.SellerName = spdb.Seller_BasicInfo.Where(x => x.SellerID == itemInfo.ItemSellerID).Select(x => x.SellerName).FirstOrDefault();
                    //// 使用商品類別 layer2 搜尋 商品子類別 layer1
                    //itemInfo.SubcategoryID = db.Category.Where(x => x.ID == itemInfo.ItemCategoryID).Select(x => x.ParentID).FirstOrDefault();
                    //// 使用商品子類別 layer1 搜尋 商品主類別 layer0
                    //itemInfo.Industry = db.Category.Where(x => x.ID == itemInfo.SubcategoryID).Select(x => x.ParentID).FirstOrDefault();
                    //// 類別ID 轉成 類別名稱
                    //itemInfo.ItemCategoryName = this.CategoryID2Title(itemInfo.ItemCategoryID);
                    //itemInfo.SubcategoryName = this.CategoryID2Title(db.Category.Where(x => x.ID == itemInfo.ItemCategoryID).Select(x => x.ParentID).FirstOrDefault());
                    //itemInfo.IndustryName = this.CategoryID2Title(db.Category.Where(x => x.ID == itemInfo.SubcategoryID).Select(x => x.ParentID).FirstOrDefault());
                    // 製造商ID 轉成 製造商名稱
                    //itemInfo.ManufacturerName = spdb.Seller_ManufactureInfo.Where(x => x.SN == itemInfo.ItemManufacturerID).Select(x => x.ManufactureName).FirstOrDefault();
                    
                    // 使用ItemID產生對應圖片網址                    
                    string pid = itemInfo.ItemID.ToString("00000000");
                    string pidf4 = pid.Substring(0, 4);
                    string pidl4 = pid.Substring(4, 4);
                    itemInfo.ImageSource = images + "/pic/item/" + pidf4 + "/" + pidl4 + "_1_60.jpg";
                    // 賣場連結
                    itemInfo.ItemUrl = this.webSite + "/item/itemdetail?item_id=" + itemInfo.ItemID;
                    // 轉成UTC格式給JSON才不會-8，但此處時間內容是本地時間
                    itemInfo.ItemCreateDate = itemInfo.ItemCreateDate.ToUniversalTime().AddHours(8);
                    itemInfo.ItemDateStart = itemInfo.ItemDateStart.ToUniversalTime().AddHours(8);
                    itemInfo.ItemDateEnd = itemInfo.ItemDateEnd.ToUniversalTime().AddHours(8);

                    #region

                    try
                    {
                        List<int> categoryIDCell = db.ItemCategory.Where(x => x.ItemID == itemInfo.ItemID && x.FromSystem == "1").Select(x => x.CategoryID).ToList();

                        if (categoryIDCell.Count > 0)
                        {
                            itemInfo.ItemCategory.SubCategoryID_1_Layer2 = categoryIDCell[0];
                            itemInfo.ItemCategory.SubCategoryID_1_Layer2_Name = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_1_Layer2).Select(x => x.Description).FirstOrDefault();
                            itemInfo.ItemCategory.SubCategoryID_1_Layer1 = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_1_Layer2).Select(x => x.ParentID).FirstOrDefault();
                            itemInfo.ItemCategory.SubCategoryID_1_Layer1_Name = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_1_Layer1).Select(x => x.Description).FirstOrDefault();

                            if (categoryIDCell.Count == 2)
                            {
                                itemInfo.ItemCategory.SubCategoryID_2_Layer2 = categoryIDCell[1];
                                itemInfo.ItemCategory.SubCategoryID_2_Layer2_Name = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_2_Layer2).Select(x => x.Description).FirstOrDefault();
                                itemInfo.ItemCategory.SubCategoryID_2_Layer1 = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_2_Layer2).Select(x => x.ParentID).FirstOrDefault();
                                itemInfo.ItemCategory.SubCategoryID_2_Layer1_Name = db.Category.Where(x => x.ID == itemInfo.ItemCategory.SubCategoryID_2_Layer1).Select(x => x.Description).FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Msg = "發生 exception，取得跨分類資訊失敗。";
                        log.Info("取得跨分類資訊失敗(exception)：(ItemSketchID = " + itemInfo.ItemID + ") " + ex.ToString());
                    }


                    #endregion
                }

                if (itemInfoList.Count == 0)
                {
                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "查無商品";
                }
                else
                {
                    result.Code = (int)ResponseCode.Success;
                    result.IsSuccess = true;
                    result.Msg = totalCount.ToString();
                    result.Body = itemInfoList;
                }
            }
            else
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "查無此Seller";
            }

            return result;
        }
        #endregion

        #region ItemFBN
        /// <summary>
        /// 列出指定Seller由新蛋運送之商品清單
        /// </summary>
        /// <param name="itemSearch">itemSearch</param>
        /// <returns>ActionResponse</returns>
        public Models.ActionResponse<List<ItemInfoList>> ItemFBNList(Models.ItemSearchCondition itemSearch)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            Models.ActionResponse<List<ItemInfoList>> result = new Models.ActionResponse<List<ItemInfoList>>();

            // 檢查輸入的Seller是否存在
            if (this.CheckSellerExist(itemSearch.SellerID))
            {
                var itemQueryable = this.ItemSearchResult(itemSearch);

                List<Models.ItemInfoList> itemFBNList = new List<ItemInfoList>();

                itemQueryable = itemQueryable.Where(x => x.ItemShipType == "N").OrderBy(x => x.ItemID).AsQueryable();

                // 紀錄符合條件總筆數(實際回傳筆數只有PageSize筆)
                int totalCount = itemQueryable.Count();

                // API分頁
                if (itemSearch.PageInfo != null)
                {
                    itemQueryable = itemQueryable.Skip(itemSearch.PageInfo.PageIndex * itemSearch.PageInfo.PageSize).Take(itemSearch.PageInfo.PageSize).AsQueryable();
                }
                else
                {
                    itemQueryable = itemQueryable.AsQueryable();
                }

                itemFBNList = itemQueryable.ToList();

                foreach (var itemFBN in itemFBNList)
                {
                    // 欄位資料處理 Jack.W.Wu
                    // SellerID=>Name
                    itemFBN.SellerName = spdb.Seller_BasicInfo.Where(x => x.SellerID == itemFBN.ItemSellerID).Select(x => x.SellerName).FirstOrDefault();
                    // ProductStatus 商品成色
                    itemFBN.ProductStatus = db.Product.Where(x => x.ID == itemFBN.ProductID).Select(x => x.Status).FirstOrDefault().ToString();
                    // CategoryID=>商品子類別
                    itemFBN.SubcategoryID = db.Category.Where(x => x.ID == itemFBN.ItemCategoryID).Select(x => x.ParentID).FirstOrDefault();
                    // CategoryID=>商品主類別
                    itemFBN.Industry = db.Category.Where(x => x.ID == itemFBN.SubcategoryID).Select(x => x.ParentID).FirstOrDefault();
                    // CategoryID=>Name
                    itemFBN.IndustryName = this.CategoryID2Title(db.Category.Where(x => x.ID == itemFBN.SubcategoryID).Select(x => x.ParentID).FirstOrDefault());
                    itemFBN.SubcategoryName = this.CategoryID2Title(db.Category.Where(x => x.ID == itemFBN.ItemCategoryID).Select(x => x.ParentID).FirstOrDefault());
                    itemFBN.ItemCategoryName =
                        itemFBN.SubcategoryName != null ?
                        itemFBN.SubcategoryName + "&" + this.CategoryID2Title(itemFBN.ItemCategoryID) :
                        itemFBN.ItemCategoryName = this.CategoryID2Title(itemFBN.ItemCategoryID);

                    // ItemID=>Item Pic URL
                    string pid = itemFBN.ItemID.ToString("00000000");
                    string pidf4 = pid.Substring(0, 4);
                    string pidl4 = pid.Substring(4, 4);
                    itemFBN.ImageSource = images + "/pic/item/" + pidf4 + "/" + pidl4 + "_1_60.jpg";

                    // 製造商ID=>Name
                    itemFBN.ManufacturerName = spdb.Seller_ManufactureInfo.Where(x => x.SN == itemFBN.ItemManufacturerID).Select(x => x.ManufactureName).FirstOrDefault();

                    //賣場連結
                    itemFBN.ItemUrl = this.webSite + "/item/itemdetail?item_id=" + itemFBN.ItemID;
                }

                if (itemFBNList.Count == 0)
                {
                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "查無商品";
                }
                else
                {
                    result.Code = (int)ResponseCode.Success;
                    result.IsSuccess = true;
                    result.Msg = totalCount.ToString();
                    result.Body = itemFBNList;
                }
            }
            else
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "查無此Seller";
            }

            return result;
        }

        #endregion

        #region Search function
        /// <summary>
        /// 四種搜尋模式及進階查詢
        /// </summary>
        /// <param name="itemSearch">itemSearch</param>
        /// <returns>IQueryable</returns>
        private IQueryable<Models.ItemInfoList> ItemSearchResult(TWNewEgg.API.Models.ItemSearchCondition itemSearch)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            // 查詢特定seller 所有商品
            var itemList = this.QueryItemList(itemSearch.SellerID).AsQueryable();

            switch (itemSearch.SearchMode)
            {
                // 商家商品編號
                case 0:
                    if (itemSearch.Keyword != string.Empty && itemSearch.Keyword != null)
                    {
                        itemList = itemList.Where(x => x.ProductSellerProductID == itemSearch.Keyword).AsQueryable();
                    }

                    break;
                // 廠商產品編號
                case 1:
                    if (itemSearch.Keyword != string.Empty && itemSearch.Keyword != null)
                    {
                        itemList = itemList.Where(x => x.ProductManufacturerPartNum == itemSearch.Keyword).AsQueryable();
                    }

                    break;
                // 新蛋商品編號
                case 2:
                    if (itemSearch.Keyword != string.Empty && itemSearch.Keyword != null)
                    {
                        int intKeyword = -1;
                        Int32.TryParse(itemSearch.Keyword, out intKeyword);
                        itemList = itemList.Where(x => x.ItemID == intKeyword).AsQueryable();
                    }

                    break;
                // 商品描述
                case 3:
                    if (itemSearch.Keyword != string.Empty && itemSearch.Keyword != null)
                    {
                        itemList = itemList.Where(x => x.ItemName.Contains(itemSearch.Keyword)).AsQueryable();
                    }

                    break;
                // 綜合搜尋
                case 4:
                    if (itemSearch.Keyword != string.Empty && itemSearch.Keyword != null)
                    {
                        int intKeyword = 0;
                        bool isInt = Int32.TryParse(itemSearch.Keyword, out intKeyword);
                        if (isInt == false)
                        {
                            intKeyword = -1;
                        }

                        TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

                        var manufacturerInfo = spdb.Seller_ManufactureInfo.ToList();
                        if (manufacturerInfo.Where(x => x.ManufactureName == itemSearch.Keyword).Select(x => x.SN).Any())
                        {
                            int tempManufacturerID = manufacturerInfo.Where(x => x.ManufactureName == itemSearch.Keyword).Select(x => x.SN).FirstOrDefault();

                            itemList = itemList.Where(
                                x => x.ItemName.Contains(itemSearch.Keyword)
                                    || x.ProductManufacturerPartNum == itemSearch.Keyword
                                    || x.ProductSellerProductID == itemSearch.Keyword
                                    || x.ItemID == intKeyword
                                    || x.ItemManufacturerID == intKeyword
                                    || x.ItemManufacturerID == tempManufacturerID).AsQueryable();
                        }
                        else
                        {
                            itemList = itemList.Where(
                                x => x.ItemName.Contains(itemSearch.Keyword)
                                    || x.ProductManufacturerPartNum == itemSearch.Keyword
                                    || x.ProductSellerProductID == itemSearch.Keyword
                                    || x.ItemID == intKeyword
                                    || x.ItemManufacturerID == intKeyword).AsQueryable();
                        }
                    }

                    break;
            }
            // 進階搜尋
            // 審核狀態
            if (itemSearch.CheckStatus.HasValue && itemSearch.CheckStatus != -1)
            {
                itemList = itemList.Where(x => x.Status == itemSearch.CheckStatus).AsQueryable();
            }
            // 商品狀態
            if (itemSearch.Status.HasValue)
            {
                itemList = itemList.Where(x => x.ItemStatus == itemSearch.Status).AsQueryable();
            }
            // 製造商
            if (itemSearch.Manufacturer.HasValue)
            {
                itemList = itemList.Where(x => x.ItemManufacturerID == itemSearch.Manufacturer).AsQueryable();
            }
            // 商品成色
            if (itemSearch.ItemCondition.HasValue)
            {
                itemList = itemList.Where(x => x.ProductStatusInt == itemSearch.ItemCondition).AsQueryable();
            }
            // 運費
            if (itemSearch.Shipping.HasValue)
            {
                itemList = itemList.Where(x => x.ItemShipFee == itemSearch.Shipping).AsQueryable();
            }

            // 以主類別做搜尋(layer0)
            // Item及Product只有紀錄商品類別(layer2)，所以必須由搜尋條件(主類別layer0)撈出底下所有包含的子類別(layer1)，最後才用子類別(layer)撈出符合的商品類別(layer2)回傳。
            if (itemSearch.Industry.HasValue)
            {
                List<Models.ItemInfoList> tempItemList = itemList.ToList();
                List<Models.ItemInfoList> resultList = new List<Models.ItemInfoList>();
                List<int> matchID = new List<int>();

                // 第0層撈第1層
                List<int> subcategoryID = this.QueryCategory(1, itemSearch.Industry.Value).Body.Select(x => x.ID).ToList();
                // 撈到所有第1層後，往下撈各自的第2層
                for (int i = 0; i < subcategoryID.Count; i++)
                {
                    int tempSubID = subcategoryID[i];
                    List<int> tempMatchID = db.Category.Where(x => x.ParentID == tempSubID).Select(x => x.ID).ToList();
                    foreach (var x in tempMatchID)
                    {
                        matchID.Add(x);
                    }
                }
                // 將ItemCategoryID和撈取的第2層條件做比對
                for (int i = 0; i < tempItemList.Count(); i++)
                {
                    int tempID = tempItemList[i].ItemCategoryID;
                    if (matchID.Contains(tempID))
                    {
                        resultList.Add(tempItemList[i]);
                    }
                }

                itemList = resultList.AsQueryable();
            }

            // 以子類別做搜尋(layer1)
            // 由搜尋條件(子類別layer1)撈出底下所有包含的商品類別(layer2)結果回傳。
            if (itemSearch.SubCategory.HasValue)
            {
                List<Models.ItemInfoList> tempItemList = itemList.ToList();
                List<Models.ItemInfoList> resultList = new List<Models.ItemInfoList>();
                // 從第1層撈第2層
                List<int> matchID = db.Category.Where(x => x.ParentID == itemSearch.SubCategory).Select(x => x.ID).ToList();
                // 將ItemCategoryID和撈取的第2層條件做比對
                for (int i = 0; i < tempItemList.Count(); i++)
                {
                    int tempID = tempItemList[i].ItemCategoryID;
                    if (matchID.Contains(tempID))
                    {
                        resultList.Add(tempItemList[i]);
                    }
                }

                itemList = resultList.AsQueryable();
            }

            // 由搜尋條件(商品類別layer3)撈出底下所有包含的商品結果回傳。
            if (itemSearch.ItemCategory.HasValue)
            {
                log.Info("查詢第三層類別商品");
                List<Models.ItemInfoList> tempItemList = itemList.ToList();
                List<Models.ItemInfoList> resultList = new List<Models.ItemInfoList>();
                // 從第2層撈第3層
                int? matchID = itemSearch.ItemCategory;
                // 將ItemCategoryID和撈取的第3層條件做比對
                for (int i = 0; i < tempItemList.Count(); i++)
                {
                    int tempID = tempItemList[i].ItemCategoryID;
                    if (tempID == matchID)
                    {
                        resultList.Add(tempItemList[i]);
                    }
                }

                itemList = resultList.AsQueryable();
            }

            // 以庫存量做搜尋
            if (itemSearch.Inventory.HasValue && itemSearch.Inventory != 0)
            {
                if (itemSearch.Inventory <= 100)
                {
                    // 庫存量輸入<100列出小於輸入量的所有商品
                    itemList = itemList.Where(x => x.ProductInventory < itemSearch.Inventory).AsQueryable();
                }
                else
                {
                    // 庫存量輸入>=100列出大於等於輸入量的所有商品
                    itemList = itemList.Where(x => x.ProductInventory >= 100).AsQueryable();
                }
            }

            // 依建立日期做搜尋，使用AsQueryable無法在Linq查詢中加入部分function所以先轉成list做處理。
            switch (itemSearch.CreateDateBefore)
            {
                case 1:
                case 3:
                case 7:
                case 30:
                    if (itemSearch.CreateDateBefore.HasValue)
                    {
                        // 輸入天數，搜尋距離現在日期前幾天建立的商品
                        var tempItemList = itemList.ToList();
                        tempItemList = tempItemList.Where(x => DateTime.Compare(x.ItemCreateDate, DateTime.Now.AddDays(-itemSearch.CreateDateBefore.Value)) > 0).ToList();
                        itemList = tempItemList.AsQueryable();
                    }
                    break;
                case 2:
                    if (itemSearch.CreateDateStart.HasValue && itemSearch.CreateDateEnd == null)
                    {
                        // 只輸入開始日期，搜尋特定日期
                        var tempItemList = itemList.ToList();
                        itemSearch.CreateDateStart = itemSearch.CreateDateStart.Value.AddHours(8);
                        tempItemList = tempItemList.Where(x => x.ItemCreateDate.DayOfYear == itemSearch.CreateDateStart.Value.DayOfYear).ToList();
                        itemList = tempItemList.AsQueryable();
                    }
                    break;
                case 4:
                    if (itemSearch.CreateDateStart.HasValue && itemSearch.CreateDateEnd.HasValue)
            {
                        // 輸入開始及結束日期區間，搜尋區間中所建立的商品
                        var tempItemList = itemList.ToList();
                        itemSearch.CreateDateEnd = itemSearch.CreateDateEnd.Value.AddHours(8);
                        tempItemList = tempItemList.Where(x => DateTime.Compare(x.ItemCreateDate, itemSearch.CreateDateStart.Value) > 0 && DateTime.Compare(x.ItemCreateDate, itemSearch.CreateDateEnd.Value.AddDays(1)) < 0).ToList();
                        itemList = tempItemList.AsQueryable();
            }
                    break;
            }
            // 查詢結果排序後回傳
            return this.SortingItems(itemSearch, itemList.OrderBy(x => x.ItemID).AsQueryable());
        }

        #endregion

        #region Sorting
        /// <summary>
        /// 排序邏輯
        /// </summary>
        /// <param name="itemSearch">itemSearch</param>
        /// <param name="itemList">itemList</param>
        /// <returns>排序結果</returns>
        public IQueryable<ItemInfoList> SortingItems(ItemSearchCondition itemSearch, IQueryable<ItemInfoList> itemList)
        {
            // 設定排序方式
            if (!string.IsNullOrEmpty(itemSearch.SortField))
            {
                // 利用字串判斷ItemInfoList排序欄位 0430 因為改用AsQueryable停用
                //switch (itemSearch.SortType.ToLower())
                //{
                //    default:
                //        itemList = itemList.OrderBy(x => x.GetType().GetProperty(itemSearch.SortField).GetValue(x)).AsQueryable();
                //        break;
                //    case "asc":
                //        itemList = itemList.OrderBy(x => x.GetType().GetProperty(itemSearch.SortField).GetValue(x)).AsQueryable();
                //        break;
                //    case "desc":
                //        itemList = itemList.OrderByDescending(x => x.GetType().GetProperty(itemSearch.SortField).GetValue(x)).AsQueryable();
                //        break;
                //}
                if (itemSearch.SortType != "desc")
                {
                    // 預設以ItemID排序
                    switch (itemSearch.SortField.ToLower())
                    {
                        default:
                            itemList = itemList.OrderBy(x => x.ItemID).AsQueryable();
                            break;
                        case "itemstatus":
                            itemList = itemList.OrderBy(x => x.ItemStatus).AsQueryable();
                            break;
                        case "itemname":
                            itemList = itemList.OrderBy(x => x.ItemName).AsQueryable();
                            break;
                        case "itemsellerid":
                            itemList = itemList.OrderBy(x => x.ItemSellerID).AsQueryable();
                            break;
                        case "productsellerproductid":
                            itemList = itemList.OrderBy(x => x.ProductSellerProductID).AsQueryable();
                            break;
                        case "productupc":
                            itemList = itemList.OrderBy(x => x.ProductUPC).AsQueryable();
                            break;
                        case "itemid":
                            itemList = itemList.OrderBy(x => x.ItemID).AsQueryable();
                            break;
                        case "productid":
                            itemList = itemList.OrderBy(x => x.ProductID).AsQueryable();
                            break;
                        case "manufacturername":
                            itemList = itemList.OrderBy(x => x.ItemManufacturerID).AsQueryable();
                            break;
                        case "productmanufacturerpartnum":
                            itemList = itemList.OrderBy(x => x.ProductManufacturerPartNum).AsQueryable();
                            break;
                        case "itemshiptype":
                            itemList = itemList.OrderBy(x => x.ItemShipType).AsQueryable();
                            break;
                        case "itemqty":
                            itemList = itemList.OrderBy(x => x.ItemQty).AsQueryable();
                            break;
                        case "productinventory":
                            itemList = itemList.OrderBy(x => x.ProductInventory).AsQueryable();
                            break;
                        case "productqty":
                            itemList = itemList.OrderBy(x => x.ProductQty).AsQueryable();
                            break;
                        case "productreg":
                            itemList = itemList.OrderBy(x => x.ProductReg).AsQueryable();
                            break;
                        case "itemmarketprice":
                            itemList = itemList.OrderBy(x => x.ItemMarketPrice).AsQueryable();
                            break;
                        case "itempricecash":
                            itemList = itemList.OrderBy(x => x.ItemPriceCash).AsQueryable();
                            break;
                        case "itemshipfee":
                            itemList = itemList.OrderBy(x => x.ItemShipFee).AsQueryable();
                            break;
                        case "itemdatestart":
                            itemList = itemList.OrderBy(x => x.ItemDateStart).AsQueryable();
                            break;
                        case "itemdateend":
                            itemList = itemList.OrderBy(x => x.ItemDateEnd).AsQueryable();
                            break;
                        case "itemcreatedate":
                            itemList = itemList.OrderBy(x => x.ItemCreateDate).AsQueryable();
                            break;
                        case "industryname":
                            itemList = itemList.OrderBy(x => x.IndustryName).AsQueryable();
                            break;
                        case "subcategoryname":
                            itemList = itemList.OrderBy(x => x.SubcategoryName).AsQueryable();
                            break;
                        case "itemcategoryname":
                            itemList = itemList.OrderBy(x => x.ItemCategoryName).AsQueryable();
                            break;
                    }
                }
                else
                {
                    switch (itemSearch.SortField.ToLower())
                    {
                        // 預設以ItemID排序
                        default:
                            itemList = itemList.OrderByDescending(x => x.ItemID).AsQueryable();
                            break;
                        case "itemstatus":
                            itemList = itemList.OrderByDescending(x => x.ItemStatus).AsQueryable();
                            break;
                        case "itemname":
                            itemList = itemList.OrderByDescending(x => x.ItemName).AsQueryable();
                            break;
                        case "itemsellerid":
                            itemList = itemList.OrderByDescending(x => x.ItemSellerID).AsQueryable();
                            break;
                        case "productsellerproductid":
                            itemList = itemList.OrderByDescending(x => x.ProductSellerProductID).AsQueryable();
                            break;
                        case "productupc":
                            itemList = itemList.OrderByDescending(x => x.ProductUPC).AsQueryable();
                            break;
                        case "itemid":
                            itemList = itemList.OrderByDescending(x => x.ItemID).AsQueryable();
                            break;
                        case "productid":
                            itemList = itemList.OrderByDescending(x => x.ProductID).AsQueryable();
                            break;
                        case "manufacturername":
                            itemList = itemList.OrderByDescending(x => x.ItemManufacturerID).AsQueryable();
                            break;
                        case "productmanufacturerpartnum":
                            itemList = itemList.OrderByDescending(x => x.ProductManufacturerPartNum).AsQueryable();
                            break;
                        case "itemshiptype":
                            itemList = itemList.OrderByDescending(x => x.ItemShipType).AsQueryable();
                            break;
                        case "itemqty":
                            itemList = itemList.OrderByDescending(x => x.ItemQty).AsQueryable();
                            break;
                        case "productinventory":
                            itemList = itemList.OrderByDescending(x => x.ProductInventory).AsQueryable();
                            break;
                        case "productqty":
                            itemList = itemList.OrderByDescending(x => x.ProductQty).AsQueryable();
                            break;
                        case "productreg":
                            itemList = itemList.OrderByDescending(x => x.ProductReg).AsQueryable();
                            break;
                        case "itemmarketprice":
                            itemList = itemList.OrderByDescending(x => x.ItemMarketPrice).AsQueryable();
                            break;
                        case "itempricecash":
                            itemList = itemList.OrderByDescending(x => x.ItemPriceCash).AsQueryable();
                            break;
                        case "itemshipfee":
                            itemList = itemList.OrderByDescending(x => x.ItemShipFee).AsQueryable();
                            break;
                        case "itemdatestart":
                            itemList = itemList.OrderByDescending(x => x.ItemDateStart).AsQueryable();
                            break;
                        case "itemdateend":
                            itemList = itemList.OrderByDescending(x => x.ItemDateEnd).AsQueryable();
                            break;
                        case "itemcreatedate":
                            itemList = itemList.OrderByDescending(x => x.ItemCreateDate).AsQueryable();
                            break;
                        case "industryname":
                            itemList = itemList.OrderByDescending(x => x.IndustryName).AsQueryable();
                            break;
                        case "subcategoryname":
                            itemList = itemList.OrderByDescending(x => x.SubcategoryName).AsQueryable();
                            break;
                        case "itemcategoryname":
                            itemList = itemList.OrderByDescending(x => x.ItemCategoryName).AsQueryable();
                            break;
                    }
                }
            }

            return itemList;
        }

        #endregion

        #region ItemModify

        /// <summary>
        /// 使用ItemList對商品修改儲存，修改運送方、狀態、庫存、市場建議售價、售價、運費、成本價(Vendor)
        /// </summary>
        /// <param name="itemInfoList">欲修改的商品原始資料</param>
        /// <returns>ActionRespon包含修改結果</returns>
        public Models.ActionResponse<string> ItemModify(List<Models.ItemInfoList> itemInfoList)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
           
            bool errorData = false;
           
            string errorMsg = string.Empty;
            List<Models.ItemInfoList> priceChangedList = new List<ItemInfoList>();
            List<int> updateIPPItemId = new List<int>();

            foreach (var itemInfo in itemInfoList)
            {
                // 判斷是Seller或Vendor，隨欄位不同影響Deliver Type，回傳 S/V
                string accountType = QueryAccountType(itemInfo.ItemSellerID);
                
                // ItemList 商業邏輯，欄位檢查
                itemModifyBusinessLogic(db, ref errorData, ref errorMsg, itemInfo, priceChangedList, accountType);
                
                // ItemList 資料直接存取
                itemModifyDataAccess(db, itemInfo, accountType);
                
                // 總價化 ItemID 加入
                updateIPPItemId.Add(itemInfo.ItemID);
                // ============ 資料寫入 End ============ 
            }

            // ItemList 處理結果回傳至 UI
            result = itemModifyResult(db, errorData, errorMsg, priceChangedList, updateIPPItemId);
            
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="errorData"></param>
        /// <param name="errorMsg"></param>
        /// <param name="priceChangedList"></param>
        /// <param name="updateIPPItemId"></param>
        /// <returns></returns>
        private Models.ActionResponse<string> itemModifyResult(DB.TWSqlDBContext db, bool errorData, string errorMsg, List<Models.ItemInfoList> priceChangedList, List<int> updateIPPItemId)
        {
            Models.ActionResponse<string> result = new ActionResponse<string>();

            try
            {
                if (errorData == false)
                {
                    // Transcation 宣告，防止失敗資料寫入 DB
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                            result.IsSuccess = false;
                            result.Code = (int)ResponseCode.Error;
                            result.Msg = "例外發生: " + ex.Message;
                            result.Body = null;

                            return result;
                        }
                        scope.Complete();
                    }
                    

                    // 紀錄更新失敗的 ItemID
                    List<int> updateIPPPriceFaildID = new List<int>();
                    log.Info("UpdateIPPID 數量" + updateIPPItemId.Count);

                    log.Info("Update Price, Type:Edit - call IPP API before");
                    var issuccess = this.frontendService.PriceAPI(updateIPPItemId);

                    if (issuccess.IsSuccess == true)
                    {
                        result.Code = (int)ResponseCode.Success;
                        result.IsSuccess = true;
                        result.Msg = "Success";
                        result.Body = "商品資料修改成功";

                        log.Info("商品資料修改成功");
                        // 上架商品且價格有變動，則發信給PM
                        if (priceChangedList.Count != 0)
                        {
                            SendMailForPriceChanged(priceChangedList);
                        }
                    }
                    else
                    {
                        // 修改總價失敗
                        result.Code = (int)ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "商品資料修改成功，但價格計算有誤。請稍後在試，並紀錄新蛋賣場、商品編號連絡新蛋客服。";
                        result.Body = null;
                        log.Info("商品資料修改成功，總價化失敗");
                        foreach (var itemid in updateIPPPriceFaildID)
                            log.Error("總價化失敗 ItemID " + itemid);

                    }
                }
                else
                {
                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "Failure: " + (errorMsg == string.Empty ? "請確認輸入資料" : errorMsg);
                    result.Body = errorMsg == string.Empty ? "請確認輸入資料" : errorMsg;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "商品修改，發生意外錯誤: " + ex.Message;
                result.Body = null;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="errorData"></param>
        /// <param name="errorMsg"></param>
        /// <param name="itemInfo"></param>
        /// <param name="priceChangedList"></param>
        /// <param name="accountType"></param>
        private void itemModifyBusinessLogic(DB.TWSqlDBContext db, ref bool errorData, ref string errorMsg, ItemInfoList itemInfo, List<Models.ItemInfoList> priceChangedList, string accountType)
        {
            bool isSellerItem = false;

            // 驗證欲修改的Item資料是否屬於該Seller
            isSellerItem = db.Item.Where(x => x.SellerID == itemInfo.ItemSellerID && x.ID == itemInfo.ItemID).Any();
            // 判斷使用者輸入的ItemID 及 ProductID 是否匹配
            var isProductMatch = db.Item.Where(x => x.ID == itemInfo.ItemID).Select(x => x.ProductID).FirstOrDefault() == itemInfo.ProductID;

            if (itemInfo.ItemID == 0 || isSellerItem == false || isProductMatch == false)
            {
                errorMsg = "無此商品";
                errorData = true;

            }
            else if (new TimeSpan(itemInfo.ItemDateStart.Ticks - itemInfo.ItemDateEnd.Ticks).Days > 0)
            {
                errorMsg = "NE Item# " + itemInfo.ItemID + ". 上架日期必須早於下架日期";
                errorData = true;
            }
            else if (itemInfo.ItemQty > itemInfo.ProductInventory)
            {
                errorMsg = "NE Item# " + itemInfo.ItemID + ". 賣場限量必須小於等於庫存量";
                errorData = true;
            }

            // 上架商品且價格變動加入發信通知PM的清單
            if (itemInfo.ItemPriceCash != itemInfo.OriginalItemPriceCash && itemInfo.ItemStatus == 0 && itemInfo.AccountTypeCode == "S")
            {
                priceChangedList.Add(itemInfo);
            }

            // (Vendor) 商品售價不得小於成本
            if (itemInfo.ItemPriceCash <= itemInfo.ProductCost && itemInfo.AccountTypeCode == "V")
            {
                errorMsg = "NE Item# " + itemInfo.ItemID + ". 毛利率為負數，請重新設定售價或成本";
                errorData = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="itemInfo"></param>
        /// <param name="accountType"></param>
        private void itemModifyDataAccess(DB.TWSqlDBContext db, ItemInfoList itemInfo, string accountType)
        {
            Item item = new Item();
            ItemStock itemStock = new ItemStock();
            Product product = new Product();

            item = db.Item.Where(x => x.ID == itemInfo.ItemID).FirstOrDefault();
            itemStock = db.ItemStock.Where(x => x.ProductID == itemInfo.ProductID).FirstOrDefault();
            product = db.Product.Where(x => x.ID == itemInfo.ProductID).FirstOrDefault();

            // ============ 資料寫入 Start ============

            // 不用判斷帳戶類型即可修改的欄位
            item.ShipType = itemInfo.ItemShipType;
            item.Status = itemInfo.ItemStatus;
            item.Qty = itemInfo.ItemQty;
            item.MarketPrice = itemInfo.ItemMarketPrice;
            item.PriceLocalship = itemInfo.ItemShipFee.Value;
            item.PriceCash = item.PriceCard = itemInfo.ItemPriceCash;

            // 判斷帳戶類型寫入不同價格欄位，分Seller及Vendor
            // shiptype = 'S' => delvtype = 2
            // shiptype = 'V' => delvtype = 7
            // shiptype = 'N' => delvtype = 8(S) or 9(V)
            switch (accountType)
            {
                case "S":
                    // 由運送方決定DelvType
                    switch (item.ShipType)
                    {
                        case "S":
                            item.DelvType = 2;
                            product.DelvType = 2;
                            break;
                        case "N":
                            item.DelvType = 8;
                            product.DelvType = 8;
                            break;
                    }

                    break;

                case "V":
                    {
                        // 由運送方決定DelvType
                        switch (item.ShipType)
                        {
                            case "V":
                                item.DelvType = 7;
                                product.DelvType = 7;
                                break;
                            case "N":
                                item.DelvType = 9;
                                product.DelvType = 9;
                                break;
                        }

                        log.Info("ItemID: " + itemInfo.ItemID + ", Update User: " + itemInfo.UpdateUserID + ", Product Original Cost: " + product.Cost + ", Product Cost: " + itemInfo.ProductCost);

                        // Vendor同時寫入Cost
                        product.Cost = itemInfo.ProductCost;

                        if (itemInfo.ItemPriceCash < itemInfo.ProductCost)
                        {
                            itemInfo.ItemStatus = 1;
                        }
                        break;
                    }
            }

            // 接到的時間格式為UTC，須轉為Local time。 SQL限制([DateStart]<=[DateEnd] AND [DateEnd]<[DateDel])
            // 輸入時間格式若為預設值不修改
            if (itemInfo.ItemDateStart != DateTime.MinValue && itemInfo.ItemDateEnd != DateTime.MinValue)
            {
                item.DateStart = itemInfo.ItemDateStart.ToLocalTime();
                item.DateEnd = itemInfo.ItemDateEnd.ToLocalTime();
                item.DateDel = item.DateEnd.AddDays(1);
            }
            // Updating UpdateDate & UpdateUser
            item.UpdateDate = DateTime.UtcNow.AddHours(8);
            item.UpdateUser = itemInfo.UpdateUserID.ToString();
            product.UpdateDate = DateTime.UtcNow.AddHours(8);
            product.UpdateUser = itemInfo.UpdateUserID.ToString();

            // 判斷是否寫入itemStock
            if (itemStock.Qty != itemInfo.ProductInventory + itemStock.QtyReg)
            {
                itemStock.Qty = itemInfo.ProductInventory + itemStock.QtyReg;
                itemStock.UpdateDate = DateTime.UtcNow.AddHours(8);
                itemStock.UpdateUser = itemInfo.UpdateUserID.ToString();
            }

            // 將修改價錢的動作記錄下來
            if (itemInfo.ItemPriceCash != itemInfo.OriginalItemPriceCash)
            {
                log.Info("ItemID: " + itemInfo.ItemID + ", Update User: " + itemInfo.UpdateUserID + ", Orignal Price: " + itemInfo.OriginalItemPriceCash + ", Update Price: " + itemInfo.ItemPriceCash);
            }
        }

        /// <summary>
        /// 修改Itemstock 尚未使用。
        /// </summary>
        /// <param name="itemInfoList">itemInfoList</param>
        /// <returns>result</returns>
        public Models.ActionResponse<string> ItemstockModify(List<Models.ItemInfoList> itemInfoList)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            ItemStock itemStock = new ItemStock();
            // List<int> tempProductID = new List<int>();
            Dictionary<int, int> uniqueIDWithQty = new Dictionary<int, int>();
            bool isDuplicateID = false;

            foreach (var itemInfo in itemInfoList)
            {
                if (uniqueIDWithQty.ContainsKey(itemInfo.ProductID))
                {
                    if (uniqueIDWithQty.Where(x => x.Key == itemInfo.ProductID).Select(x => x.Value).FirstOrDefault() != itemInfo.ProductInventory)
                    {
                        isDuplicateID = true;
                    }
                }
                else
                {
                    uniqueIDWithQty.Add(itemInfo.ProductID, itemInfo.ProductInventory);

                    itemStock.Qty = itemInfo.ProductInventory + itemStock.QtyReg;
                    itemStock.UpdateDate = DateTime.UtcNow.AddHours(8);
                    itemStock.UpdateUser = itemInfo.ItemSellerID.ToString();
                }
            }

            if (isDuplicateID == true)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Failure";
                result.Body = "同Product有不同庫存量，請確認資料";
            }
            else
            {
                db.SaveChanges();
                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Success";
                result.Body = "商品資料修改成功";
            }

            return result;
        }

        /// <summary>
        /// 修改由新蛋運送之商品資料，只可修改狀態及UPC
        /// </summary>
        /// <param name="itemInfoList">原始商品資料</param>
        /// <returns>ActionResponse包含修改結果</returns>
        public Models.ActionResponse<string> ShippedByNeweggModify(List<Models.ItemInfoList> itemInfoList)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            Item modItem = new Item();
            Product modProduct = new Product();
            bool errorData = false;
            bool isSellerItem = false;

            foreach (var itemInfo in itemInfoList)
            {
                isSellerItem = db.Item.Where(x => x.SellerID == itemInfo.ItemSellerID && x.ID == itemInfo.ItemID).Any();

                if (itemInfo.ItemID == 0 || isSellerItem == false)
                {
                    errorData = true;
                    break;
                }
                // 0117 mark by Jack.W.Wu FBN修改兩個欄位
                // Item 狀態
                modItem = db.Item.Where(x => x.ID == itemInfo.ItemID).FirstOrDefault();
                modItem.Status = itemInfo.ItemStatus;
                // Product UPC
                modProduct = db.Product.Where(x => x.ID == itemInfo.ProductID).FirstOrDefault();
                modProduct.UPC = itemInfo.ProductUPC;

                // Updating UpdateDate & UpdateUser
                modItem.UpdateDate = DateTime.UtcNow.AddHours(8);
                modItem.UpdateUser = itemInfo.ItemSellerID.ToString();
                modProduct.UpdateDate = DateTime.UtcNow.AddHours(8);
                modProduct.UpdateUser = itemInfo.ItemSellerID.ToString();
            }

            if (errorData == false)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        db.SaveChanges();
                        result.Code = (int)ResponseCode.Success;
                        result.IsSuccess = true;
                        result.Msg = "Success";
                        result.Body = "商品資料修改成功";
                        if (result.IsSuccess == true)
                        {
                            scope.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        result.IsSuccess = false;
                        result.Code = (int)ResponseCode.Error;
                        result.Msg = "例外發生: " + ex.Message;
                        result.Body = null;

                        return result;
                    }
                }
            }
            else
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Failure";
                result.Body = "輸入資料有誤";
            }

            return result;
        }

        #endregion

        #region Delete
        /// <summary>
        /// 刪除賣場，將狀態改為99
        /// </summary>
        /// <param name="deleteItem">ItemInfoList</param>
        /// <returns>回傳成功失敗</returns>
        public Models.ActionResponse<string> DeleteItem(ItemInfoList deleteItem)
        {
            DB.TWSQLDB.Models.Item item = new Item();
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();

            int resultCode = (int)ResponseCode.Error;
            item = db.Item.Where(x => x.ID == deleteItem.ItemID).FirstOrDefault();

            if (item != null)
            {
                resultCode = (int)ResponseCode.Success;
                item.Status = 99;
                item.UpdateUser = deleteItem.UpdateUserID.ToString();
                item.UpdateDate = DateTime.Now;
            }

            switch (resultCode)
            {
                default:
                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "刪除失敗";
                    break;

                case (int)MessageCode.Success:
                    db.SaveChanges();
                    result.Code = (int)ResponseCode.Success;
                    result.IsSuccess = true;
                    result.Msg = "刪除成功";
                    result.Body = "刪除成功";
                    break;
            }

            return result;
        }

        #endregion

        #region UpdateItemStatus

        public ActionResponse<string> UpdateItemStatus(ItemInfoList UpdateStatusItem)
        {
            DB.TWSQLDB.Models.Item item = new Item();
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            
            item = db.Item.Where(x => x.ID == UpdateStatusItem.ItemID).FirstOrDefault();
     
            // Business logic
            CheckUpdateItemStatus(ref db,item, UpdateStatusItem, ref result);

            return result;
        }

        
        private void CheckUpdateItemStatus(ref DB.TWSqlDBContext db, Item item, ItemInfoList UpdateStatusItem, ref ActionResponse<string> result)
        {
            bool isError = false;

            decimal? tmp_productCost = db.Product.Where(x => x.ID == item.ProductID).Select(r => r.Cost).FirstOrDefault();

            // item 不為 null
            if (item != null)
            {
                // 售價不得低於成本
                if (item.PriceCash < tmp_productCost.Value && UpdateStatusItem.AccountTypeCode == "V")
                {
                    isError = true;
                    
                    result.Finish(false, (int)ResponseCode.Error, "上架失敗，商品售價不得低於成本", null);                   
                }

                if (isError == false)
                {
                    item.Status = UpdateStatusItem.ItemStatus;
                    item.UpdateUser = UpdateStatusItem.UpdateUserID.ToString();
                    item.UpdateDate = DateTime.Now;

                    db.SaveChanges();
                    
                    result.Finish(true, (int)ResponseCode.Success, "上架成功", null);
                    log.Info("ItemID: " + UpdateStatusItem.ItemID + ", Status: " + UpdateStatusItem.ItemStatus + ", UpdateUser: " + UpdateStatusItem.UpdateUserID.ToString());
                }              
            }
        }

        #endregion

        #region QueryCategory
        /// <summary>
        /// 由layer和parentID找出符合條件的Category
        /// </summary>
        /// <param name="layer">layer</param>
        /// <param name="parentID">parentID</param>
        /// <returns>查詢結果</returns>
        public Models.ActionResponse<List<DB.TWSQLDB.Models.Category>> QueryCategory(int? layer, int? parentID)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            Models.ActionResponse<List<DB.TWSQLDB.Models.Category>> result = new Models.ActionResponse<List<DB.TWSQLDB.Models.Category>>();
            List<DB.TWSQLDB.Models.Category> categoryList = new List<DB.TWSQLDB.Models.Category>();
            if (layer == null && parentID == null)
            {
                categoryList = db.Category.ToList();
            }
            else
            {
                categoryList = db.Category.Where(x => x.Layer == layer && x.ParentID == parentID).ToList();
            }

            if (categoryList.Count() == 0)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "請確認Layer和Parent ID";
            }
            else
            {
                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = categoryList.Count.ToString();
            }

            // 成功或失敗都要回傳body
            result.Body = categoryList;
            return result;
        }

        /// <summary>
        /// 查詢主類別數量給創建商品目錄使用
        /// </summary>
        /// <returns>Dictionary</returns>
        public Dictionary<string, int> CountCategory()
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            Dictionary<string, int> result = new Dictionary<string, int>();
            List<DB.TWSQLDB.Models.Category> allCategory = db.Category.ToList();
            List<DB.TWSQLDB.Models.Category> industry = allCategory.Where(x => x.Layer == 0).ToList();
            List<DB.TWSQLDB.Models.Category> secondLayer = allCategory.Where(x => x.Layer == 1).ToList();
            List<DB.TWSQLDB.Models.Category> thirdLayer = allCategory.Where(x => x.Layer == 2).ToList();

            for (int i = 0; i < industry.Count; i++)
            {
                int industryID = industry[i].ID;
                List<int> secondLayers = secondLayer.Where(x => x.ParentID == industryID).Select(x => x.ID).ToList();
                int totalCount = 0;
                foreach (var item in secondLayers)
                {
                    totalCount += thirdLayer.Where(x => x.ParentID == item).Count();
                }

                result.Add(industryID.ToString(), totalCount);
                totalCount = 0;
            }

            return result;
        }

        #endregion

        #region 寄倉
        /// <summary>
        /// 寄倉最後儲存
        /// </summary>
        /// <param name="addProducts">addProducts</param>
        /// <returns>Models.ActionResponse</returns>
        public Models.ActionResponse<string> AddProductDetail(List<Models.ProductDetail> addProducts)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            int resultCode = -1;

            AutoMapper.Mapper.CreateMap<API.Models.ProductDetail, TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>();
            var mapperItems = AutoMapper.Mapper.Map<List<API.Models.ProductDetail>, List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>>(addProducts);

            // 判斷新增資料是否存在
            if (this.IsProductDetailExist(mapperItems) == false && !this.IsInvalidQty(mapperItems) && !this.IsProductDetailExist(mapperItems))
            {
                for (int i = 0; i < addProducts.Count; i++)
                {
                    mapperItems[i].InDate = DateTime.UtcNow.AddHours(8);
                    spdb.Seller_ProductDetail.Add(mapperItems[i]);
                }

                resultCode = (int)MessageCode.Success;
            }
            else if (this.IsInvalidQty(mapperItems))
            {
                resultCode = (int)MessageCode.InvalidQty;
            }
            else if (this.IsProductDetailExist(mapperItems))
            {
                resultCode = (int)MessageCode.ProductExist;
            }

            switch (resultCode)
            {
                case (int)MessageCode.Success:

                    spdb.SaveChanges();
                    result.Code = (int)ResponseCode.Success;
                    result.IsSuccess = true;
                    result.Msg = addProducts.Count().ToString();
                    result.Body = "新增成功";
                    break;

                case (int)MessageCode.ProductExist:

                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "商品已存在";
                    break;

                case (int)MessageCode.InvalidQty:

                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "Qty必須大於等於QtyReg";
                    break;
            }

            return result;
        }
        #endregion

        #region 針對創建商品的欄位搜尋
        /// <summary>
        /// 搜尋功能
        /// </summary>
        /// <param name="sellerID">賣家</param>
        /// <param name="itemID">商品編號</param>
        /// <returns>回復</returns>
        public ActionResponse<APIItemModel> SearchCreatedItem(int sellerID, int itemID)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            ActionResponse<APIItemModel> result = new ActionResponse<APIItemModel>();

            APIItemModel item = new APIItemModel();

            item = QueryItemModel(sellerID, itemID);

            if (item != null)
            {
                // 商品圖
                string pid = item.API_Item_ID.ToString("00000000");
                string pidf4 = pid.Substring(0, 4);
                string pidl4 = pid.Substring(4, 4);
                item.API_Item_ItemImages = new List<string>();
                for (int picIndex = 1; picIndex <= item.API_Item_PicEnd; picIndex++)
                {
                    item.API_Item_ItemImages.Add(this.images + "/pic/item/" + pidf4 + "/" + pidl4 + "_" + picIndex + "_300.jpg");
                }

                //商品屬性
                API.Service.CategoryPropertyService PropertyService = new Service.CategoryPropertyService();
                item.API_Product_Property = new List<SaveProductProperty>();
                item.API_Product_Property = PropertyService.GetProductProperty(item.API_Item_ProductID).Body;

                // 抓主類別編號 先填入CategoryID
                int tempCategoryID = item.API_Item_CategoryID;
                int tempSubCategoryID = db.Category.Where(x => x.ID == tempCategoryID).Select(x => x.ParentID).FirstOrDefault();
                item.API_Record_IndustryID = db.Category.Where(x => x.ID == tempSubCategoryID).Select(x => x.ParentID).FirstOrDefault();

                // 類別地圖：主類別 > 子類別 > 商品類別
                item.API_Record_CategoryNameMap = CategoryID2Title(item.API_Record_IndustryID) + " > " + CategoryID2Title(tempSubCategoryID) + " > " + CategoryID2Title(tempCategoryID);

                // 製造商ID轉名稱
                int tempManuID = item.API_Item_ManufactureID;
                item.API_Record_ManufacturerName = spdb.Seller_ManufactureInfo.Where(x => x.SN == tempManuID).Select(x => x.ManufactureName).FirstOrDefault();

                // 預設指定賣場開始日期為現在
                item.API_Item_DateStart = DateTime.Now;
                
                // 判斷總價化是否有數值，若無則計算帶入初始值
                if (!item.API_Itemdisplayprice_GrossMargin.HasValue)
                    item.API_Itemdisplayprice_GrossMargin = decimal.Round(((item.API_Item_PriceCash - item.API_Product_Cost.Value) / item.API_Item_PriceCash) * 100, 2);

                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "搜尋成功";
                result.Body = item;
            }
            else
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "查無商品";
            }

            return result;
        }

        #endregion

        #region 使用創建商品頁面修改商品詳細資訊
        /// <summary>
        /// 編輯修改商品詳細資料
        /// </summary>
        /// <param name="editedItem">要修改的商品資料</param>
        /// <returns>修改成功或失敗</returns>
        public ActionResponse<string> EditCreatedItem(APIItemModel editedItem)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            // 改三張表
            Product product = new Product();
            Item item = new Item();
            ItemStock itemStock = new ItemStock();

            if (editedItem != null)
            {
                int itemID = editedItem.API_Item_ID;
                int sellerID = editedItem.API_Item_SellerID;
                int productID = editedItem.API_Item_ProductID;

                // 驗證itemID sellerID productID
                bool isSellerItem = db.Item.Where(x => x.SellerID == sellerID && x.ID == itemID && x.ProductID == productID).Any();

                // 資料驗證成功進行修改
                if (isSellerItem)
                {
                    item = db.Item.Where(x => x.ID == itemID).FirstOrDefault();
                    product = db.Product.Where(x => x.ID == productID).FirstOrDefault();
                    itemStock = db.ItemStock.Where(x => x.ProductID == productID).FirstOrDefault();

                    #region Basic Info

                    // ====== Basic info ======
                    item.Name = product.Name = product.NameTW = editedItem.API_Item_Name;
                    product.Description = product.DescriptionTW = item.ItemDesc = item.DescTW = editedItem.API_Product_Description;
                    item.Note = product.Note = editedItem.API_Product_Note ?? string.Empty;
                    item.Model = product.Model = editedItem.API_Product_Model ?? string.Empty;
                    product.UPC = editedItem.API_Product_UPC;
                    product.Length = editedItem.API_Product_Length;
                    product.Width = editedItem.API_Product_Width;
                    product.Height = editedItem.API_Product_Height;
                    product.Weight = editedItem.API_Product_Weight;
                    product.Warranty = editedItem.API_Product_Warranty;

                    #endregion

                    #region Specific Info

                    // ====== Specific Info ======
                    item.ItemPackage = editedItem.API_Item_ItemPackage;
                    product.Status = editedItem.API_Product_Status;
                    item.ShipType = editedItem.API_Item_ShipType;
                    // 只要ShipType有變動，DelvType也要更新對應值
                    item.DelvType = editedItem.API_Item_DelvType;
                    product.DelvType = editedItem.API_Product_DelvType;
                    item.Spechead = editedItem.API_Item_Spechead;
                    item.Sdesc = editedItem.API_Item_Sdesc;
                    item.DelvDate = editedItem.API_Item_DelvDate;
                    item.DateStart = editedItem.API_Item_DateStart.ToLocalTime();
                    item.DateEnd = editedItem.API_Item_DateEnd.ToLocalTime();
                    product.IsShipDanger = editedItem.API_Product_IsShipDanger;
                    product.Is18 = editedItem.API_Product_Is18;
                    product.IsChokingDanger = editedItem.API_Product_IsChokingDanger;
                    // 2014.09.09 紀錄商品成色 add by jack
                    item.IsNew = editedItem.API_Item_IsNew;

                    #endregion

                    #region Price Inventory Setting Info

                    // ====== Price Inventory Setting Info ======
                    product.ManufactureID = editedItem.API_Product_ManufacturerID;
                    item.ManufactureID = editedItem.API_Item_ManufactureID;
                    product.MenufacturePartNum = editedItem.API_Product_MenufacturePartNum;
                    product.SellerProductID = editedItem.API_Product_SellerProductID;
                    product.BarCode = editedItem.API_Product_BarCode;
                    item.MarketPrice = editedItem.API_Item_MarketPrice;
                    
                    itemStock.Qty = editedItem.API_Item_Inventory + itemStock.QtyReg;
                    itemStock.SafeQty = editedItem.API_Item_ItemStockSafeQty;

                    item.Qty = editedItem.API_Item_Qty;
                    item.QtyLimit = editedItem.API_Item_QtyLimit;
                    item.PriceLocalship = editedItem.API_Item_PriceLocalship;
                    product.Cost = editedItem.API_Product_Cost;
                    // 2014.07.02 有圖片要由 1 開始
                    if (editedItem.API_Item_ItemImages != null)
                    {
                        item.PicStart = 1;
                        product.PicStart = 1;

                        item.PicEnd = product.PicEnd = editedItem.API_Item_ItemImages.Count();
                    }
                    else
                    {
                        item.PicStart = 0;
                        product.PicStart = 0;
                        item.PicEnd = product.PicEnd = 0;
                    }

                    // 2014.09.29 若商品售價小於成本，即下架  add by jack
                    if ((editedItem.API_Item_PriceCash <= editedItem.API_Product_Cost) && (editedItem.AccountTypeCode == "V"))
                    {
                        item.Status = 1;
                    }

                    
                    // 2014.06.10 新增 pricecard pricecash 修改 jack.c
                    item.PriceCard = editedItem.API_Item_PriceCash;
                    item.PriceCash = editedItem.API_Item_PriceCash;
                    // 修改更新日期及更新者
                    item.UpdateDate = product.UpdateDate = itemStock.UpdateDate = DateTime.Now;
                    item.UpdateUser = product.UpdateUser = itemStock.UpdateUser =
                        editedItem.API_Item_UpdateUser;


                    #endregion
            
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                            result.IsSuccess = false;
                            result.Code = (int)ResponseCode.Error;
                            result.Msg = "例外發生: " + ex.Message;
                            result.Body = null;

                            return result;
                        }
                        scope.Complete();
                    }

                    //修改商品屬性
                    API.Service.CategoryPropertyService PropertyService = new Service.CategoryPropertyService();
                    Models.ActionResponse<string> propertyResult = new ActionResponse<string>();
                    propertyResult = PropertyService.SaveProductPropertyClick(editedItem.API_Product_Property, editedItem.API_Item_ProductID);

                    string itemUpdateResult = string.Empty;

                    itemUpdateResult = this.UpdateTWItemService(item.ID, product.ID);

                    // 判斷是否成功更新總價
                    if (string.IsNullOrWhiteSpace(itemUpdateResult))
                    {
                        result.Code = (int)ResponseCode.Success;
                        result.IsSuccess = true;
                        result.Msg = "修改成功";
                        result.Body = "修改成功";

                        log.Info("商品資料修改成功");
                        
                        // 上架商品且價格變動加入發信通知PM的清單
                        if (editedItem.API_Item_PriceCash != editedItem.OriginalItemPriceCash && editedItem.API_Item_Status == 0)
                        {
                            SendMailForPriceChanged(editedItem);
                        }

                        // 將修改價錢的動作記錄下來
                        if (editedItem.API_Item_PriceCash != editedItem.OriginalItemPriceCash)
                        {
                            log
                            .Info("ItemID: " + editedItem.API_Item_ID + ", Update User: " + editedItem.API_Item_UpdateUser + ", Original Price: " + editedItem.OriginalItemPriceCash + ", Update Price: " + editedItem.API_Item_PriceCash);
                        }
                    }
                    else
                    {
                        result.Code = (int)ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "商品資料修改成功，但商品屬性或價格計算有誤。請稍後在試，並紀錄新蛋賣場、商品編號連絡新蛋客服。";
                        result.Body = null;          
                    }
                }
                else
                {
                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "請確認賣場或商品編號";
                }
            }

            return result;
        }

        /// <summary>
        /// 呼叫台蛋進行商品屬性及總價化更新
        /// </summary>
        /// <param name="ItemID">商品 ItemID</param>
        /// <param name="ProductID">商品 ProductID</param>
        /// <returns>回傳string，Empty代表成功，否則回傳錯誤訊息</returns>
        private string UpdateTWItemService(int ItemID, int ProductID)
        {
            // 回傳更新結果
            string UpdateTWItemResult = string.Empty;

            // 總價化回傳結果，Succes為空字串，否則回傳錯誤訊息
            string priceChangeResult = "default";

            // 商品屬性更興，Success 為空字串，否則回傳錯誤訊息
            string productXMLUpdateResult = "default";

            log.Info("ItemID: " + ItemID + "; Type:EditCreatedItem - call IPP API before");  //2014.7.8 add log by ice
            priceChangeResult = this.frontendService.PriceAPI(ItemID);

            log.Info("Product: " + ProductID + "; Type:EditCreatedItem - call IPP API before");
            productXMLUpdateResult = this.frontendService.PropertyXMLAPI(ProductID).Msg;

            // IPP 總價化更新判斷，更新成功無字串回傳
            if (!string.IsNullOrWhiteSpace(priceChangeResult))
            {
                UpdateTWItemResult = priceChangeResult;

                log.Error("總價化失敗 ItemID: " + ItemID);
            }

            // Product XML 更新判斷，更新成功無字串傳回
            if (!string.IsNullOrWhiteSpace(productXMLUpdateResult))
            {
                UpdateTWItemResult = productXMLUpdateResult;

                log.Error("Product XML 更新失敗 ProductID" + ProductID);
            }
                
            return UpdateTWItemResult;
        }

        public class ActiveItem
        {
            public int SellerID { get; set; }

            public int Status { get; set; }

            public DateTime CreateDate { get; set; }
        }

        /// <summary>
        /// 計算 Seller and Vendor 一共多少商品上架
        /// </summary>
        /// <returns>回傳 Seller and Vendor 共多少商品上架</returns>
        public ActionResponse<Dictionary<string, int>> CountActiveItem(string Date)
        {
            Models.ActionResponse<Dictionary<string, int>> result = new Models.ActionResponse<Dictionary<string, int>>();
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            // 排除條件後，無法在SellerPortal DB找到商品歸屬Seller的商品數量
            int noSellerItemCount = 0;
            try
            {
                DateTime day, endDate;

                DB.TWSqlDBContext db = new DB.TWSqlDBContext();
                List<ActiveItem> itemList = new List<ActiveItem>();
                if (!string.IsNullOrWhiteSpace(Date))
                {
                    // 判斷字串為 ALL 計算全部的
                    if (Date == "ALL")
                    {
                        itemList = (from i in db.Item
                                    select new ActiveItem
                                    {
                                        SellerID = i.SellerID,
                                        Status = i.Status,
                                        CreateDate = i.CreateDate
                                    }).Where(x => (x.SellerID != 1 && x.SellerID != 2 && x.SellerID != 3 && x.SellerID != 4 && x.SellerID != 5) && x.Status == 0).ToList();
                    }
                    else
                    {
                        // 取得特定日期
                        DateTime.TryParse(Date, out day);
                        day = day.Date;
                        endDate = day.AddDays(1);
                        itemList = (from i in db.Item
                                    select new ActiveItem
                                    {
                                        SellerID = i.SellerID,
                                        Status = i.Status,
                                        CreateDate = i.CreateDate
                                    }).Where(x => (x.SellerID != 1 && x.SellerID != 2 && x.SellerID != 3 && x.SellerID != 4 && x.SellerID != 5) && x.Status == 0 && x.CreateDate >= day && x.CreateDate < endDate).ToList();
                    }                   
                }
                else
                {
                    // 若沒給日期，給當天的
                    //DateTime.TryParse(Date, out day);
                    day = DateTime.Now.Date;
                    endDate = day.AddDays(1);

                    itemList = (from i in db.Item
                                select new ActiveItem
                                {
                                    SellerID = i.SellerID,
                                    Status = i.Status,
                                    CreateDate = i.CreateDate
                                }).Where(x => (x.SellerID != 1 && x.SellerID != 2 && x.SellerID != 3 && x.SellerID != 4 && x.SellerID != 5) && x.Status == 0 && x.CreateDate >= day && x.CreateDate < endDate).ToList();
                }
                
                var basicInfo = spdb.Seller_BasicInfo.OrderBy(x => x.SellerID).ToList();

                Dictionary<string, int> accountTypeWithQty = new Dictionary<string, int>();
                accountTypeWithQty.Add("Seller", 0);
                accountTypeWithQty.Add("Vendor", 0);

                foreach (var item in itemList)
                {
                    if (basicInfo.Where(x => x.SellerID == item.SellerID && x.AccountTypeCode == "S").Any())
                    {
                        accountTypeWithQty["Seller"]++;
                    }
                    else if (basicInfo.Where(x => x.SellerID == item.SellerID && x.AccountTypeCode == "V").Any())
                    {
                        accountTypeWithQty["Vendor"]++;
                    }
                    else
                    {
                        // 找不到該商品Seller
                        noSellerItemCount++;
                    }
                }

                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Body = accountTypeWithQty;
                result.Msg = noSellerItemCount.ToString();
                return result;
            }
            catch
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Msg = noSellerItemCount.ToString();
                return result;
            }
        }

        #endregion

        #region 商品審核機制與通知信

        /// <summary>
        /// load 商品內容，條件為商家狀態為"Inactive"，建立日期為(今天 - 1)的，定時每天六點寄信給PM(Seller_User.GroupID = 5)
        /// </summary>
        /// <returns>回傳今日需要審核的 Item 列表</returns>
        public Models.ActionResponse<List<Models.ItemInfoList>> SendPMMail()
        {
            Models.ActionResponse<List<Models.ItemInfoList>> sendResult = new Models.ActionResponse<List<Models.ItemInfoList>>();

            string website = System.Configuration.ConfigurationManager.AppSettings["TWSPHost"];

            TWNewEgg.DB.TWSqlDBContext frontenddb = new DB.TWSqlDBContext();
            TWNewEgg.DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            ItemSearchCondition searchCondition = new ItemSearchCondition();
            Models.Connector connector = new Models.Connector();

            List<Models.ItemInfoList> pmsItem = new List<ItemInfoList>();

            List<int> sendMail_Sellers = new List<int>();
            List<PendingItem> sendMail_Items = new List<PendingItem>();

            DateTime start_today = new DateTime();
            DateTime before_day = new DateTime();
            // 取得今日6點
            DateTime.TryParse(DateTime.Now.ToString("yyyy-MM-dd 06:00:00"), out start_today);
            // 取得昨日6點
            DateTime.TryParse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 06:00:00"), out before_day);

            // 找出今日 6點 到 昨日 6點的 Item ，select 出 sellerID 及 ItemID 
            var itemInfos = frontenddb.Item.Where(x => DateTime.Compare(x.CreateDate, before_day) > 0 && DateTime.Compare(x.CreateDate, start_today) < 0).Select(q => new { SellerID = q.SellerID, ItemID = q.ID }).ToList();

            // 篩選出不重複的 SellerID
            var sellerIDs = itemInfos.GroupBy(x => x.SellerID).Select(g => g.First()).Select(y => y.SellerID).ToList();

            // 篩選出此範圍內的 Item，SellerID 的 SellerStatus == I ，代表需要寄Mail 給 PM 的商品
            foreach (var id in sellerIDs)
            {
                var inactiveID = spdb.Seller_BasicInfo.Where(x => x.SellerID == id && x.SellerStatus == "I").FirstOrDefault();
                if (inactiveID != null)
                {
                    sendMail_Sellers.Add(inactiveID.SellerID);
                }
            }

            if (sendMail_Sellers.Count > 0)
            {
                // 篩選出要寄送提醒的 ItemID
                /*foreach (var id in sendMail_Sellers)
                //{
                //    sendMail_Items.AddRange(itemInfos.Where(x => x.SellerID == id).ToList());
                }*/

                var taa = itemInfos.Where(x => sendMail_Sellers.Contains(x.SellerID)).ToList();

                // 利用 ItemList 找出要的資料
                foreach (var item in taa)
                {
                    searchCondition.SellerID = item.SellerID;
                    searchCondition.SearchMode = 2;
                    searchCondition.Keyword = item.ItemID.ToString();

                    pmsItem.AddRange(this.ItemList(searchCondition).Body);
                }

                switch (website)
                {
                    // 正式環境
                    case "http://sellerportal.newegg.com.tw":
                        {
                            // 取出 GroupID = 5 的 User email (PM) 
                            var neweggPMEmails = spdb.Seller_User.Where(x => x.GroupID == 5).ToList();
                           
                            foreach (var mail in neweggPMEmails)
                            {
                                Models.Mail send_PM_Mail = new Models.Mail();
                                send_PM_Mail.MailType = Models.Mail.MailTypeEnum.Mail_TO_PMs;
                                send_PM_Mail.UserEmail = mail.UserEmail.Trim();
                                send_PM_Mail.ItemInfoList = pmsItem;

                                connector.SendMail(null, null, send_PM_Mail);
                            }
                        }

                        break;
                    default:
                        {
                            // 測試用
                            string usermail = System.Configuration.ConfigurationManager.AppSettings["TestUserEMail"];

                            // 寄送 Email
                            string[] usermail_array = usermail.Split(',');

                            foreach (var mail in usermail_array)
                            {
                                Models.Mail send_PM_Mail = new Models.Mail();
                                send_PM_Mail.MailType = Models.Mail.MailTypeEnum.Mail_TO_PMs;
                                send_PM_Mail.UserEmail = mail.Trim();
                                send_PM_Mail.ItemInfoList = pmsItem;

                                connector.SendMail(null, null, send_PM_Mail);
                            }
                        }

                        break;
                }

                sendResult.IsSuccess = true;
                sendResult.Msg = "今日待審核商品共: " + pmsItem.Count() + "筆";
                sendResult.Code = (int)ResponseCode.Success;
                sendResult.Body = pmsItem;
            }
            else
            {
                sendResult.IsSuccess = true;
                sendResult.Msg = "今日無待審核商品";
                sendResult.Code = (int)ResponseCode.Success;
                sendResult.Body = null;
            }

            return sendResult;
        }

        #endregion       
    
        #region 匯出 Excel

        /// <summary>
        /// 匯出 Excel 列表
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>成功及失敗訊息</returns>
        public API.Models.ActionResponse<string> DownloadItemList(API.Models.DownloadItemListModel dataInfo)
        {
            API.Models.ActionResponse<string> result = new Models.ActionResponse<string>();

            

            // 如果有搜尋條件
            if (dataInfo.itemSearchCondition != null)
            {
                // 取得 Item List
                dataInfo.dataList = this.ItemList(dataInfo.itemSearchCondition).Body;

                // 判斷是否有轉換的資料列表
                if (dataInfo.dataList != null && dataInfo.dataList.Any())
                {
                    // 有資料，匯出指定的顯示格式的 Excel 檔案
                    result.Msg = DataToExcel.Export.ListToExcel(ExportToExcelModel(dataInfo.dataList), dataInfo.fileName, dataInfo.sheetName, dataInfo.titleLine);
                }
                else
                {
                    // 無資料，填寫錯誤訊息
                    result.Msg = "Error: 資料數過多，系統無法匯出。";
                }
            }
            else
            {
                // 沒有搜尋條件，直接輸出有欄位名稱的空白表單
                dataInfo.dataList = new List<ItemInfoList>();
                result.Msg = DataToExcel.Export.ListToExcel(ExportToExcelModel(dataInfo.dataList), dataInfo.fileName, dataInfo.sheetName, dataInfo.titleLine);
            }

            

            // 判斷轉換是否成功
            // 成功：提供下載位置
            // 失敗：顯示錯誤資訊
            if (result.Msg.IndexOf(@"Success") == 0)
            {
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;

                // 讀取成功訊息後面的日期檔名
                string saveDate = result.Msg.Substring(8).Trim();

                // 合併下載路徑
                result.Body = string.Format("{0}{1}_{2}.xls", System.Configuration.ConfigurationSettings.AppSettings["ReturnExcel"], dataInfo.fileName, saveDate);

                result.Msg = "Success";
            }
            else
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Body = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 匯出 Excel 列表
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>成功及失敗訊息</returns>
        public API.Models.ActionResponse<string> DownloadItemListToExcel(API.Models.DownloadItemListModel dataInfo)
        {
            API.Models.ActionResponse<string> result = new Models.ActionResponse<string>();


            log.Info("IndustryID : " + dataInfo.itemSearchCondition.Industry + " at API_itemService_download");
            log.Info("SubCategoryID : " + dataInfo.itemSearchCondition.SubCategory + " at API_itemService_download");
            log.Info("ItemCategoryID : " + dataInfo.itemSearchCondition.ItemCategory + " at API_itemService_download");
            // 如果有搜尋條件
            if (dataInfo.itemSearchCondition != null)
            {
                // 取得 Item List
                dataInfo.dataList = this.ItemList(dataInfo.itemSearchCondition).Body;

                // 判斷是否有轉換的資料列表
                if (dataInfo.dataList != null && dataInfo.dataList.Any())
                {
                    // 有資料，匯出指定的顯示格式的 Excel 檔案
                    result.Msg = DataToExcel.Export.ListToExcel(ExportToExcel(dataInfo.dataList), dataInfo.fileName, dataInfo.sheetName, dataInfo.titleLine);
                }
                else
                {
                    // 無資料，填寫錯誤訊息
                    result.Msg = "查無資料，系統無法匯出。";
                }
            }
            else
            {
                // 沒有搜尋條件，直接輸出有欄位名稱的空白表單
                dataInfo.dataList = new List<ItemInfoList>();
                result.Msg = DataToExcel.Export.ListToExcel(ExportToExcel(dataInfo.dataList), dataInfo.fileName, dataInfo.sheetName, dataInfo.titleLine);
            }



            // 判斷轉換是否成功
            // 成功：提供下載位置
            // 失敗：顯示錯誤資訊
            if (result.Msg.IndexOf(@"Success") == 0)
            {
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;

                // 讀取成功訊息後面的日期檔名
                string saveDate = result.Msg.Substring(8).Trim();

                // 合併下載路徑
                result.Body = string.Format("{0}{1}_{2}.xls", System.Configuration.ConfigurationSettings.AppSettings["ReturnExcel"], dataInfo.fileName, saveDate);

                result.Msg = "Success";
            }
            else
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Body = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 轉成指定的顯示格式
        /// </summary>
        /// <param name="dataList">匯出資訊</param>
        /// <returns>指定格式的匯出列表</returns>
        private List<API.Models.ExportToExcelModel> ExportToExcelModel(List<API.Models.ItemInfoList> dataList)
        {
            List<API.Models.ExportToExcelModel> exportToExcelModel = new List<ExportToExcelModel>();

            foreach (API.Models.ItemInfoList itemListInfo in dataList)
            {
                DB.TWSqlDBContext db = new DB.TWSqlDBContext();

                // 查尋前台 item 其它欄位資料
                API.Models.ExportToExcelModel otherItemInfo = db.Item.Where(x => x.ProductID == itemListInfo.ProductID)
                    .Select(x => new API.Models.ExportToExcelModel
                    {
                        ProductNote = x.Note,
                        ProductShortDesc = x.Sdesc,
                        ProductFeatureTitle = x.Spechead,
                        DelvDay = x.DelvDate
                    }).FirstOrDefault();

                // 查尋前台 product 的保固資料
                int? warranty = db.Product.Where(x => x.ID == itemListInfo.ProductID).Select(x => x.Warranty).FirstOrDefault() ?? null;

                exportToExcelModel.Add(new API.Models.ExportToExcelModel
                {
                    // 新蛋賣場編號
                    ItemID = itemListInfo.ItemID,

                    // 商品名稱(品名)
                    WebsiteShortTitle = itemListInfo.ItemName,

                    // 商品描述(內文)
                    // 此欄位內容會超出 excel 一格的字元數 32767，因此匯出時此欄位直接給空白
                    ProductDescription = string.Empty,

                    // 注意事項
                    // 若查無資料，則填入 null
                    ProductNote = otherItemInfo != null ? otherItemInfo.ProductNote : null,

                    // 簡要描述(主賣點2)
                    // 若查無資料，則填入 null
                    ProductShortDesc = otherItemInfo != null ? otherItemInfo.ProductShortDesc : null,

                    // 簡要條列式描述(主賣點1)
                    // 若查無資料，則填入 null
                    ProductFeatureTitle = otherItemInfo != null ? otherItemInfo.ProductFeatureTitle : null,

                    // 商家商品編號
                    ProductSellerProductID = itemListInfo.ProductSellerProductID,

                    // 成本(seller非必填、vendor必填)
                    Cost = itemListInfo.ProductCost,

                    // 賣價(user價)
                    SellingPrice = itemListInfo.ItemPriceCash,

                    // 市場建議售價
                    MarketPrice = itemListInfo.ItemMarketPrice,

                    // 保固
                    Warranty = warranty,

                    // 庫存量
                    Inventory = itemListInfo.ProductInventory,

                    // 到貨天數
                    // 若查無資料，則填入 null
                    DelvDay = otherItemInfo != null ? otherItemInfo.DelvDay : null
                });
            }

            return exportToExcelModel;
        }

        /// <summary>
        /// 轉成指定的顯示格式
        /// </summary>
        /// <param name="dataList">匯出資訊</param>
        /// <returns>指定格式的匯出列表</returns>
        private List<API.Models.ExportToExcel> ExportToExcel(List<API.Models.ItemInfoList> dataList)
        {
            List<API.Models.ExportToExcel> exportToExcelModel = new List<ExportToExcel>();

            foreach (API.Models.ItemInfoList itemListInfo in dataList)
            {
                DB.TWSqlDBContext db = new DB.TWSqlDBContext();

                string value;
                switch (itemListInfo.ItemStatus)
                {
                    case 0:
                        value = "上架";
                        break;
                    case 1:
                        value = "下架";
                        break;
                    case 2:
                        value = "強制下架";
                        break;
                    case 3:
                        value = "售價異常";
                        break;
                    default:
                        value = string.Empty;
                        break;
                }

                string ShipType;
                switch (itemListInfo.ItemShipType)
                {
                    case "S":
                    case "V":
                        ShipType = "供應商";
                        break;
                    case "N":
                        ShipType = "Newegg";
                        break;
                    default:
                        ShipType = string.Empty;
                        break;

                }

                // 查尋前台 product 的保固資料
                int? warranty = db.Product.Where(x => x.ID == itemListInfo.ProductID).Select(x => x.Warranty).FirstOrDefault() ?? null;

                exportToExcelModel.Add(new API.Models.ExportToExcel
                {
                    // 第一層類別名稱
                    IndustryName = itemListInfo.IndustryName,

                    //第二層類別名稱
                    SubcategoryName = itemListInfo.SubcategoryName,

                    //第三層類別名稱
                    ItemCategoryName = itemListInfo.ItemCategoryName,

                    // 新蛋賣場編號
                    ItemID = itemListInfo.ItemID,

                    // 商家產品編號
                    ProductSellerProductID = itemListInfo.ProductSellerProductID,

                    // 商品名稱(品名)
                    ItemName = itemListInfo.ItemName,

                    // 市場建議售價
                    ItemMarketPrice = itemListInfo.ItemMarketPrice,

                    // 售價
                    ItemPriceCash = itemListInfo.ItemPriceCash,

                    // 成本(seller非必填、vendor必填)
                    ProductCost = itemListInfo.ProductCost,

                    // 庫存量
                    ProductInventory = itemListInfo.ProductInventory,

                    // 商品狀態
                    ItemStatus = value,

                    // 出貨方
                    ItemShipType = ShipType,

                    SubCategoryLayer1_Name = string.IsNullOrEmpty(itemListInfo.ItemCategory.SubCategoryID_1_Layer1_Name) ? null : itemListInfo.ItemCategory.SubCategoryID_1_Layer1_Name + "(" + itemListInfo.ItemCategory.SubCategoryID_1_Layer1 + ")",
                    ItemCategoryLayer1_Name = string.IsNullOrEmpty(itemListInfo.ItemCategory.SubCategoryID_1_Layer2_Name) ? null : itemListInfo.ItemCategory.SubCategoryID_1_Layer2_Name + "(" + itemListInfo.ItemCategory.SubCategoryID_1_Layer2 + ")",
                    SubcategoryLayer2_Name = string.IsNullOrEmpty(itemListInfo.ItemCategory.SubCategoryID_2_Layer1_Name) ? null : itemListInfo.ItemCategory.SubCategoryID_2_Layer1_Name + "(" + itemListInfo.ItemCategory.SubCategoryID_2_Layer1 + ")",
                    ItemCategoryLayer2_Name = string.IsNullOrEmpty(itemListInfo.ItemCategory.SubCategoryID_2_Layer2_Name) ? null : itemListInfo.ItemCategory.SubCategoryID_2_Layer2_Name + "(" + itemListInfo.ItemCategory.SubCategoryID_2_Layer2 + ")"
                });
            }

            return exportToExcelModel;
        }

        #endregion 匯出 Excel
    }
}
