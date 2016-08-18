using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using TWNewEgg.API.Models;
using log4net;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// 處理 Inventory Service 
    /// IsSuccess 只有連線失敗或 DB Error 之類才會是 False
    /// </summary>
    public class ItemInventoryAlertService
    {
        private DB.TWSellerPortalDBContext sellerPortaldb = new DB.TWSellerPortalDBContext();
        private DB.TWSqlDBContext sqldb = new DB.TWSqlDBContext();
        private int totalCount = 0;

        private static ILog log = LogManager.GetLogger(typeof(ItemInventoryAlertService));

        #region 搜尋庫存異常
        /// <summary>
        /// Search ItemInventoryInfo
        /// </summary>
        /// <param name="itemSearch">Contains Keyword、SellerID to search access from Controller </param>
        /// <returns>Search results</returns>
        public API.Models.ActionResponse<List<API.Models.VM_ItemInventoryAlertInfo>> SearchItemInventoryInfo(API.Models.InventoryAlertSearchModel itemSearch)
        {
            API.Models.ActionResponse<List<API.Models.VM_ItemInventoryAlertInfo>> searchQueryResult = new Models.ActionResponse<List<Models.VM_ItemInventoryAlertInfo>>();

            try
            {
                //var itemInventoryEnumberable = this.QueryItemInventoryList(itemSearch.SellerID).AsQueryable();
                var tempConventModel = this.SearchData(itemSearch);
                
                if (tempConventModel.Count() > 0)
                {
                    searchQueryResult.IsSuccess = true;
                    searchQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                    searchQueryResult.Msg = this.totalCount.ToString();
                    searchQueryResult.Body = tempConventModel;
                }
                else
                {
                    searchQueryResult.IsSuccess = true;
                    searchQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                    searchQueryResult.Msg = "No Match Data.";
                    searchQueryResult.Body = null;
                }
            }
            catch (Exception ex)
            {
                searchQueryResult.IsSuccess = false;
                searchQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                searchQueryResult.Msg = ex.ToString();
                searchQueryResult.Body = null;
            }

            return searchQueryResult;
        }

        /// <summary>
        /// Search ItemInventoryInfo
        /// </summary>
        /// <param name="itemSearch">Input Search KeyWord、SellerID</param>
        /// <returns>Search result</returns>
        public List<API.Models.VM_ItemInventoryAlertInfo> SearchData(API.Models.InventoryAlertSearchModel itemSearch)
        {
            List<API.Models.VM_ItemInventoryAlertInfo> returnItemInventorySearchList = new List<Models.VM_ItemInventoryAlertInfo>();
            
            var itemInventoryList = this.QueryItemInventoryList(itemSearch.SellerID).AsQueryable();

            // 供 AddFBNItem 頁面判斷是否存在 DB 內用的
            if (itemSearch.KeyWord == "GetAllItemInventoryInfo")
            {
                returnItemInventorySearchList = this.sellerPortaldb.Seller_ProductDetail.Where(q => q.SellerID == itemSearch.SellerID).Select(x => new Models.VM_ItemInventoryAlertInfo
                {
                    ID = x.ID,
                    Name = x.Name,                    
                    NameTW = x.NameTW,
                    ManufactureID = x.ManufactureID,                   
                    ManufacturePartNum = x.ManufacturePartNum,
                    Condition = x.Condition,
                    ProductID = x.ProductID,
                    SellerID = x.SellerID,
                    InDate = x.InDate,
                    Qty = x.Qty,
                    SafeQty = x.SafeQty,
                    QtyReg = x.QtyReg,
                    ShipType = x.ShipType,
                    SellerProductID = x.SellerProductID,
                    UpdateDate = x.UpdateDate,
                    InUserID = x.InUserID,
                    UPC = x.UPC,
                    Status = x.Status,
                    UpdateUserID = x.UpdateUserID
                }).ToList();

                this.totalCount = returnItemInventorySearchList.Count();
                return returnItemInventorySearchList;
            }

            if (string.IsNullOrEmpty(itemSearch.KeyWord))
            {
                returnItemInventorySearchList = itemInventoryList.Where(x => x.ID != 0).ToList();
            }
            else
            {
                // char[] delimiterChars = { ' ', ',' };
                // string[] Keywords = delEmpty(SearchKeyWord.Split(delimiterChars));

                // 避免廠商名稱是中間有空格的，能夠搜尋出來
                // 避免 DB 內的欄位資料是 Null，改以此方法
                // ReturnSearchDataList.AddRange(TempSearchDataList.Where(x => string.IsNullOrEmpty(x.ManufactureName) ? true : x.ManufactureName.Contains(SearchKeyWord)));

                // foreach (string Keyword in Keywords)
                // {
                    int outNum;
                    if (int.TryParse(itemSearch.KeyWord, out outNum))
                    {
                        returnItemInventorySearchList.AddRange(itemInventoryList.Where(x => x.ProductID == outNum).AsQueryable());
                    }
                    else
                    {
                        // 避免 DB 內的欄位資料是 Null，採用 string.IsNullOrEmpty 判斷
                        returnItemInventorySearchList.AddRange(itemInventoryList.Where(x => string.IsNullOrEmpty(x.SellerProductID) ? false : x.SellerProductID.Contains(itemSearch.KeyWord)).AsQueryable());
                        returnItemInventorySearchList.AddRange(itemInventoryList.Where(x => string.IsNullOrEmpty(x.ManufactureName) ? false : x.ManufactureName.Contains(itemSearch.KeyWord)).AsQueryable());
                        returnItemInventorySearchList.AddRange(itemInventoryList.Where(x => string.IsNullOrEmpty(x.NameTW) ? false : x.NameTW.Contains(itemSearch.KeyWord)).AsQueryable());
                        returnItemInventorySearchList.AddRange(itemInventoryList.Where(x => x.ShipType == itemSearch.KeyWord
                                                                                         || x.ManufacturePartNum == itemSearch.KeyWord).AsQueryable());                  
                    }

                   returnItemInventorySearchList = returnItemInventorySearchList.GroupBy(x => x.ID).Select(g => g.First()).ToList();
                // }
            }

            this.totalCount = returnItemInventorySearchList.Where(x => x.SafeQty > 0).Count();

            foreach (var item in returnItemInventorySearchList)
            {
                // 建立 ItemCondition 狀態
                switch (item.Condition)
                {
                    case 0:
                        item.ItemCondition = "未標";
                        break;
                    case 1:
                        item.ItemCondition = "新品";
                        break;
                    case 2:
                        item.ItemCondition = "拆封";
                        break;
                }
            }

            return this.SortItemInventory(itemSearch, returnItemInventorySearchList);
        }

        /// <summary>
        /// 處理分頁及Sort
        /// </summary>
        /// <param name="itemSearch">Contains pageInfo</param>
        /// <param name="itemInventoryList">The Search results</param>
        /// <returns>Return Search Result</returns>
        private List<API.Models.VM_ItemInventoryAlertInfo> SortItemInventory(API.Models.InventoryAlertSearchModel itemSearch, List<API.Models.VM_ItemInventoryAlertInfo> itemInventoryList)
        {
            if (itemSearch.PageInfo != null)
            {
                itemInventoryList = itemInventoryList.Where(x => x.SafeQty != 0)
                .Skip(itemSearch.PageInfo.PageIndex * itemSearch.PageInfo.PageSize).Take(itemSearch.PageInfo.PageSize).ToList();
            }

            if (!string.IsNullOrEmpty(itemSearch.SortField))
            {
                try
                {
                    switch (itemSearch.SortType.ToLower())
                    { 
                        default:
                            itemInventoryList = itemInventoryList.OrderBy(x => x.GetType().GetProperty(itemSearch.SortField).GetValue(x)).ToList();
                            break;
                        case "asc":
                            itemInventoryList = itemInventoryList.OrderBy(x => x.GetType().GetProperty(itemSearch.SortField).GetValue(x)).ToList();
                            break;
                        case "desc":
                            itemInventoryList = itemInventoryList.OrderByDescending(x => x.GetType().GetProperty(itemSearch.SortField).GetValue(x)).ToList();
                            break;
                    }                 
                }
                catch (Exception)
                {                    
                }
            }

            return itemInventoryList;
        }

        // Join 三張表的內容回傳
        private IQueryable<API.Models.VM_ItemInventoryAlertInfo> QueryItemInventoryList(int sellerID)
        {
            var itemInventoryInfoList = this.sellerPortaldb.Seller_ProductDetail
                .SelectMany(
                x => this.sellerPortaldb.Seller_ManufactureInfo
                .Where(p => p.SN == x.ManufactureID).DefaultIfEmpty(),
                (x, p) => new Models.VM_ItemInventoryAlertInfo
                {
                    ID = x.ID, 
                    Name = x.Name, 
                    // 取得Product內的NameTW
                    NameTW = x.NameTW, 
                    ManufactureID = x.ManufactureID, 
                    // 由製造商的表內取得Name
                    ManufactureName = p.ManufactureName, 
                    ManufacturePartNum = x.ManufacturePartNum, 
                    Condition = x.Condition, 
                    ProductID = x.ProductID, 
                    SellerID = x.SellerID, 
                    InDate = x.InDate, 
                    Qty = x.Qty, 
                    SafeQty = x.SafeQty, 
                    QtyReg = x.QtyReg, 
                    ShipType = x.ShipType, 
                    SellerProductID = x.SellerProductID, 
                    UpdateDate = x.UpdateDate, 
                    InUserID = p.InUserID, 
                    UPC = x.UPC, 
                    Status = x.Status, 
                    UpdateUserID = p.UpdateUserID,
                    ItemCondition = string.Empty 
                }).Where(x => x.SellerID == sellerID && x.SafeQty > 0).AsQueryable(); 
            // Join Product Name 至回傳的 List 內
            foreach (var getProductNamefromTWDB in itemInventoryInfoList)
            {
                // 搜尋 Product Name
                getProductNamefromTWDB.NameTW = this.sqldb.Product.Where(x => x.SellerProductID == getProductNamefromTWDB.SellerProductID && x.SellerID == getProductNamefromTWDB.SellerID).Select(x => x.NameTW).FirstOrDefault();
            }
           
            return itemInventoryInfoList;
        }
        
        /// <summary>
        /// delEmpty
        /// </summary>
        /// <param name="searchWord">The Searchword will delete empty than return</param>
        /// <returns>string[] Search Words</returns>
        private string[] DelEmpty(string[] searchWord)
        {
            List<string> keyWord = new List<string>();
            foreach (var word in searchWord)
            {
                if (word != string.Empty)
                {
                    keyWord.Add(word);
                }
            }

            return keyWord.ToArray();
        }

        #endregion

        #region 刪除庫存警示

        /// <summary>
        /// 刪除 ItemInventoryAlert
        /// </summary>
        /// <param name="deleteInventoryList">Delete inventoryInfo List</param>
        /// <returns>The Delete Query result</returns>
        public API.Models.ActionResponse<List<string>> DeleteInventoryAlert(List<API.Models.DeleteItemInventory> deleteInventoryList)
        {
            API.Models.ActionResponse<List<string>> deleteQueryResult = new Models.ActionResponse<List<string>>();

            List<string> deleteResultMessage = new List<string>();
            try
            {
                foreach (var item in deleteInventoryList)
                {
                    DB.TWSELLERPORTALDB.Models.Seller_ProductDetail findDeleteItem
                        = this.sellerPortaldb.Seller_ProductDetail.Where(x => x.ProductID == item.ProductID
                                                          && x.SellerProductID == item.SellerProductID
                                                          && x.SellerID == item.SellerID
                                                       // && x.ShipType == item.ShipType
                                                       // && x.Condition ==item.Condition
                                                       // && x.ManufactureID == item.ManufactureID
                                                       ).FirstOrDefault();

                    if (findDeleteItem == null)
                    {
                        string returnstring = string.Format("Delete Faild ! Can't Find ProductID: {0}", item.ProductID);

                        // DeleteResultMessage[DeleteIDCount] = returnstring;
                        deleteResultMessage.Add(returnstring);
                    }
                    else
                    {
                        this.sellerPortaldb.Seller_ProductDetail.Remove(findDeleteItem);
                        this.sellerPortaldb.SaveChanges();
                    }
                }

                if (deleteResultMessage.Count() > 0)
                {
                    deleteQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                    deleteQueryResult.IsSuccess = true;
                    deleteQueryResult.Msg = "Delete Faild";
                    deleteQueryResult.Body = deleteResultMessage;
                }
                else
                {
                    deleteQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                    deleteQueryResult.IsSuccess = true;
                    deleteQueryResult.Msg = "Delete Success";
                    deleteQueryResult.Body = null;
                }
            }
            catch (Exception ex)
            {
                deleteQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                deleteQueryResult.IsSuccess = false;
                deleteQueryResult.Msg = ex.ToString();
                deleteQueryResult.Body = null;
            }

            return deleteQueryResult;
        }

        #endregion

        #region SaveALL

        /// <summary>
        /// SaveALLUpdate 存取所有變更
        /// </summary>
        /// <param name="inputSaveInventoryModel">Access new IteminventoryInfo or Edit ItemInventoryInfo</param>
        /// <returns>Access result</returns>
        public API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>> SaveAllUpdateInventoryAlert(List<API.Models.VM_ItemInventoryAlertInfo> inputSaveInventoryModel)
        {
            AutoMapper.Mapper.CreateMap<API.Models.VM_ItemInventoryAlertInfo, DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>();

            List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail> saveInventoryModel = new List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>();
            saveInventoryModel = AutoMapper.Mapper.Map<List<API.Models.VM_ItemInventoryAlertInfo>, List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>>(inputSaveInventoryModel);
            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>> getSaveAllQueryResult = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>>();
            List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail> errorList = new List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>();

            string returnMsgList = string.Empty;
            string accessErrorString = string.Empty;

            if (saveInventoryModel == null)
            {
                getSaveAllQueryResult.IsSuccess = true;
                getSaveAllQueryResult.Msg = "No Save Data";
                getSaveAllQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                getSaveAllQueryResult.Body = null;

                return getSaveAllQueryResult;
            }

            foreach (var item in saveInventoryModel)
            {
                object[] errorCheckobj = null;
                errorCheckobj = this.CheckSaveALLEmptyOrError(item);
                bool isError = (bool)errorCheckobj[0];
                
                if (!isError)
                {
                    errorList.Add(item);

                    if (saveInventoryModel.Count() == 1)
                    {
                        returnMsgList += "ProductID: " + item.ProductID + (ErrorCheckCode)errorCheckobj[1] + " have error.";
                    }
                    else
                    {
                        if (returnMsgList == string.Empty)
                        {
                            returnMsgList += "ProductID: " + item.ProductID + (ErrorCheckCode)errorCheckobj[1] + " have error.";
                        }
                        else
                        {
                            returnMsgList += "," + "ProductID: " + item.ProductID + (ErrorCheckCode)errorCheckobj[1] + " have error.";
                        }
                    }
                }
            }

            // 判斷是否有 Error 若無就 Create 或 Edit
            if (errorList.Count() == 0)
            {
                foreach (var itemInventoryInfo in saveInventoryModel)
                {
                    // 檢查是否存在於DB內，存在代表 "修改"，不存在代表  "新增"
                    bool isExist = this.sellerPortaldb.Seller_ProductDetail.Where(x => x.ProductID == itemInventoryInfo.ProductID
                                                                   && x.SellerProductID == itemInventoryInfo.SellerProductID
                                                                   && x.ProductID == itemInventoryInfo.ProductID
                                                                // && x.ManufactureID == itemInventoryInfo.ManufactureID
                                                                // && x.Condition == itemInventoryInfo.Condition
                                                                // && x.ShipType == itemInventoryInfo.ShipType
                                                                ).Any();

                    try
                    {
                        // isExist 為True 代表"修改"，反之為"新增"
                        if (isExist)
                        {
                            this.EditItemInventory(itemInventoryInfo);
                        }
                        else
                        {
                            this.AddItemInventoryInfo(itemInventoryInfo);
                        }

                        getSaveAllQueryResult.IsSuccess = true;
                        getSaveAllQueryResult.Msg = saveInventoryModel.Count() > 1
                                            ? "Save ItemInventoryInfo List Success" : "Save ItemInventoryInfo Success";
                        getSaveAllQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                        getSaveAllQueryResult.Body = null;
                    }
                    catch (Exception ex)
                    {
                        log.Error("SellerProductID: " + itemInventoryInfo.SellerProductID + "\r\n" +
                                      "Qty: " + itemInventoryInfo.Qty + "\r\n" +
                                      "QtyReg: " + itemInventoryInfo.QtyReg + "\r\n" +
                                      "SafeQty: " + itemInventoryInfo.SafeQty + "\r\n");

                        if (ex.ToString().Contains("CK_Seller_ProductDetail_qty_qtyreg"))
                        {
                            accessErrorString = "Can't Save. Qty must > Qtyreg";

                            log.Error("Can't Save. Qty must > Qtyreg");
                        }

                        log.Error("Exception Msg: " + ex.Message);

                        string errorString = ex.ToString();
                        getSaveAllQueryResult.IsSuccess = false;
                        getSaveAllQueryResult.Msg = errorString/*string.IsNullOrEmpty(AccessErrorString) && !string.IsNullOrEmpty(exString) ? "Data access error. Please contact the Newegg support" : AccessErrorString*/;
                        getSaveAllQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.AccessError;
                        getSaveAllQueryResult.Body = null;

                        
                    }
                }
            }
            else
            {
                getSaveAllQueryResult.IsSuccess = true;
                getSaveAllQueryResult.Msg = returnMsgList;
                // 因為欲存取的資料有 Error
                getSaveAllQueryResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                getSaveAllQueryResult.Body = errorList;
            }

            return getSaveAllQueryResult;
        }
       
        // 存進去的欄位檢查，只有檢查主 Key、SellerID、UpdateUserID 及 SafeQty
        private object[] CheckSaveALLEmptyOrError(DB.TWSELLERPORTALDB.Models.Seller_ProductDetail saveModel)
        {
            object[] returnCheckResult = new object[2];
            bool isError = true;
            ErrorCheckCode errorCode = new ErrorCheckCode();
            if (saveModel.SafeQty == 0 || (object)saveModel.SafeQty == null)
            {
                isError = false;
                errorCode = ErrorCheckCode.SafeQty;
            }

            if (saveModel.InUserID == 0 && (object)saveModel.InUserID == null)
            {
                isError = false;
                errorCode = ErrorCheckCode.InUserID;
            }

            //if (saveModel.ShipType == string.Empty)
            //{
            //    isError = false;
            //    errorCode = ErrorCheckCode.ShipType;
            //}

            if (saveModel.SellerID == 0 || (object)saveModel.SellerID == null)
            {
                isError = false;
                errorCode = ErrorCheckCode.SellerID;
            }

            if (saveModel.ProductID == 0 || (object)saveModel.ProductID == null)
            {
                isError = false;
                errorCode = ErrorCheckCode.ProductID;
            }

            //if (saveModel.ManufactureID == 0 || (object)saveModel.ManufactureID == null)
            //{
            //    isError = false;
            //    errorCode = ErrorCheckCode.ManufactureID;
            //}

            //if (saveModel.Condition == 0 || (object)saveModel.Condition == null)
            //{
            //    isError = false;
            //    errorCode = ErrorCheckCode.Condition;
            //}

            if (saveModel.UpdateUserID == 0 || (object)saveModel.UpdateUserID == null)
            {
                isError = false;
                errorCode = ErrorCheckCode.UpdateUserID;
            }

            returnCheckResult[0] = isError;
            returnCheckResult[1] = errorCode;

            return returnCheckResult;
        }

        private void EditItemInventory(DB.TWSELLERPORTALDB.Models.Seller_ProductDetail editItemInventory)
        {

            DB.TWSELLERPORTALDB.Models.Seller_ProductDetail dbData_ItemInventoryEdit =
                this.sellerPortaldb.Seller_ProductDetail.Where(x => x.ProductID == editItemInventory.ProductID
                                                              && x.SellerProductID == editItemInventory.SellerProductID
                                                              && x.ProductID == editItemInventory.ProductID
                // && x.ManufactureID == editItemInventory.ManufactureID
                // && x.Condition == editItemInventory.Condition
                // && x.ShipType == editItemInventory.ShipType
                                                           ).FirstOrDefault();

            dbData_ItemInventoryEdit.SafeQty = editItemInventory.SafeQty;
            dbData_ItemInventoryEdit.UpdateDate = editItemInventory.UpdateDate;
            dbData_ItemInventoryEdit.UpdateUserID = editItemInventory.UpdateUserID;

            this.sellerPortaldb.SaveChanges();
        }

        private void AddItemInventoryInfo(DB.TWSELLERPORTALDB.Models.Seller_ProductDetail addItemInventoryInfo)
        {

            this.sellerPortaldb.Seller_ProductDetail.Add(addItemInventoryInfo);
            this.sellerPortaldb.SaveChanges();
        }

        #endregion

        #region 庫存異常通知信

        /// <summary>
        /// 寄出 InventoryAlert Mail 
        /// </summary>
        /// <param name="mailInfo">Input MailType、UserMail、UserName、ProductName</param>
        /// <returns>Sending Mail result</returns>
        public API.Models.ActionResponse<Models.MailResult> SendInventoryAlertEmail(Models.ItemInventoryMailInfo mailInfo)
        {
            Models.Connector connector = new Models.Connector();

            Models.Mail inventoryAlertMail = new Models.Mail();
            inventoryAlertMail.MailType = Models.Mail.MailTypeEnum.InventoryAlertEmail;
            inventoryAlertMail.UserEmail = mailInfo.UserEmail;
            inventoryAlertMail.UserName = mailInfo.UserName;
            inventoryAlertMail.MailMessage = mailInfo.ProductName;
           
            return connector.SendMail(null, null, inventoryAlertMail);
        }
        #endregion
    }
}
