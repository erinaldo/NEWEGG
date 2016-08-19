using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.ECWeb.Controllers;
using TWNewEgg.Redeem.Service.PromotionGiftService;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.Models.ViewModels.Redeem;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.ECWeb.Services.OldCart.CartService
{
    public class AfterPayService
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        public void CreatePOAndAuth(string arg_strOrderNumber, TWNewEgg.DB.TWSQLDB.Models.Auth arg_oAuth, string arg_strErrCode, string arg_strPlat)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            SalesOrder objMasterSalesOrder = null;

            List<SalesOrderItem> listSalesOrderItem = null;
            List<SalesOrder> listSalesOrder = null;
            SalesOrder objItemOfSalesOrder = null;
            int numSalesOrderGroupId = 0;
            int numAuthCount = 0;
            string strAccountId = "";
            string strPoCode = "";

            //PromotionGift用的Service
            //PromotionGiftRepository objPromotionGiftService = null;

            string strErrorRecord = "";
            bool payFailFlag = false;

            try
            {
                if (arg_strPlat.Equals("") || arg_strPlat.Equals("CHINATRUST"))
                {
                    arg_strPlat = "CHINATRUST";
                    objMasterSalesOrder = db_before.SalesOrder.Where(x => x.Code == arg_strOrderNumber).FirstOrDefault<SalesOrder>();
                    numSalesOrderGroupId = Convert.ToInt32(objMasterSalesOrder.SalesOrderGroupID);
                }
                else if (arg_strPlat.Equals("ALLPAY"))
                {
                    //因ALLPAY的OrderNumber預設是使用CartNo, 故要將真正的OrderNumber拿出來
                    numSalesOrderGroupId = Convert.ToInt32(arg_strOrderNumber);
                }
                else if (arg_strPlat.Equals("NCCC"))
                {
                    numSalesOrderGroupId = Convert.ToInt32(arg_strOrderNumber);
                }
                else
                {
                    //Hitrust
                    //objMasterSalesOrder = db_before.SalesOrder.Where(x => x.Code == arg_strOrderNumber).FirstOrDefault<SalesOrder>();
                    numSalesOrderGroupId = Convert.ToInt32(arg_strOrderNumber);
                } //end if
                objMasterSalesOrder = null;

                #region 產生Auth
                /* ------ starting of 產生Auth ------*/
                strErrorRecord = "------------create auth ------------\r\n";

                listSalesOrder = db_before.SalesOrder.Where(x1 => x1.SalesOrderGroupID == numSalesOrderGroupId).ToList<SalesOrder>();
                List<TWNewEgg.DB.TWSQLDB.Models.Auth> data = new List<TWNewEgg.DB.TWSQLDB.Models.Auth>();
                foreach (SalesOrder objSalesOrder in listSalesOrder)
                {
                    listSalesOrderItem = db_before.SalesOrderItem.Where(x => x.SalesorderCode == objSalesOrder.Code).ToList<SalesOrderItem>();
                    foreach (SalesOrderItem objSalesOrderItem in listSalesOrderItem)
                    {
                        TWNewEgg.DB.TWSQLDB.Models.Auth auth = new TWNewEgg.DB.TWSQLDB.Models.Auth();
                        auth.SuccessFlag = arg_oAuth.SuccessFlag;
                        auth.SalesOrderItemCode = objSalesOrderItem.Code;
                        auth.SalesOrderGroupID = numSalesOrderGroupId;
                        auth.AcqBank = arg_oAuth.AcqBank;
                        auth.CustomerID = arg_oAuth.CustomerID;
                        auth.AgreementID = arg_oAuth.AgreementID;
                        objItemOfSalesOrder = db_before.SalesOrder.Where(x1 => x1.Code == objSalesOrder.Code).FirstOrDefault<SalesOrder>();
                        auth.AccountID = objItemOfSalesOrder.AccountID;
                        strAccountId = objItemOfSalesOrder.AccountID.ToString();
                        auth.OrderNO = objItemOfSalesOrder.Code;
                        auth.Qty = (int)objSalesOrderItem.Qty;
                        //SalesOrderGroup price = db.SalesOrderGroup.Where(x => x.ID == nSalesOrderGroupId).FirstOrDefault<SalesOrderGroup>();
                        //Auth.Amount = price.PriceSum;
                        auth.Amount = arg_oAuth.Amount;
                        try
                        {
                            auth.AmountSelf = arg_oAuth.AmountSelf;
                        }
                        catch
                        {
                            auth.AmountSelf = 0;
                        }

                        auth.Bonus = arg_oAuth.Bonus;
                        auth.BonusBLN = (int)objSalesOrderItem.RedmBLN;

                        if (arg_oAuth.HpMark != null)
                        {
                            auth.HpMark = arg_oAuth.HpMark;
                        }
                        else
                        {
                            auth.HpMark = "";
                        }

                        auth.PriceFirst = arg_oAuth.PriceFirst;
                        try
                        {
                            auth.PriceOther = arg_oAuth.PriceOther;
                        }
                        catch
                        {
                            auth.PriceOther = 0;
                        }

                        auth.AuthCode = arg_oAuth.AuthCode;
                        auth.AuthSN = arg_oAuth.AuthSN;
                        if (arg_oAuth.AuthDate == null || DateTime.Compare(Convert.ToDateTime("1900/01/01"), arg_oAuth.AuthDate) >= 0)
                            auth.AuthDate = DateTime.Now;
                        else
                            auth.AuthDate = arg_oAuth.AuthDate;
                        auth.RspCode = arg_oAuth.RspCode;
                        auth.RspMSG = arg_oAuth.RspMSG;
                        auth.RspOther = arg_oAuth.RspOther;
                        //因資料庫未設定預設值為null, 故以1900/01/01為初始設定
                        auth.CancelDate = Convert.ToDateTime("1900/01/01");
                        auth.CancelRspCode = "";
                        auth.CancelRspMSG = "";
                        auth.FaildealUser = "";
                        auth.FaildealNote = "";
                        auth.FaildealDate = Convert.ToDateTime("1900/01/01"); //因資料庫未設定預設值為null, 故以1900/01/01為初始設定
                        auth.EraseDate = Convert.ToDateTime("1900/01/01"); //因資料庫未設定預設值為null, 故以1900/01/01為初始設定
                        auth.CreateUser = arg_oAuth.CreateUser;
                        auth.CreateDate = DateTime.Now;
                        auth.UpdateDate = Convert.ToDateTime("1900/01/01");
                        auth.AgreementID = "";

                        db_before.Auth.Add(auth);
                        data.Add(auth);
                        numAuthCount++;
                        break;  //懶得改成Auth只存一筆的判斷式, 使用break強制跳出迴圈, 讓Auth只存一筆
                    }

                    if (numAuthCount > 0)
                    {
                        break; //懶得改成Auth只存一筆的判斷式, 使用break強制跳出迴圈, 讓Auth只存一筆
                    }
                } //end foreach

                try
                {
                    db_before.SaveChanges();
                    logger.Error("save Auth done");
                }
                catch (Exception exA)
                {
                    string msg = "";
                    try
                    {
                        msg += exA.Message;
                    }
                    catch
                    {
                    }

                    try
                    {
                        msg += exA.InnerException.Message;
                    }
                    catch
                    {
                    }

                    try
                    {
                        msg += exA.InnerException.InnerException.Message;
                    }
                    catch
                    {
                    }

                    try
                    {
                        msg += exA.InnerException.InnerException.InnerException.Message;
                    }
                    catch
                    {
                    }

                    strErrorRecord += msg + "\r\n\r\n";
                    try
                    {
                        strErrorRecord += "Object:" + Newtonsoft.Json.JsonConvert.SerializeObject(data.FirstOrDefault());
                    }
                    catch
                    {
                    }
                }
                finally
                {
                    strErrorRecord += "------------end of create auth ------------\r\n";
                    strErrorRecord += String.Join(",", data.Select(x => x.ID).ToArray()) + "\r\n";
                }

                /* ------ end of 產生Auth ------*/
                #endregion

                #region 匯款失敗
                //匯款失敗, 要改成被動取消:salesorder_status=4
                payFailFlag = false;
                if ((arg_strPlat.Equals("CHINATRUST") && Convert.ToInt32(arg_strErrCode) != 0))
                {
                    payFailFlag = true;
                }
                else if ((arg_strPlat.Equals("ALLPAY") && !arg_strErrCode.Equals("1")))
                {
                    payFailFlag = true;
                }
                else if ((arg_strPlat.Equals("HITRUST") || arg_strPlat.Equals("NCCC")) && !arg_strErrCode.Equals("00"))
                {
                    payFailFlag = true;
                }

                if (payFailFlag)
                {
                    strErrorRecord += "------------money faild ------------\r\n";
                    TWSqlDBContext oDb = new TWSqlDBContext();
                    List<SalesOrder> listErrorSalesOrder = (from x in oDb.SalesOrder
                                                            join p in oDb.SalesOrderGroup on x.SalesOrderGroupID equals p.ID
                                                            join c in oDb.SalesOrder on p.ID equals c.SalesOrderGroupID
                                                            where x.Code == arg_strOrderNumber
                                                            select c).ToList();

                    if (listErrorSalesOrder != null && listErrorSalesOrder.Count > 0)
                    {
                        foreach (SalesOrder objSubItem in listErrorSalesOrder)
                        {
                            objSubItem.Status = (int)SalesOrder.status.付款失敗取消訂單;
                            objSubItem.Updated = objSubItem.Updated + 1;
                            objSubItem.UpdateDate = DateTime.Now;
                            objSubItem.UpdateUser = arg_strPlat;
                        } //end foreach
                        try
                        {
                            oDb.SaveChanges();
                        }
                        catch (Exception exB)
                        {
                            strErrorRecord += exB.Message + "\r\n";
                        }
                        listErrorSalesOrder.Clear();
                        listErrorSalesOrder = null;
                    } //end if
                    oDb.Dispose();
                    oDb = null;
                    strErrorRecord += "------------end of money faild ------------\r\n";

                    /* ------ 設定PromotionGift的狀態為匯款失敗不使用 ------ */
                    //objPromotionGiftService.UpdatePromotionGiftRecord(numSalesOrderGroupId, PromotionGiftRecords.UsedStatusOption.NotUsed);
                    Processor.Request<string, string>("Service.PromotionGiftService.PromotionGiftRecordRepository", "UpdatePromotionGiftRecordBySOGroupId", numSalesOrderGroupId, TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords.UsedStatusOption.NotUsed);
                    /* ------ end of 設定PromotionGift的狀態為匯款失敗不使用 ------ */
                } //end if
                #endregion

                #region 匯款成功產生PO及折價券
                /* ------ starting of 產生PO ------ */
                //匯款成功才需要產生PO
                //if ((arg_strPlat.Equals("CHINATRUST") && (arg_strErrCode.Length == 0 || Convert.ToInt32(arg_strErrCode) == 0)) || (arg_strPlat.Equals("ALLPAY") && arg_strErrCode.Equals("1")))
                if (!payFailFlag)
                {
                    strErrorRecord += "------------create po ------------\r\n";
                    TWNewEgg.Website.ECWeb.Service.PlaceOrder po = null;
                    po = new PlaceOrder();
                    List<string> needReSendList = new List<string>();
                    foreach (SalesOrder objSalesOrder in listSalesOrder)
                    {
                        try
                        {
                            strPoCode = "";
                            //sw.WriteLine("po new: " + oSalesOrder.salesorder_code);
                            strPoCode = po.SendPlaceOrder(numSalesOrderGroupId, objSalesOrder.Code);
                            objSalesOrder.Status = (int)SalesOrder.status.付款成功;
                            db_before.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            objSalesOrder.Status = (int)SalesOrder.status.付款成功拋單失敗;
                            db_before.SaveChanges();
                            needReSendList.Add(objSalesOrder.Code);
                            logger.Error(objSalesOrder.Code + ":" + e.Message + "_:_" + e.StackTrace);
                            strErrorRecord += e.Message + "\r\n";
                        }
                    } //end foreach
                    if (needReSendList.Count > 0)
                    {
                        CartController cartController = new CartController(HttpContext.Current);
                        cartController.SOReSendMail(needReSendList); // 寄發訂單失敗通知信
                    }
                    logger.Info("needReSendList[" + needReSendList.Count + "]");
                    logger.Info("折價券Start");
                    //設定使用的折價券
                    if (System.Configuration.ConfigurationManager.AppSettings.Get("CouponFunction").ToUpper().Equals("ON"))
                    {
                        logger.Info("折價券In");
                        this.AddSalesOrderCoupon(numSalesOrderGroupId, listSalesOrder.First().AccountID.ToString());
                    }
                    logger.Info("折價券End");

                    /* ------ 設定PromotionGift的狀態為匯款成功已使用 ------ */
                    //objPromotionGiftService.UpdatePromotionGiftRecord(numSalesOrderGroupId, PromotionGiftRecords.UsedStatusOption.Used);
                    Processor.Request<string, string>("Service.PromotionGiftService.PromotionGiftRepository", "UpdatePromotionGiftRecordBySOGroupId", numSalesOrderGroupId, TWNewEgg.Models.DomainModels.Redeem.PromotionGiftRecords.UsedStatusOption.Used);
                    /* ------ end of 設定PromotionGift的狀態為匯款成功已使用 ------ */
                }

                strErrorRecord += "------------end of create po ------------\r\n";
                /* ------ end of 產生PO ------ */
                #endregion
            }
            catch (Exception ex)
            {
                strErrorRecord += "big try-catch: " + ex.ToString() + "\r\n";
            }
            finally
            {
                strErrorRecord = "===================" + DateTime.Now + "===================\r\n" + strErrorRecord;
                try
                {
                    logger.Error(strErrorRecord);
                }
                catch
                {
                }

                //釋放所有記憶體
                if (db_before != null)
                {
                    db_before.Dispose();
                }

                objMasterSalesOrder = null;

                listSalesOrderItem = null;
                listSalesOrder = null;
                objItemOfSalesOrder = null;

            } //end finally
        } //end createPO 

        public void AddSalesOrderCoupon(int arg_nSalesOrderGroupId, string arg_strAccountId)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            List<SalesOrderItem> listSalesOrderItem = null;
            List<TWNewEgg.Models.ViewModels.Redeem.Coupon> listUserActiveCoupon = null;
            List<TWNewEgg.Models.DomainModels.Redeem.Coupon> listUpdateCoupon = null;
            TWNewEgg.Models.ViewModels.Redeem.Coupon objCoupon = null;
            TWNewEgg.Models.DomainModels.Redeem.Coupon objDomainCoupon = null;

            oDb = new DB.TWSqlDBContext();
            //取得所有的有使用Coupon的SalesOrder
            listSalesOrderItem = (from x in oDb.SalesOrderItem
                                  join p in oDb.SalesOrder on x.SalesorderCode equals p.Code
                                  where p.SalesOrderGroupID == arg_nSalesOrderGroupId
                                  && x.Coupons.Length > 0
                                  select x).ToList();

            if (listSalesOrderItem != null && listSalesOrderItem.Count > 0)
            {
                listUpdateCoupon = new List<TWNewEgg.Models.DomainModels.Redeem.Coupon>();
                //取得該User所有可以使用的Coupon清單
                //listUserActiveCoupon = objCouponService.getActiveCouponListByAccount(arg_strAccountId);
                listUserActiveCoupon = Processor.Request<List<TWNewEgg.Models.ViewModels.Redeem.Coupon>, List<TWNewEgg.Models.DomainModels.Redeem.Coupon>>("Service.CouponService.CouponServiceRepository", "getActiveCouponListByAccount", arg_strAccountId).results;
                if (listUserActiveCoupon != null && listUserActiveCoupon.Count > 0)
                {
                    foreach (SalesOrderItem objSubSalesOrderItem in listSalesOrderItem)
                    {
                        objCoupon = listUserActiveCoupon.Where(x => x.id == Convert.ToInt32(objSubSalesOrderItem.Coupons)).FirstOrDefault();
                        if (objCoupon != null)
                        {
                            objDomainCoupon = ModelConverter.ConvertTo<TWNewEgg.Models.ViewModels.Redeem.Coupon, TWNewEgg.Models.DomainModels.Redeem.Coupon>(objCoupon);
                            //設定Coupon狀態為已使用(下次不可以再使用)
                            objDomainCoupon.usestatus = (int)TWNewEgg.Models.DomainModels.Redeem.Coupon.CouponUsedStatusOption.Used;
                            objDomainCoupon.usedate = DateTime.Now;
                            objDomainCoupon.ordcode = objSubSalesOrderItem.Code;
                            objDomainCoupon.updatedate = DateTime.Now;
                            objDomainCoupon.updateuser = arg_strAccountId;
                            listUpdateCoupon.Add(objDomainCoupon);
                        }
                    } //end foreach
                } //end if

                try
                {
                    //更新Coupon的資訊
                    //objCouponService.editCouponList(listUpdateCoupon);
                    Processor.Request<bool, bool>("Service.CouponService.CouponServiceRepository", "editCouponList", listUpdateCoupon);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.InnerException);
                }
            }
            //釋放記憶體
            if (oDb != null)
            {
                oDb.Dispose();
            }
            listSalesOrderItem = null;
            listUserActiveCoupon = null;
            listUpdateCoupon = null;
            objCoupon = null;
        }
    }
}