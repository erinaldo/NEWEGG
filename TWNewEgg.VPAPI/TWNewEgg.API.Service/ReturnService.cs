using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;
using TWNewEgg.DB;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models.ExtModels;

namespace TWNewEgg.API.Service
{
    public class ReturnService
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        #region 主單

        /// <summary>
        /// 取得退貨清單
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>退貨清單</returns>
        public ActionResponse<List<MainRetgood>> GetMainRegood(MainRetgoodSearchCondition searchCondition)
        {
            ActionResponse<List<MainRetgood>> result = new ActionResponse<List<MainRetgood>>();
            result.Body = new List<MainRetgood>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            ActionResponse<IQueryable<MainRetgood>> setQueryable = SetQueryable(searchCondition);

            if (setQueryable.IsSuccess)
            {
                #region 讀取退貨單

                try
                {
                    result.Body = setQueryable.Body.ToList();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("讀取退貨單失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                }

                #endregion 讀取退貨單

                #region 填寫顯示文字

                if (result.IsSuccess && result.Body.Count > 0)
                {
                    #region 查詢 SellerName

                    // 收集要查詢 SellerName 的 SellerID
                    List<int> sellerIDCell = result.Body.Where(x => x.SellerID.HasValue).Select(x => x.SellerID.Value).Distinct().ToList();

                    // 查詢  SellerName 結果
                    ActionResponse<List<Seller_ID_Name>> getSellerNameBySellerID = new ActionResponse<List<Seller_ID_Name>>();

                    if (sellerIDCell.Count > 0)
                    {
                        SellerBasicInfoService sellerBasicInfoService = new SellerBasicInfoService();
                        getSellerNameBySellerID = sellerBasicInfoService.GetSellerNameBySellerID(sellerIDCell);

                        #region 若待查詢筆數與查詢結果筆數不同，記錄漏掉的商家

                        if ((getSellerNameBySellerID.IsSuccess && getSellerNameBySellerID.Body.Count > 0) && (sellerIDCell.Count != getSellerNameBySellerID.Body.Count))
                        {
                            string missingSellerID = string.Empty;

                            foreach (int sellerID in sellerIDCell)
                            {
                                if (getSellerNameBySellerID.Body.Any(x => x.ID == sellerID) == false)
                                {
                                    if (!string.IsNullOrEmpty(missingSellerID))
                                    {
                                        missingSellerID += "、";
                                    }
                                    
                                    missingSellerID += sellerID.ToString();
                                }
                            }

                            if (!string.IsNullOrEmpty(missingSellerID))
                            {
                                logger.Info(string.Format("商家編號 {0}，查無商家資訊.", missingSellerID));
                            }
                        }

                        #endregion 若待查詢筆數與查詢結果筆數不同，記錄漏掉的商家
                    }

                    #endregion 查詢 SellerName

                    try
                    {
                        foreach (MainRetgood mainRetgood in result.Body)
                        {
                            #region 填寫退貨狀態名稱

                            if (mainRetgood.Status.HasValue)
                            {
                                switch (mainRetgood.Status.Value)
                                {
                                    case (int)Retgood.status.退貨處理中:
                                        {
                                            mainRetgood.StatusName = Retgood.status.退貨處理中.ToString();
                                            break;
                                        }
                                    case (int)Retgood.status.退貨中:
                                        {
                                            mainRetgood.StatusName = Retgood.status.退貨中.ToString();
                                            break;
                                        }
                                    case (int)Retgood.status.完成退貨:
                                        {
                                            mainRetgood.StatusName = Retgood.status.完成退貨.ToString();
                                            break;
                                        }
                                    case (int)Retgood.status.退貨異常:
                                        {
                                            mainRetgood.StatusName = Retgood.status.退貨異常.ToString();
                                            break;
                                        }
                                    case (int)Retgood.status.退貨取消:
                                        {
                                            mainRetgood.StatusName = Retgood.status.退貨取消.ToString();
                                            break;
                                        }
                                    default:
                                        {
                                            if (searchCondition.IsAdmin)
                                            {
                                                mainRetgood.StatusName = ((Retgood.status)mainRetgood.Status.Value).ToString();
                                            }
                                            else
                                            {
                                                mainRetgood.StatusName = null;
                                            }
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                mainRetgood.StatusName = null;
                            }

                            #endregion 填寫退貨狀態名稱

                            #region 填寫商家名稱及商家編號

                            if (mainRetgood.SellerID.HasValue && (getSellerNameBySellerID.IsSuccess && getSellerNameBySellerID.Body.Count > 0))
                            {
                                string sellerName = getSellerNameBySellerID.Body.Where(x => x.ID == mainRetgood.SellerID.Value).Select(x => x.Name).FirstOrDefault();
                                mainRetgood.SellerName = sellerName;
                                mainRetgood.Seller = string.Format("{0} ({1})", sellerName, mainRetgood.SellerID.Value);
                            }
                            else
                            {
                                mainRetgood.Seller = null;
                            }

                            #endregion 填寫商家名稱及商家編號

                            #region 填寫付款方式名稱

                            if (mainRetgood.PayType.HasValue)
                            {
                                mainRetgood.PayTypeName = ((TWNewEgg.DB.TWBACKENDDB.Models.Cart.paytypestatus)mainRetgood.PayType.Value).ToString();
                            }
                            else
                            {
                                mainRetgood.PayTypeName = null;
                            }

                            #endregion 填寫付款方式名稱

                            #region 填寫交易模式名稱

                            if (mainRetgood.ShipType.HasValue)
                            {
                                mainRetgood.ShipTypeName = ((TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus)mainRetgood.ShipType.Value).ToString();
                            }
                            else
                            {
                                mainRetgood.ShipTypeName = null;
                            }

                            #endregion 填寫交易模式名稱
                        }
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        logger.Info(string.Format("填寫顯示文字失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                    }
                }

                #endregion 填寫顯示文字

                #region 排序

                // 2015/11/02 需求，由訂單日期做降冪排序
                result.Body = result.Body.OrderByDescending(x => x.RetgoodCreateDate).ToList();

                #endregion 排序
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 設定搜尋條件
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        private ActionResponse<IQueryable<MainRetgood>> SetQueryable(MainRetgoodSearchCondition searchCondition)
        {
            ActionResponse<IQueryable<MainRetgood>> result = new ActionResponse<IQueryable<MainRetgood>>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            ActionResponse<IQueryable<MainRetgood>> setQueryable_JoinTables = SetQueryable_JoinTables(searchCondition);

            if (setQueryable_JoinTables.IsSuccess)
            {
                result.Body = setQueryable_JoinTables.Body;

                try
                {
                    #region 依 SellerID 篩選

                    // 有管理權限且 SellerID 為 null，才不進行 SellerID 篩選
                    if (!(searchCondition.IsAdmin && searchCondition.SellerID == null))
                    {
                        result.Body = result.Body.Where(x => x.SellerID == searchCondition.SellerID).AsQueryable();
                    }

                    #endregion 依 SellerID 篩選

                    #region 依關鍵字篩選

                    if (!string.IsNullOrEmpty(searchCondition.KeyWord))
                    {
                        switch (searchCondition.KeyWordSearchType)
                        {
                            default:
                            case (int)RetgoodKeyWordSearchType.訂單編號:
                                {
                                    result.Body = result.Body.Where(x => x.CartID == searchCondition.KeyWord).AsQueryable();
                                    break;
                                }
                            case (int)RetgoodKeyWordSearchType.商品名稱:
                                {
                                    result.Body = result.Body.Where(x => x.ProcessTitle.IndexOf(searchCondition.KeyWord) != -1).AsQueryable();
                                    break;
                                }
                            case (int)RetgoodKeyWordSearchType.收件人姓名:
                                {
                                    result.Body = result.Body.Where(x => x.FrmName == searchCondition.KeyWord).AsQueryable();
                                    break;
                                }
                            case (int)RetgoodKeyWordSearchType.商家銷售編號:
                                {
                                    result.Body = result.Body.Where(x => x.SellerProductID == searchCondition.KeyWord).AsQueryable();
                                    break;
                                }
                        }
                    }

                    #endregion 依關鍵字篩選

                    #region 依退貨狀態篩選

                    // 讀取只會用在 Vendor Portal 的退貨狀態
                    List<int> vendorPortalRetgoodStatusCell = new List<int>();
                    vendorPortalRetgoodStatusCell.Add((int)Retgood.status.退貨處理中);
                    vendorPortalRetgoodStatusCell.Add((int)Retgood.status.退貨中);
                    vendorPortalRetgoodStatusCell.Add((int)Retgood.status.完成退貨);
                    vendorPortalRetgoodStatusCell.Add((int)Retgood.status.退貨異常);
                    vendorPortalRetgoodStatusCell.Add((int)Retgood.status.退貨取消);

                    // 篩選掉非 Vendor Portal 退貨狀態的資料
                    result.Body = result.Body.Where(x => vendorPortalRetgoodStatusCell.Contains(x.Status.Value)).AsQueryable();

                    // 依搜尋條件的退貨狀態篩選
                    if (searchCondition.RetgoodStatus.HasValue)
                    {
                        result.Body = result.Body.Where(x => x.Status == searchCondition.RetgoodStatus).AsQueryable();
                    }

                    #endregion 依退貨狀態篩選

                    #region 依訂單日期篩選

                    DateTime? startDate = null;
                    DateTime? endDate = null;

                    switch (searchCondition.CreateDateSearchType)
                    {
                        default:
                        case (int)RetgoodCreateDateSearchType.全部:
                            {
                                break;
                            }
                        case (int)RetgoodCreateDateSearchType.今天:
                            {
                                startDate = DateTime.Today;
                                endDate = DateTime.Today.AddDays(1);
                                break;
                            }
                        case (int)RetgoodCreateDateSearchType.最近3天:
                            {
                                startDate = DateTime.Today.AddDays(-2);
                                endDate = DateTime.Today.AddDays(1);
                                break;
                            }
                        case (int)RetgoodCreateDateSearchType.最近7天:
                            {
                                startDate = DateTime.Today.AddDays(-6);
                                endDate = DateTime.Today.AddDays(1);
                                break;
                            }
                        case (int)RetgoodCreateDateSearchType.最近30天:
                            {
                                startDate = DateTime.Today.AddDays(-29);
                                endDate = DateTime.Today.AddDays(1);
                                break;
                            }
                        case (int)RetgoodCreateDateSearchType.指定日期:
                            {
                                startDate = searchCondition.StartDate.Value.ToLocalTime().Date;
                                endDate = searchCondition.StartDate.Value.ToLocalTime().Date.AddDays(1);
                                break;
                            }
                        case (int)RetgoodCreateDateSearchType.定制日期範圍:
                            {
                                startDate = searchCondition.StartDate.Value.ToLocalTime().Date;
                                endDate = searchCondition.EndDate.Value.ToLocalTime().Date.AddDays(1);
                                break;
                            }
                    }

                    if (searchCondition.CreateDateSearchType >= 1 && searchCondition.CreateDateSearchType <= 6)
                    {
                        result.Body = result.Body.Where(x => x.CartCreateDate >= startDate && x.CartCreateDate < endDate).AsQueryable();
                    }

                    #endregion 依訂單日期篩選
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("設定搜尋條件失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                }
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }


        /// <summary>
        /// JoinTables
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>JoinTables</returns>
        private ActionResponse<IQueryable<MainRetgood>> SetQueryable_JoinTables(MainRetgoodSearchCondition searchCondition)
        {
            ActionResponse<IQueryable<MainRetgood>> result = new ActionResponse<IQueryable<MainRetgood>>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            TWBackendDBContext dbBack = new TWBackendDBContext();

            try
            {
                result.Body = (from regood in dbBack.Retgood
                               join cart in dbBack.Cart on regood.CartID equals cart.ID
                               // join 子單的時候，只讀取子單的第 1 筆資料
                               join process in dbBack.Process on regood.CartID equals process.CartID into process_Temp
                               let process = process_Temp.FirstOrDefault()
                               select new MainRetgood
                               {
                                   CartCreateDate = cart.CreateDate,
                                   CartID = regood.CartID,
                                   RetgoodCreateDate = regood.Date,
                                   FinReturnDate = regood.FinReturnDate,
                                   FrmName = regood.FrmName,
                                   PayType = cart.PayType,
                                   ProcessTitle = process.Title,
                                   Status = regood.Status,
                                   SellerID = regood.SupplierID,
                                   ShipType = cart.ShipType,
                                   ProductID = regood.ProductID,
                               }).AsQueryable();

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                logger.Info(string.Format("JoinTables 失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 填寫 Response Code
        /// </summary>
        /// <param name="isSuccess">成功、失敗資訊</param>
        /// <returns>Response Code</returns>
        private int SetResponseCode(bool isSuccess)
        {
            if (isSuccess)
            {
                return (int)ResponseCode.Success;
            }
            else
            {
                return (int)ResponseCode.Error;
            }
        }

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
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

        #endregion 主單

        #region 派車功能

        /// <summary>
        /// 取得退貨商品資訊
        /// </summary>
        /// <param name="cart_id">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodAPIModel> retgoodInfomation(string cart_id)
        {
            TWNewEgg.DB.TWBackendDBContext DB_Back = new DB.TWBackendDBContext();
            TWNewEgg.DB.TWSqlDBContext DB_Front = new DB.TWSqlDBContext();
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodUpper> retgoodUpper = new Models.ActionResponse<Models.RetgoodUpper>();
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodAPIModel> result = new Models.ActionResponse<Models.RetgoodAPIModel>();
            #region 判斷是否有傳入 card_id
            // 判斷是否有傳入 card_id
            if (string.IsNullOrEmpty(cart_id) == true)
            {
                result.IsSuccess = false;
                result.Msg = "沒有退貨編號";
                result.Body = null;
                return result;
            }
            #endregion

            #region 判斷是否由 Vendor 出貨

            ActionResponse<int?> isShipByVendor = this.IsShipByVendor(cart_id);
            if (!isShipByVendor.IsSuccess)
            {
                result.Finish(false, (int)ResponseCode.Error, isShipByVendor.Msg, null);
                logger.Error(string.Format("退貨備註更新失敗 : 交易模式錯誤，Shiptype = {0}。", isShipByVendor.Body.HasValue ? ((TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus)isShipByVendor.Body).ToString() + "(" + isShipByVendor.Body + ")" : "null"));
                return result;
            }

            #endregion 判斷是否由 Vendor 出貨

            #region 開始讀取需要顯示在畫面上的上半部資料(退換貨概要資訊)
            // 開始讀取需要顯示在畫面上的上半部資料
            retgoodUpper = this.retgoodupperData(cart_id);
            // 讀取失敗
            if (retgoodUpper.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = retgoodUpper.Msg;
                result.Body = null;
                return result;
            }

            result.Body = new Models.RetgoodAPIModel();
            result.Body.retgoodUpper = retgoodUpper.Body;
            #endregion
            #region 轉換 retgood 的狀態
            // 轉換 retgood 的狀態
            TWNewEgg.API.Models.ActionResponse<string> changeEnumToStr = this.StatusEnumChangeToStr(result.Body.retgoodUpper.Retgood_Status);
            // 轉換失敗
            if (changeEnumToStr.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = changeEnumToStr.Msg;
            }
            // 轉換成功把對應的 status 說明放回要回傳回畫面上顯示的 model 欄位
            result.Body.retgoodUpper.Retgood_Status_str = changeEnumToStr.Body;
            #endregion
            #region 讀取退換貨商品資訊
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.RetgoodGrid>> getRetgoodGridData = this.retgoodgirdDate(cart_id);
            //判斷是否讀取退換貨商品資訊正確還是有錯誤
            if (getRetgoodGridData.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = getRetgoodGridData.Msg;
                result.Body = null;
                return result;
            }

            result.Body.retgoodgrid = getRetgoodGridData.Body;
            #endregion
            result.IsSuccess = true;
            result.Msg = "Success";
            return result;
        }

        /// <summary>
        /// 開始讀取需要顯示在畫面上的上半部資料
        /// </summary>
        /// <param name="cart_id">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        private TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodUpper> retgoodupperData(string cart_id = "")
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodUpper> result = new Models.ActionResponse<Models.RetgoodUpper>();
            TWNewEgg.DB.TWBackendDBContext DB_Back = new DB.TWBackendDBContext();
            #region 判斷是否有傳入訂單編號
            //判斷是否有傳入訂單編號
            if (string.IsNullOrEmpty(cart_id))
            {
                result.IsSuccess = false;
                result.Msg = "沒有退貨單資料";
                result.Body = null;
                return result;
            }
            #endregion
            IQueryable<Models.RetgoodUpper> RetgoodUpperDataAsQueryable = null;
            try
            {
                #region 利用 card_id 編號去讀取出對應的 cart 和 retgood 相關資料
                //利用 card_id 編號去讀取出對應的 cart 和 retgood 相關資料
                RetgoodUpperDataAsQueryable = (from cart in DB_Back.Cart
                                               join retgood in DB_Back.Retgood on cart.ID equals retgood.CartID
                                               where cart.ID == cart_id
                                               select new Models.RetgoodUpper
                                               {
                                                   //訂單編號
                                                   Cart_ID = cart.ID,
                                                   //訂單日期
                                                   Cart_CreateDate = cart.CreateDate.Value,
                                                   //訂購人姓名
                                                   Cart_Username = cart.Username,
                                                   //訂購人手機
                                                   Cart_Mobile = cart.Mobile,
                                                   //取件郵遞區號
                                                   Retgood_FrmZipcode = retgood.FrmZipcode,
                                                   //取件人縣市
                                                   Retgood_FrmLocation = retgood.FrmLocation,
                                                   //取件人地址
                                                   Retgood_FrmADDR = retgood.FrmADDR,
                                                   //取件人姓名
                                                   Retgood_FrmName = retgood.FrmName,
                                                   //取件人手機
                                                   Retgood_FrmMobile = retgood.FrmMobile,
                                                   //取件人市話
                                                   Retgood_FrmPhone = retgood.FrmPhone,
                                                   //退/換貨狀態
                                                   Retgood_Status = retgood.Status.Value
                                               }).AsQueryable();
                //判斷是否有資料
                if (RetgoodUpperDataAsQueryable.Any() == false)
                {
                    result.IsSuccess = false;
                    result.Body = null;
                    result.Msg = "沒有相關的資料";
                    logger.Error("cart_id: " + cart_id + ", Cart join retgood 沒有資料");
                }
                else
                {
                    result.IsSuccess = true;
                    result.Body = RetgoodUpperDataAsQueryable.FirstOrDefault();
                    result.Msg = "成功";
                }
                #endregion
            }
            catch (Exception error)
            {
                logger.Error(this.errorMsg(error));
                result.IsSuccess = false;
                result.Body = null;
                result.Msg = "資料處理錯誤";
            }
            return result;
        }

        /// <summary>
        /// 列舉之間的轉換
        /// </summary>
        /// <param name="int_status"></param>
        /// <returns></returns>
        private TWNewEgg.API.Models.ActionResponse<string> StatusEnumChangeToStr(int int_status)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            result = new Models.ActionResponse<string>();
            try
            {
                //因為列舉的型態是繼承 short 所以必須先把狀態 int 轉換成 short 才可做列舉資料之間的轉換
                Int16 int16_int_status = Convert.ToInt16(int_status);
                //判斷列舉是否存在這一個資料
                if (Enum.IsDefined(typeof(TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status), int16_int_status) == true)
                {
                    //轉換 Status 為中文說明
                    result.Body = ((TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status)int16_int_status).ToString();
                    result.IsSuccess = true;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = "資料不存在";
                    logger.Error("列舉資料不存在. Status = " + int16_int_status + ", 列舉的資料為: "
                        + Newtonsoft.Json.JsonConvert.SerializeObject(Enum.GetValues(typeof(TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status))));
                }
            }
            catch (Exception error)
            {
                logger.Error(this.errorMsg(error));
                result.IsSuccess = false;
                result.Body = string.Empty;
                result.Msg = "資料處理錯誤";
            }
            return result;
        }

        /// <summary>
        /// 讀取退換貨商品資訊
        /// </summary>
        /// <param name="cart_id">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        private TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.RetgoodGrid>> retgoodgirdDate(string cart_id = "")
        {
            TWNewEgg.DB.TWBackendDBContext DB_Back = new DB.TWBackendDBContext();
            TWNewEgg.DB.TWSqlDBContext DB_Front = new DB.TWSqlDBContext();
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.RetgoodGrid>> result = new Models.ActionResponse<List<Models.RetgoodGrid>>();
            List<TWNewEgg.API.Models.RetgoodGrid> retgoodGridList = new List<Models.RetgoodGrid>();
            #region 判斷是否有傳入訂單編號
            if (string.IsNullOrEmpty(cart_id))
            {
                result.IsSuccess = false;
                result.Msg = "沒有訂單編號";
                result.Body = null;
                return result;
            }
            #endregion
            try
            {
                // 利用 card_id 讀取 Process、Retgood 對應的資料
                List<TWNewEgg.DB.TWBACKENDDB.Models.Process> list_Process = DB_Back.Process.Where(p => p.CartID == cart_id).ToList();
                TWNewEgg.DB.TWBACKENDDB.Models.Retgood _retgood = DB_Back.Retgood.Where(p => p.CartID == cart_id).FirstOrDefault();
                // 判斷是否 Process、Retgood 有這一筆資料
                if (list_Process == null || _retgood == null || list_Process.Count == 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "沒有任何處理的資料";
                    return result;
                }

                // 計算訂購數量
                int ProcessQty = list_Process.Sum(p => p.Qty.Value);
                // 取得對應的 product_id
                int productid = list_Process.FirstOrDefault().ProductID.Value;
                // 利用 productid 讀取 product 相關的資料
                TWNewEgg.DB.TWSQLDB.Models.Product _product = DB_Front.Product.Where(p => p.ID == productid).FirstOrDefault();
                // 判斷 Product 有相關資料
                if (_product == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "沒有任何產品的資料";
                    return result;
                }

                #region 開始寫入相關的資料到要回傳的 model
                TWNewEgg.API.Models.RetgoodGrid retgoodSingle = new Models.RetgoodGrid();
                // 訂單編號
                retgoodSingle.card_id = cart_id;
                // 商家銷售編號
                retgoodSingle.product_sellerproductid = _product.SellerProductID;
                // 新蛋商家編號
                retgoodSingle.product_productid = _product.ID;
                // 廠商產品編號
                retgoodSingle.product_MenufacturePartNum = _product.MenufacturePartNum;
                // UPC
                retgoodSingle.product_UPC = _product.UPC;
                // 商品說明
                retgoodSingle.process_Title = list_Process.FirstOrDefault().Title;
                // 訂購數量
                retgoodSingle.process_Qty = ProcessQty;
                // 退/換貨數量
                retgoodSingle.retgood_Qty = _retgood.Qty ?? 0;
                // 單價($)
                retgoodSingle.process_unitPrice = list_Process.FirstOrDefault().Price.Value;
                // 退/換貨原因
                retgoodSingle.retgood_CauseNote = _retgood.CauseNote;
                // 退款總金額
                retgoodSingle.retgood_Price = _retgood.Price ?? 0m;
                // 貨運編號
                retgoodSingle.retgood_ShpCode = _retgood.ShpCode;
                // 優惠總額
                retgoodSingle.process_coupon_total = list_Process.Sum(x => x.Pricecoupon) ?? 0m + list_Process.Sum(x => x.ApportionedAmount);
                #endregion
                retgoodGridList.Add(retgoodSingle);
                result.Body = retgoodGridList;
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.Body = null;
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error(this.errorMsg(error));
            }

            return result;
        }

        /// <summary>
        /// 更新退貨商品相關資訊
        /// </summary>
        /// <param name="updateRetGoodsInfo">所要更新資訊</param>
        /// <returns>返回更新結果</returns>
        public TWNewEgg.API.Models.ActionResponse<bool> UpdateRetGoods(UpdateRetGoodsInfo updateRetGoodsInfo)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Body = true;
            try
            {
                if (updateRetGoodsInfo == null)
                {
                    throw new Exception("無更新資料");
                }
                // 檢驗退貨備註資訊欄位是否尚可新增備註
                ActionResponse<bool> updateNoteCheck = UpdateNoteCheck(updateRetGoodsInfo.Cart_ID, updateRetGoodsInfo.OtherUpDataNote);
                if (!updateNoteCheck.Body)
                {
                    result = updateNoteCheck;
                    logger.Error("退貨狀態修改失敗 : " + updateNoteCheck.Msg);
                    return result;
                }
                // 更新資訊組合
                this.RetGoodsDataCheck(updateRetGoodsInfo);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = false;
                result.Msg = ex.Message;
                logger.Error("退貨狀態修改失敗 : " + ex.ToString());
                return result;
            }

            Service.TWService twser = new Service.TWService();
            try
            {
                // 更新退貨商品相關資訊
                result = twser.UpdateRetGoods(updateRetGoodsInfo);
                if (!result.IsSuccess)
                {
                    result.Body = false;
                    logger.Error("退貨狀態修改失敗 : " + result.Msg);
                    result.Msg = "系統退貨狀態更新失敗";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = false;
                result.Msg = "系統退貨狀態更新失敗";
                logger.Error("退貨狀態修改失敗 : " + ex.ToString());
                return result;
            }

            return result;
        }

        /// <summary>
        /// 檢驗退貨備註資訊欄位是否尚可新增備註
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <param name="updateNote">新增備註</param>
        /// <returns>返回檢驗結果</returns>
        private ActionResponse<bool> UpdateNoteCheck(string cartID, string updateNote)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Body = true;
            TWBackendDBContext db_after = new TWBackendDBContext();
            Retgood retgood = db_after.Retgood.Where(x => x.CartID == cartID).FirstOrDefault();
            if (retgood == null)
            {
                result.IsSuccess = false;
                result.Body = false;
                result.Msg = "查無此退貨資訊";
            }
            else
            {
                string getNewNote = retgood.UpdateNote + DateTime.UtcNow.AddHours(8).ToString("yyyy/MM/dd HH:mm:ss") + "　退貨處理中;　system;　" + updateNote + "<br>";
                int getNewNoteCount = getNewNote.Length;
                if (getNewNoteCount >= 4000)
                {
                    result.IsSuccess = false;
                    result.Body = false;
                    result.Msg = "退貨備註字數已超出限定字數，儲存失敗";
                    logger.Error("更新退貨資料與狀態失敗 : 退貨備註字數已超出限定字數，儲存失敗，字數長度 : " + getNewNoteCount);
                }
            }

            return result;
        }

        /// <summary>
        /// 更新資訊組合
        /// </summary>
        /// <param name="updateRetGoodsInfo">欲更新資訊</param>
        /// <returns>返回組合後更新資訊</returns>
        private void RetGoodsDataCheck(UpdateRetGoodsInfo updateRetGoodsInfo)
        {
            TWBackendDBContext db_after = new TWBackendDBContext();
            ActionResponse<bool> result = new ActionResponse<bool>();
            Retgood retgoodData = db_after.Retgood.Where(x => x.CartID == updateRetGoodsInfo.Cart_ID).FirstOrDefault();
            Cart cartData = db_after.Cart.Where(x => x.ID == updateRetGoodsInfo.Cart_ID).FirstOrDefault();
            if (retgoodData == null)
            {
                if (updateRetGoodsInfo.Cart_ID != null)
                {
                    throw new Exception("訂單編號[" + updateRetGoodsInfo.Cart_ID + "]在Table[Retgood]中，查無此筆資訊");
                }
                else
                {
                    throw new Exception("Table[Retgood]查無此筆資訊");
                }
            }
            // 退貨狀態可更新規則
            string errorMessage = UpdateStatusRule(retgoodData.Status ?? 0, updateRetGoodsInfo.Retgood_Status ?? 0);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// 退貨狀態可更新規則
        /// </summary>
        /// <param name="retgoodStatus">退貨商品目前狀態</param>
        /// <param name="updateStatus">所要更新狀態</param>
        /// <returns>返回檢查結果</returns>
        private string UpdateStatusRule(int retgoodStatus, int updateStatus)
        {
            string errorMessage = string.Empty;
            switch (retgoodStatus)
            {
                case (int)Retgood.status.退貨處理中:
                    if (updateStatus != (int)Retgood.status.退貨中)
                    {
                        errorMessage = "\"退貨處理中\"商品僅可更新為\"退貨中\"狀態";
                    }
                    break;
                case (int)Retgood.status.退貨中:
                    if (updateStatus != (int)Retgood.status.完成退貨 && updateStatus != (int)Retgood.status.退貨異常)
                    {
                        errorMessage = "\"退貨中\"商品僅可更新為\"完成退貨\"與\"退貨中\"狀態";
                    }
                    break;
                case (int)Retgood.status.完成退貨:
                    errorMessage = "\"完成退貨\"商品不得更新其他狀態";
                    break;
                case (int)Retgood.status.退貨異常:
                    errorMessage = "\"退貨異常\"商品不得更新其他狀態";
                    break;
                case (int)Retgood.status.退貨取消:
                    if (updateStatus != (int)Retgood.status.退貨處理中 && updateStatus != (int)Retgood.status.退貨中)
                    {
                        errorMessage = "\"退貨取消\"商品僅可更新為\"退貨處理中\"與\"退貨中\"狀態";
                    }
                    break;
                default:
                    errorMessage = "狀態輸入錯誤!";
                    break;
            }

            return errorMessage;
        }

        /// <summary>
        /// 廠商已查看派車明細備註
        /// </summary>
        /// <param name="userID">使用者ID</param>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回執行結果</returns>
        public TWNewEgg.API.Models.ActionResponse<bool> HasBeenViewed(int userID, string cartID)
        {
            TWBackendDBContext db_after = new TWBackendDBContext();
            API.Models.Connector conn = new Connector();
            string hasBeenViewed = conn.GetAPIWebConfigSetting("HasBeenViewed");
            TWNewEgg.API.Models.ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Body = true;
            try
            {
                if (userID <= 0 || string.IsNullOrEmpty(cartID))
                {
                    result.IsSuccess = false;
                    result.Body = false;
                    result.Msg = "更新資訊缺漏";
                    logger.Error("退貨備註更新失敗 : 更新資訊缺漏");
                    return result;
                }

                if (string.IsNullOrEmpty(hasBeenViewed))
                {
                    throw new Exception("廠商已點選派車按鈕查看明細資訊缺漏");
                }

                Retgood retgoodData = db_after.Retgood.Where(x => x.CartID == cartID).FirstOrDefault();
                if (retgoodData == null)
                {
                    throw new Exception("訂單編號[" + cartID + "]在Table[Retgood]中，查無此筆資訊");
                }

                if (retgoodData.UpdateNote.Contains(hasBeenViewed))
                {
                    return result;
                }

                string retgoodUpdateNote = retgoodData.UpdateNote + DateTime.UtcNow.AddHours(8).ToString("yyyy/MM/dd HH:mm:ss") + "　VendorPortal : UserID(" + userID.ToString() + "); " + hasBeenViewed + " <br>";
                if (retgoodUpdateNote.Length < 4000)
                {
                    retgoodData.UpdateNote = retgoodUpdateNote;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Body = false;
                    result.Msg = "退貨備註字數已超出限定字數，儲存失敗";
                    logger.Error("退貨備註更新失敗 : 退貨備註字數已超出限定字數，儲存失敗，字數長度 : " + retgoodUpdateNote.Length);
                    return result;
                }

                db_after.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = false;
                result.Msg = "系統退貨備註更新失敗";
                logger.Error("退貨備註更新失敗 : " + ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 組合要寫入 logger 的錯誤訊息
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private string errorMsg(Exception error)
        {
            string innerMsg = string.Empty;
            innerMsg = error.InnerException == null ? string.Empty : error.InnerException.Message;
            return "[Msg]: " + error.Message + " ;[StackTrace]:" + error.StackTrace + " ;[innerMsg]: " + innerMsg;
        }

        #endregion

        #region 回報功能

        /// <summary>
        /// 取得退貨商品狀態與更新訊息資訊
        /// </summary>
        /// <param name="cart_id">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodNote> retgoodNote(string cart_id = "")
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodNote> result = new Models.ActionResponse<Models.RetgoodNote>();
            if (string.IsNullOrEmpty(cart_id) == true)
            {
                result.IsSuccess = false;
                result.Msg = "無傳入訂單編號";
                result.Body = null;
                return result;
            }

            ActionResponse<int?> isShipByVendor = this.IsShipByVendor(cart_id);
            if (!isShipByVendor.IsSuccess)
            {
                result.Finish(false, (int)ResponseCode.Error, isShipByVendor.Msg, null);
                logger.Error(string.Format("退貨備註更新失敗 : 交易模式錯誤，Shiptype = {0}。", isShipByVendor.Body.HasValue ? ((TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus)isShipByVendor.Body).ToString() + "(" + isShipByVendor.Body + ")" : "null"));
                return result;
            }

            TWNewEgg.DB.TWBackendDBContext DBBack = new DB.TWBackendDBContext();
            TWNewEgg.DB.TWBACKENDDB.Models.Retgood _retgood = new DB.TWBACKENDDB.Models.Retgood();
            _retgood = DBBack.Retgood.Where(p => p.CartID == cart_id).FirstOrDefault();
            if (_retgood == null)
            {
                result.IsSuccess = false;
                result.Msg = "查無對應的資料";
                result.Body = null;
                return result;
            }

            result.Body = new Models.RetgoodNote();
            result.Body.Note_Des = _retgood.UpdateNote;
            result.Body.Note_Status = _retgood.Status.Value;
            result.Body.Note_UpdateRec = _retgood.UpdateNote;
            result.IsSuccess = true;
            result.Msg = "Success";
            return result;
        }

        #endregion

        #region 查看功能

        /// <summary>
        /// 新增備註
        /// </summary>
        /// <param name="userID">使用者ID</param>
        /// <param name="cartID">訂單編號</param>
        /// <param name="updateNote">新增備註</param>
        /// <returns>返回結果</returns>
        public TWNewEgg.API.Models.ActionResponse<bool> UpdateRetGoodsNote(int userID, string cartID, string updateNote)
        {
            TWBackendDBContext db_after = new TWBackendDBContext();
            TWNewEgg.API.Models.ActionResponse<bool> result = new ActionResponse<bool>();
            API.Models.Connector conn = new Connector();
            result.IsSuccess = true;
            result.Body = true;
            try
            {
                if (userID <= 0 || string.IsNullOrEmpty(cartID) || string.IsNullOrEmpty(updateNote))
                {
                    result.IsSuccess = false;
                    result.Body = false;
                    result.Msg = "更新資訊缺漏";
                    logger.Error("退貨備註更新失敗 : 更新資訊缺漏");
                    return result;
                }

                ActionResponse<int?> isShipByVendor = this.IsShipByVendor(cartID);
                if (!isShipByVendor.IsSuccess)
                {
                    result.Finish(false, (int)ResponseCode.Error, isShipByVendor.Msg, false); 
                    logger.Error(string.Format("退貨備註更新失敗 : 交易模式錯誤，Shiptype = {0}。", isShipByVendor.Body.HasValue ? ((TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus)isShipByVendor.Body).ToString() + "(" + isShipByVendor.Body + ")" : "null"));
                    return result;
                }

                Retgood retgoodData = db_after.Retgood.Where(x => x.CartID == cartID).FirstOrDefault();
                if (retgoodData == null)
                {
                    throw new Exception("訂單編號[" + cartID + "]在Table[Retgood]中，查無此筆資訊");
                }

                string retgoodUpdateNote = retgoodData.UpdateNote + DateTime.UtcNow.AddHours(8).ToString("yyyy/MM/dd HH:mm:ss") + "　其他;　VendorPortal : UserID(" + userID.ToString() + "); " + updateNote + "<br>";
                if (retgoodUpdateNote.Length < 4000)
                {
                    retgoodData.UpdateNote = retgoodUpdateNote;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Body = false;
                    result.Msg = "退貨備註字數已超出限定字數，儲存失敗";
                    logger.Error("退貨備註更新失敗 : 退貨備註字數已超出限定字數，儲存失敗，字數長度 : " + retgoodUpdateNote.Length);
                    return result;
                }

                db_after.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = false;
                result.Msg = "系統退貨備註更新失敗";
                logger.Error("退貨備註更新失敗 : " + ex.ToString());
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 判斷是否由 Vendor 出貨
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <returns>交易模式</returns>
        private ActionResponse<int?> IsShipByVendor(string cartID)
        {
            ActionResponse<int?> result= new ActionResponse<int?>();
            result.Body = null;
            result.IsSuccess = false;
            result.Msg = string.Empty;

            if (string.IsNullOrEmpty(cartID))
            {
                result.Finish(false, (int)ResponseCode.Error, "未傳入訂單編號", null);
                logger.Info(string.Format("判斷是否由 Vendor 出貨失敗; ErrorMessage = {0}.", "未傳入訂單編號"));
                return result;
            }

            #region 讀取 ShipType

            TWBackendDBContext dbBack = new TWBackendDBContext();

            try
            {
                result.Body = dbBack.Cart.Where(x => x.ID == cartID).Select(x => (int)x.ShipType).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.Msg = "資料讀取失敗";
                logger.Info(string.Format("讀取 ShipType 失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
            }

            #endregion 讀取 ShipType

            if (result.Body.HasValue)
            {
                switch (result.Body)
                {
                    case (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.直配:
                    case (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.B2C直配:
                        {
                            result.IsSuccess = true;
                            break;
                        }
                    default:
                        {
                            result.Msg = "此訂單為寄倉商品，將由台蛋網進行退貨事宜。";
                            break;
                        }
                }
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
    }
}
