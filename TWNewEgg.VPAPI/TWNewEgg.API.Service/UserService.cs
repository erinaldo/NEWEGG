using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Transactions;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// Account Service
    /// </summary>
    public class UserService
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sellerCreation"></param>
        /// <param name="sellerCharge"></param>
        /// <returns></returns>
        public Models.ActionResponse<string> CreateVendorOrSeller(Models.SellerCreation sellerInfo, Models.SaveSellerCharge sellerCharge, int userid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            if (sellerInfo.AccountType == "S")
            {
                sellerInfo.GroupID = 1;
            }
            else if (sellerInfo.AccountType == "V")
            {
                sellerInfo.GroupID = 8;
            }
            sellerInfo.InUserID = userid;
            // 增加檢查sellerName必填
            if (string.IsNullOrEmpty(sellerInfo.SellerName))
            {
                result.Msg = "請填入商家名稱";
                result.IsSuccess = false;
                return result;
            }
            // 增加檢查companyCode必填
            if (string.IsNullOrEmpty(sellerInfo.CompanyTaxId_Identity))
            {
                result.Msg = "請填入統一編號/身分證";
                result.IsSuccess = false;
                return result;
            }
            sellerInfo.CompanyCode = sellerInfo.CompanyTaxId_Identity;
            // 增加檢查:如果選擇了seller卻沒有填入佣金率
            if (sellerInfo.AccountType == "S")
            {
                if (sellerCharge.CommissionRate == null || sellerCharge.CommissionRate.Count == 0)
                {
                    result.Msg = "請選擇或自訂填寫佣金率!";
                    result.IsSuccess = false;
                    return result;
                }
            }
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    TWNewEgg.API.Service.UserService userService = new UserService();
                    TWNewEgg.API.Service.SellerInvitationService sellerinvitation = new SellerInvitationService();
                    var result_createSeller = this.CreateSeller(sellerInfo);
                    //CreateSeller success
                    if (result_createSeller.IsSuccess == true)
                    {
                        if (sellerInfo.AccountType == "V")
                        {
                            result.IsSuccess = true;
                            result.Msg = result_createSeller.Msg;
                            scope.Complete();
                        }
                        else if (sellerInfo.AccountType == "S")
                        {
                            // 儲存佣金至Seller
                            sellerCharge.SellerID = result_createSeller.Body.SellerID;
                            sellerCharge.SellerCountryCode = sellerInfo.Region;
                            sellerCharge.ChargeType = sellerCharge.ChargeType;

                            sellerCharge.CommissionRate = sellerCharge.CommissionRate;
                            //TWNewEgg.API.View.Service.AES aes = new Service.AES();
                            //sellerCharge.InUserID = sellerInfo_fromCookies.UserID;
                            sellerCharge.InUserID = userid;
                            // ChargeType為S(標準費率)，Commission強制搜尋為原標準預設值
                            if (sellerCharge.ChargeType == "S")
                            {
                                List<TWNewEgg.API.Models.GetSellerChargeResult> getSellerChargeResult = sellerinvitation.GetChargeList(new TWNewEgg.API.Models.GetSellerCharge() { SellerID = 0, ChargeType = "S", CountryCode = "TW" }).Body;
                                sellerCharge.CommissionRate.Clear();
                                foreach (var getSellerChargeResult_element in getSellerChargeResult)
                                {
                                    sellerCharge.CommissionRate.Add(new TWNewEgg.API.Models.SaveSellerCharge.CommissionRateInfo() { CategoryID = getSellerChargeResult_element.CategoryID, Commission = getSellerChargeResult_element.Commission });
                                }
                            }
                            var result_saveCommission = sellerinvitation.SaveSellerCharge(sellerCharge);
                            if (result_saveCommission.IsSuccess == true && result_saveCommission.Code == (int)ResponseCode.Success)
                            {
                                result.IsSuccess = true;
                                result.Msg = "傭金設定儲存成功";
                                scope.Complete();
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Msg = result_saveCommission.Msg;
                            }

                        }
                    }
                    else
                    {
                        //CreateSeller create failed
                        result.Msg = result_createSeller.Msg;
                        result.IsSuccess = false;
                        return result;
                    }
                    
                }
                catch (Exception error)
                {
                    result.Msg = error.Message;
                    result.IsSuccess = false;
                    
                }
            }
            return result;
        }
        /// <summary>
        /// 建立商家
        /// </summary>
        /// <param name="sellerCreation"></param>
        /// <returns></returns>
        public Models.ActionResponse<Models.SellerCreationResult> CreateSeller(Models.SellerCreation sellerCreation)
        {
            Models.ActionResponse<Models.SellerCreationResult> result = new Models.ActionResponse<Models.SellerCreationResult>();

            int sellerID;
            Models.ActionResponse<Models.UserCreationResult> createUserResult;

            if (string.IsNullOrWhiteSpace(sellerCreation.SellerEmail) || string.IsNullOrWhiteSpace(sellerCreation.SellerName)
                                                                      || string.IsNullOrWhiteSpace(sellerCreation.Region)
                                                                      || string.IsNullOrWhiteSpace(sellerCreation.Language)
                                                                      || string.IsNullOrWhiteSpace(sellerCreation.Status)
                                                                      || string.IsNullOrWhiteSpace(sellerCreation.AccountType)
                                                                      || sellerCreation.InUserID == null
                                                                      || sellerCreation.GroupID == null
                                                                      || sellerCreation.Identy == null
                                                                      || sellerCreation.BillingCycle == null
                                                                      || string.IsNullOrWhiteSpace(sellerCreation.CompanyTaxId_Identity))
            {
                result.Code = (int)UserLoginingResponseCode.RequiredFieldEmpty;
                result.IsSuccess = false;
                result.Msg = "Seller Creation Failed. Not all required fields are filled.";

                return result;
            }
            sellerCreation.CompanyCode = sellerCreation.CompanyCode.Trim();
            sellerCreation.SellerEmail = sellerCreation.SellerEmail.Trim();
            sellerCreation.SellerName = sellerCreation.SellerName.Trim();

            //using (TransactionScope scope = new TransactionScope())
            //{
            DB.TWSELLERPORTALDB.Models.Seller_BasicInfo basicInfo = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();

            //SellerID = Max(SellerID)+1 2014.3.20
            int maxSellerID;

            if ((SellerPortalDB.Seller_BasicInfo.Max(x => x.SellerID)) != null)
            {
                maxSellerID = SellerPortalDB.Seller_BasicInfo.Max(x => x.SellerID);
            }
            else
            {
                maxSellerID = 0;
            }
            basicInfo.SellerID = maxSellerID + 1;

            basicInfo.SellerEmail = sellerCreation.SellerEmail;
            basicInfo.SellerName = sellerCreation.SellerName;
            basicInfo.SellerCountryCode = sellerCreation.Region;
            basicInfo.LanguageCode = sellerCreation.Language;
            basicInfo.SellerStatus = sellerCreation.Status;
            basicInfo.AccountTypeCode = sellerCreation.AccountType;
            basicInfo.Currency = sellerCreation.Currency;
            basicInfo.InUserID = sellerCreation.InUserID;
            basicInfo.UpdateUserID = sellerCreation.InUserID;
            basicInfo.ActiveatedUserID = sellerCreation.InUserID;
            //廠商身分別
            basicInfo.BillingCycle = sellerCreation.BillingCycle;
            //付款方式
            basicInfo.Identy = sellerCreation.Identy;
            //公司編號或身分證
            basicInfo.CompanyCode = sellerCreation.CompanyTaxId_Identity;
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);
            basicInfo.InDate = dt;
            basicInfo.UpdateDate = dt;
            basicInfo.CreateDate = dt;
            basicInfo.ActiveatedDate = dt;
            try
            {
                SellerPortalDB.Seller_BasicInfo.Add(basicInfo);
                SellerPortalDB.SaveChanges();
            }
            catch (Exception ex)
            {
                var sellerExist = SellerPortalDB.Seller_BasicInfo.Where(x => x.SellerEmail == sellerCreation.SellerEmail).FirstOrDefault();

                if (sellerExist != null)
                {
                    result.Code = (int)UserLoginingResponseCode.SellerExist;
                    result.IsSuccess = false;
                    result.Msg = "Seller Creation Failed. The seller already exists. SellerID: " + sellerExist.SellerID;

                    return result;
                }
                else
                {
                    result.Code = (int)UserLoginingResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "Seller Creation Failed. An unexpected error occurred.";

                    return result;
                }
            }

            sellerID = SellerPortalDB.Seller_BasicInfo.Where(x => x.SellerEmail == sellerCreation.SellerEmail).FirstOrDefault().SellerID;

            #region Create User

            Models.UserCreation userCreation = new Models.UserCreation();
            userCreation.Email = sellerCreation.SellerEmail;
            userCreation.SellerID = maxSellerID + 1; //SellerID來自剛剛的CreateSeller
            userCreation.GroupID = sellerCreation.GroupID;
            userCreation.InUserID = sellerCreation.InUserID;
            userCreation.PurviewType = "S";

            createUserResult = new Models.ActionResponse<Models.UserCreationResult>();
            try
            {
                createUserResult = CreateUser(userCreation);
                //scope.Complete(); //TransactionScope 結束
            }
            catch (Exception ex)
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = ex.InnerException.InnerException.Message;

                return result;
            }

            if (createUserResult.IsSuccess == false)
            {
                result.Code = createUserResult.Code;
                result.IsSuccess = false;
                result.Msg = createUserResult.Msg;

                return result;
            }
            #endregion
            //}

            //2014.6.17 寫到前台seller的service add by ice begin
            Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
            TWService twser = new TWService();
            massage = twser.UpdateTWSeller(userCreation.Email, basicInfo.AccountTypeCode);
            if (massage.IsSuccess == false)
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = massage.IsSuccess;
                result.Msg = massage.Msg;

                return result;
            }
            //2014.6.17 寫到前台seller的service add by ice end

            result.Code = (int)UserLoginingResponseCode.Success;
            result.IsSuccess = true;
            result.Msg = "Seller Creation Succeeded";
            result.Body = new Models.SellerCreationResult();
            result.Body.SellerID = maxSellerID + 1; //SellerID來自CreateSeller
            result.Body.UserID = createUserResult.Body.UserID;
            result.Body.RanCode = createUserResult.Body.RanCode;

            int convertTemp = 0;
            if (int.TryParse(massage.Body, out convertTemp) == false)
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = false;
            }
            else
            {
                result.Code = (int)UserLoginingResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Seller Creation Succeeded";
                result.Body = new Models.SellerCreationResult();
                result.Body.SellerID = convertTemp;
                //result.Body.SellerID = maxSellerID + 1; //SellerID來自CreateSeller
                result.Body.UserID = createUserResult.Body.UserID;
                result.Body.RanCode = createUserResult.Body.RanCode;
            }
            

            return result;

        }

        /// <summary>
        /// 建立使用者
        /// </summary>
        /// <param name="userCreation"></param>
        /// <returns></returns>

        public Models.ActionResponse<Models.UserCreationResult> CreateUser(Models.UserCreation userCreation)
        {
            Models.ActionResponse<Models.UserCreationResult> result = new Models.ActionResponse<Models.UserCreationResult>();

            if (string.IsNullOrWhiteSpace(userCreation.Email) || userCreation.SellerID == null
                                                              || userCreation.GroupID == null
                                                              || userCreation.InUserID == null
                                                              || string.IsNullOrWhiteSpace(userCreation.PurviewType))
            {
                result.Code = (int)UserLoginingResponseCode.RequiredFieldEmpty;
                result.IsSuccess = false;
                result.Msg = "User Created Fail. Not all required fields are filled.";

                return result;
            }

            userCreation.Email = userCreation.Email.Trim();

            if ((userCreation.PurviewType != "U") && (userCreation.PurviewType != "S") && (userCreation.PurviewType != "G") && (userCreation.PurviewType != "N"))
            {
                result.Code = (int)UserLoginingResponseCode.PurviewTypeError;
                result.IsSuccess = false;
                result.Msg = "User Created Fail. The purview type is invalid.";

                return result;
            }

            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = new DB.TWSELLERPORTALDB.Models.Seller_User();

            //UserID = Max(UserID)+1 2014.3.20
            int maxUserID;

            if ((SellerPortalDB.Seller_User.Max(x => x.UserID)) != null)
                maxUserID = SellerPortalDB.Seller_User.Max(x => x.UserID);
            else
                maxUserID = 0;

            user.UserID = maxUserID + 1;

            user.UserEmail = userCreation.Email;
            user.SellerID = userCreation.SellerID;
            user.GroupID = userCreation.GroupID;
            user.Status = "I";
            user.PurviewType = userCreation.PurviewType;

            user.InUserID = userCreation.InUserID;
            user.UpdateUserID = userCreation.InUserID;

            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            var a = Convert.ToDateTime(dt);
            user.InDate = dt;
            user.UpdateDate = dt;
            
            //Create Random String 獨立成一個function 2014.2.21
            //新增使用者邏輯 by Jack Lin 2013.11.21
            string ranCode = CreateRandomString();
            user.RanCode = ranCode;


            try
            {
                SellerPortalDB.Seller_User.Add(user);
                SellerPortalDB.SaveChanges();
            }
            catch (Exception ex)
            {
                var userExist = SellerPortalDB.Seller_User.Where(x => x.UserEmail == userCreation.Email).FirstOrDefault();

                if (userExist != null)
                {
                    result.Code = (int)UserLoginingResponseCode.UserExist;
                    result.IsSuccess = false;
                    result.Msg = "User Creation Failed. The user already exists. UserID: " + userExist.UserID;

                    return result;
                }
                else
                {
                    result.Code = (int)UserLoginingResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "User Creation Failed. An unexpected error occurred.";

                    return result;
                }
            }

            result.Code = (int)UserLoginingResponseCode.Success;
            result.IsSuccess = true;
            result.Msg = "User Creation Succeeded";
            result.Body = new Models.UserCreationResult();
            result.Body.RanCode = ranCode;

            int userID = SellerPortalDB.Seller_User.Where(x => x.UserEmail == user.UserEmail).FirstOrDefault().UserID;
            result.Body.UserID = userID;
            
            return result;
        }
        
        /// <summary>
        /// 查詢使用者狀態（設定密碼頁面起始用）
        /// </summary>
        /// <param name="userCheckStatus"></param>
        /// <returns></returns>
        public Models.ActionResponse<Models.UserCheckStatusResult> CheckStatus(Models.UserCheckStatus userCheckStatus)
        {
            Models.ActionResponse<Models.UserCheckStatusResult> result = new Models.ActionResponse<Models.UserCheckStatusResult>();

            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == userCheckStatus.UserEmail
                                                                                                && x.RanCode == userCheckStatus.RanCode
                                                                                                && (x.Status == "I" || x.Status == "R")).FirstOrDefault();

            if (user == null)
            {
                result.Code = (int)UserLoginingResponseCode.URLFailedValidation;
                result.IsSuccess = false;
                result.Msg = "The forward URL does not exist or it has failed validation.";
            }
            else
            {
                if (user.Status == "R")
                {
                    DateTime dt = DateTime.UtcNow.AddHours(8);
                    dt.GetDateTimeFormats('r')[0].ToString();
                    Convert.ToDateTime(dt);

                    if (user.UpdateDate.HasValue)
                    {
                        TimeSpan timeSpan = new TimeSpan(dt.Ticks - user.UpdateDate.Value.Ticks);
                        if (timeSpan.TotalSeconds >= 86400)
                        {
                            result.Code = (int)UserLoginingResponseCode.ResetPasswordTimeOut;
                            result.IsSuccess = false;
                            result.Msg = "The link in email has expired. Please resubmit your password reset request by clicking on \"Forgot your password\" again.";

                            return result;
                        }
                    }

                    
                }

                result.Code = (int)UserLoginingResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Query Successful";
                result.Body = new Models.UserCheckStatusResult();
                result.Body.UserID = user.UserID;
                result.Body.Status = user.Status;
            }

            return result;
        }

        /// <summary>
        /// 查詢使用者存在與否（修改密碼頁面用）
        /// </summary>
        /// <param name="UserEmail"></param>
        /// <returns></returns>
        public Models.ActionResponse<int> CheckExist(string UserEmail)
        {
            Models.ActionResponse<int> result = new Models.ActionResponse<int>();

            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == UserEmail && (x.Status == "E")).SingleOrDefault();

            if (user == null)
            {
                result.Code = (int)UserLoginingResponseCode.URLFailedValidation;
                result.IsSuccess = false;
                result.Msg = "The forward URL does not exist or it has failed validation.";
            }
            else
            {
                result.Code = (int)UserLoginingResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Query Successful";
                result.Body = user.UserID;
            }

            return result;
        }
        
        /// <summary>
        /// 使用者登入
        /// （Seller_User.Status必須為"E"或"R"）
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public Models.ActionResponse<Models.UserLoginResult> Login(Models.UserLogin userLogin)
        {
            Models.ActionResponse<Models.UserLoginResult> result = new Models.ActionResponse<Models.UserLoginResult>();

            if (string.IsNullOrWhiteSpace(userLogin.UserEmail) || string.IsNullOrEmpty(userLogin.Password))
            {
                result.Code = (int)UserLoginingResponseCode.PasswordFaild;

                result.IsSuccess = false;
                result.Msg = "Login Failed";

                return result;
            }

            userLogin.UserEmail = userLogin.UserEmail.Trim();

            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == userLogin.UserEmail
                                                                                                && (x.Status == "E" || x.Status == "R")).FirstOrDefault();
            if (user == null)
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Login Failed";

                return result;
            }
            else
            {
                string sellerStatus = SellerPortalDB.Seller_BasicInfo.Where(x => x.SellerID == user.SellerID && x.AccountTypeCode == userLogin.VendorSeller).Select(r => r.SellerStatus).FirstOrDefault();

                if (sellerStatus == "C")
                {
                    result.Code = (int)UserLoginingResponseCode.Accountalreadystop;
                    result.IsSuccess = false;
                    result.Msg = "This account is already stop";

                    return result;
                }


                //更改密碼Hash方法為SHA-2(SHA512) 2014.2.7
                //string  = AesEncryptor.AesEncrypt(userLogin.Password).GetHashCode().ToString();
                string Pwd = AesEncryptor.AesEncrypt(userLogin.Password + user.RanNum);

                SHA512 sha512 = new SHA512CryptoServiceProvider(); //建立一個SHA512
                byte[] source = Encoding.Default.GetBytes(Pwd); //將字串轉為Byte[]
                byte[] crypto = sha512.ComputeHash(source); //進行SHA512加密

                string PwdHashed = Convert.ToBase64String(crypto); //把加密後的字串從Byte[]轉為字串

                user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == userLogin.UserEmail
                                                             && x.Pwd == PwdHashed
                                                             && (x.Status == "E" || x.Status == "R")).FirstOrDefault();

                if (user == null)
                {
                    result.Code = (int)UserLoginingResponseCode.PasswordFaild;
                    result.IsSuccess = false;
                    result.Msg = "Login Failed";

                    return result;
                }
                else if (user.Status == "R")
                {
                    DateTime dt = DateTime.UtcNow.AddHours(8);
                    dt.GetDateTimeFormats('r')[0].ToString();
                    Convert.ToDateTime(dt);

                    user.Status = "E";
                    user.UpdateUserID = user.UserID;
                    user.UpdateDate = dt;
                    SellerPortalDB.Entry(user).State = EntityState.Modified;
                    SellerPortalDB.SaveChanges();
                }


                if (VendoeSellerCheck(userLogin.UserEmail, userLogin.VendorSeller) == false)
                {
                    result.Code = (int)UserLoginingResponseCode.AccountTypeError;
                    result.IsSuccess = false;
                    result.Msg = "This AccountTypeCode is error";
                    return result;
                }
                result = returnData(user, userLogin.VendorSeller);
            }

            return result;
        }
        public Models.ActionResponse<Models.UserLoginResult> AutoLogin(Models.UserLogin userLogin)
        {
            Models.ActionResponse<Models.UserLoginResult> result = new Models.ActionResponse<Models.UserLoginResult>();
            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();
            //Password is accessToken
            if (string.IsNullOrWhiteSpace(userLogin.UserEmail) || string.IsNullOrEmpty(userLogin.Password))
            {
                result.Code = (int)UserLoginingResponseCode.PasswordFaild;

                result.IsSuccess = false;
                result.Msg = "Login Failed";

                return result;
            }

            var LoginSuccessData = SellerPortalDB.Seller_User.Where(p => p.UserEmail == userLogin.UserEmail && p.AccessToken == userLogin.Password).FirstOrDefault();
            if (LoginSuccessData == null)
            {
                result.Code = (int)UserLoginingResponseCode.Error;

                result.IsSuccess = false;
                result.Msg = "Login Failed";

                return result;
            }
            userLogin.UserEmail = userLogin.UserEmail.Trim();

            
            DB.TWSELLERPORTALDB.Models.Seller_User user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == userLogin.UserEmail
                                                                                                && (x.Status == "E" || x.Status == "R")).FirstOrDefault();
            if (user == null)
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Login Failed";

                return result;
            }
            else
            {
                string sellerStatus = SellerPortalDB.Seller_BasicInfo.Where(x => x.SellerID == user.SellerID).Select(r => r.SellerStatus).FirstOrDefault();

                if (sellerStatus == "C")
                {
                    result.Code = (int)UserLoginingResponseCode.Accountalreadystop;
                    result.IsSuccess = false;
                    result.Msg = "This account is already stop";

                    return result;
                }


                //更改密碼Hash方法為SHA-2(SHA512) 2014.2.7
                //string  = AesEncryptor.AesEncrypt(userLogin.Password).GetHashCode().ToString();
                //string Pwd = AesEncryptor.AesEncrypt(userLogin.Password + user.RanNum);

                //SHA512 sha512 = new SHA512CryptoServiceProvider(); //建立一個SHA512
                //byte[] source = Encoding.Default.GetBytes(Pwd); //將字串轉為Byte[]
                //byte[] crypto = sha512.ComputeHash(source); //進行SHA512加密

                //string PwdHashed = Convert.ToBase64String(crypto); //把加密後的字串從Byte[]轉為字串
                string PwdHashed = LoginSuccessData.Pwd;
                user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == userLogin.UserEmail
                                                             && x.Pwd == PwdHashed
                                                             && (x.Status == "E" || x.Status == "R")).FirstOrDefault();

                if (user == null)
                {
                    result.Code = (int)UserLoginingResponseCode.PasswordFaild;
                    result.IsSuccess = false;
                    result.Msg = "Login Failed";

                    return result;
                }
                else if (user.Status == "R")
                {
                    DateTime dt = DateTime.UtcNow.AddHours(8);
                    dt.GetDateTimeFormats('r')[0].ToString();
                    Convert.ToDateTime(dt);

                    user.Status = "E";
                    user.UpdateUserID = user.UserID;
                    user.UpdateDate = dt;
                    SellerPortalDB.Entry(user).State = EntityState.Modified;
                    SellerPortalDB.SaveChanges();
                }


                if (VendoeSellerCheck(userLogin.UserEmail, userLogin.VendorSeller) == false)
                {
                    result.Code = (int)UserLoginingResponseCode.AccountTypeError;
                    result.IsSuccess = false;
                    result.Msg = "This AccountTypeCode is error";
                    return result;
                }
                result = returnData(user, userLogin.VendorSeller);
            }

            return result;
        }
        /// <summary>
        /// 加密資料
        /// </summary>
        /// <param name="_strEncryptionData"></param>
        /// <returns></returns>
        public string Encryption(string _strEncryptionData)
        {
            string _strData = AesEncryptor.AesEncrypt(_strEncryptionData);
            return _strData;
        }
        public Models.ActionResponse<Models.UserLoginResult> returnData(DB.TWSELLERPORTALDB.Models.Seller_User DataInsertToReturn, string vendorSeller)
        {
            DB.TWSellerPortalDBContext sellerDB = new DB.TWSellerPortalDBContext();
            Models.ActionResponse<Models.UserLoginResult> result = new Models.ActionResponse<Models.UserLoginResult>();
            result.Code = (int)UserLoginingResponseCode.Success;
            result.Msg = "Login Successful";
            result.IsSuccess = true;
            result.Body = new Models.UserLoginResult();
            result.Body.UserID = Convert.ToString(Encryption(Convert.ToString(DataInsertToReturn.UserID)));
            var sellerData = sellerDB.Seller_User.Where(p => p.UserEmail == DataInsertToReturn.UserEmail).FirstOrDefault();
            var _intSellerID = sellerData.SellerID;
            var _intGroupID = sellerData.GroupID;
            if (_intSellerID == -1)
            {
                result.Body.SellerID = Convert.ToString(Encryption(Convert.ToString(sellerDB.Seller_BasicInfo.Where(p => p.SellerEmail == DataInsertToReturn.UserEmail && p.AccountTypeCode.ToUpper() == vendorSeller.ToUpper()).Select(p => p.SellerID).FirstOrDefault())));
            }
            else
            {
                result.Body.SellerID = Encryption(Convert.ToString(_intSellerID)).ToString();
            }
            result.Body.Token = DataInsertToReturn.AccessToken;
            result.Body.AccessToken = DataInsertToReturn.AccessToken;
            result.Body.UserEmail = Encryption(DataInsertToReturn.UserEmail);
            result.Body.GroupID = Encryption(_intGroupID.ToString());
            result.Body.AccountTypeCode = Encryption(vendorSeller);
            return result;
        }
        /// <summary>
        /// 設定密碼
        /// （Seller_User.Status必須為"I"或"E"）
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        public Models.ActionResponse<Models.UserLoginResult> SetPassword(Models.UserChangePassword changePassword)
        {
            Models.ActionResponse<Models.UserLoginResult> result = new Models.ActionResponse<Models.UserLoginResult>();

            if (string.IsNullOrWhiteSpace(changePassword.UserEmail) || string.IsNullOrWhiteSpace(changePassword.RanCode) 
                                                                    || string.IsNullOrEmpty(changePassword.NewPassword)
                                                                    || changePassword.UpdateUserID == null)
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Password Change Failed";

                return result;
            }

            changePassword.UserEmail = changePassword.UserEmail.Trim();
                       
            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == changePassword.UserEmail
                                                                                                && x.RanCode == changePassword.RanCode
                                                                                                && (x.Status == "I" || x.Status == "R")).FirstOrDefault();

            if (user == null)
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Password Change Failed";
            }
            else
            {
                //Create Random String 獨立成一個function 2014.2.21
                //密碼亂數獨立 by Jack Lin 2014.2.21
                string ranNum = CreateRandomString();

                //密碼+RanNum 2014.2.14
                //更改密碼Hash方法為SHA-2(SHA512) 2014.2.7
                string NewPwd = AesEncryptor.AesEncrypt(changePassword.NewPassword + ranNum);
                //int NewPwdHashed = NewPwd.GetHashCode();
                SHA512 sha512 = new SHA512CryptoServiceProvider(); //建立一個SHA512
                byte[] source = Encoding.Default.GetBytes(NewPwd); //將字串轉為Byte[]
                byte[] crypto = sha512.ComputeHash(source); //進行SHA512加密
                string NewPwdHashed = Convert.ToBase64String(crypto); //把加密後的字串從Byte[]轉為字串

                DateTime dt = DateTime.UtcNow.AddHours(8);
                dt.GetDateTimeFormats('r')[0].ToString();
                Convert.ToDateTime(dt);

                user.RanNum = ranNum;
                user.Pwd = NewPwdHashed;
                user.Status = "E";
                user.UpdateUserID = changePassword.UpdateUserID;
                user.UpdateDate = dt;

                user.AccessToken = AesEncryptor.AesEncrypt(
                                    changePassword.UserEmail
                                        + user.Pwd.ToString()
                                        + DateTime.UtcNow.AddHours(8).ToString()
                                    );
                SellerPortalDB.Entry(user).State = EntityState.Modified;
                SellerPortalDB.SaveChanges();

                //Generate AccessToken
                /*user.AccessToken = AesEncryptor.AesEncrypt(user.UserName + DateTime.UtcNow.ToString());
                user.UpdateDate = DateTime.UtcNow.AddHours(8);
                user.UpdateUserID = 0;
                backendDB.SaveChanges();*/
                string accountType = SellerPortalDB.Seller_BasicInfo.Where(p => p.SellerID == user.SellerID).Select(p=>p.AccountTypeCode).FirstOrDefault();
                result.Code = (int)UserLoginingResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Password Changed Successfully";
                result.Body = new Models.UserLoginResult();
                result.Body.UserEmail = user.UserEmail;
                result.Body.AccessToken = user.AccessToken;
                result.Body.UserID = user.UserID.ToString();
                result.Body.Token = user.AccessToken;
                result.Body.AccountTypeCode = accountType.ToString();
                result.Body.Password = user.Pwd;
            }
            return result;
        }

        /// <summary>
        /// 更改密碼
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        public Models.ActionResponse<Models.UserLoginResult> ChangeOldPassword(Models.UserChangePassword changePassword)
        {
            Models.ActionResponse<Models.UserLoginResult> result = new Models.ActionResponse<Models.UserLoginResult>();

            if (string.IsNullOrWhiteSpace(changePassword.UserEmail) || string.IsNullOrEmpty(changePassword.OldPassword)
                                                                    || string.IsNullOrEmpty(changePassword.NewPassword)
                                                                    || changePassword.UpdateUserID == null)
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Password Change Failed";

                return result;
            }

            changePassword.UserEmail = changePassword.UserEmail.Trim();

            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == changePassword.UserEmail).FirstOrDefault();

            //密碼+RanNum 2014.2.14
            //更改密碼Hash方法為SHA-2(SHA512) 2014.2.7
            //string OldPwdHashed = AesEncryptor.AesEncrypt(changePassword.OldPassword).GetHashCode().ToString();
            string OldPwd = AesEncryptor.AesEncrypt(changePassword.OldPassword + user.RanNum);
            SHA512 sha512 = new SHA512CryptoServiceProvider(); //建立一個SHA512
            byte[] source = Encoding.Default.GetBytes(OldPwd); //將字串轉為Byte[]
            byte[] crypto = sha512.ComputeHash(source); //進行SHA512加密
            string OldPwdHashed = Convert.ToBase64String(crypto); //把加密後的字串從Byte[]轉為字串

            user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == changePassword.UserEmail
                                                         && x.Pwd == OldPwdHashed).FirstOrDefault();

            if (user == null)
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Password Change Failed";
            }
            else
            {
                //Create Random String 獨立成一個function 2014.2.21
                string ranNum = CreateRandomString();
                //更改密碼Hash方法為SHA-2(SHA512) 2014.2.7
                string NewPwd = AesEncryptor.AesEncrypt(changePassword.NewPassword + ranNum);
                //int NewPwdHashed = NewPwd.GetHashCode();
                source = Encoding.Default.GetBytes(NewPwd); //將字串轉為Byte[]
                crypto = sha512.ComputeHash(source); //進行SHA512加密
                string NewPwdHashed = Convert.ToBase64String(crypto); //把加密後的字串從Byte[]轉為字串

                user.RanNum = ranNum;
                user.Pwd = NewPwdHashed;
                if (changePassword.UpdateUserID != null)
                    user.UpdateUserID = changePassword.UpdateUserID;
                else
                    user.UpdateUserID = user.UserID;

                DateTime dt = DateTime.UtcNow.AddHours(8);
                dt.GetDateTimeFormats('r')[0].ToString();
                Convert.ToDateTime(dt);
                user.UpdateDate = dt;

                SellerPortalDB.Entry(user).State = EntityState.Modified;
                SellerPortalDB.SaveChanges();

                //Generate AccessToken
                /*user.AccessToken = AesEncryptor.AesEncrypt(user.UserName + DateTime.UtcNow.ToString());
                user.UpdateDate = DateTime.UtcNow.AddHours(8);
                user.UpdateUserID = 0;
                backendDB.SaveChanges();*/

                result.Code = (int)UserLoginingResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Password Changed Successfully";
                result.Body = new Models.UserLoginResult();

                result.Body.UserID = user.UserID.ToString();
                result.Body.Token = user.AccessToken;
            }
            return result;
        }

        /// <summary>
        /// 重設密碼申請
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        public Models.ActionResponse<Models.ResetPasswordResult> ResetPassword(TWNewEgg.API.Models.ResetPassword resetPassword)
        {
            string userEmail = resetPassword.UserEmail;

            Models.ActionResponse<Models.ResetPasswordResult> result = new Models.ActionResponse<Models.ResetPasswordResult>();

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                result.Code = (int)UserLoginingResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Password Reset Failed";

                return result;
            }

            userEmail = userEmail.Trim();

            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == userEmail
                                                                                                && (x.Status == "E" || x.Status == "R")).FirstOrDefault();

            if (user == null)
            {
                result.Code = (int)UserLoginingResponseCode.UserNotFound;
                result.IsSuccess = false;
                result.Msg = "The user \"" + resetPassword.UserEmail + "\" does not exist or is being invited.";
            }
            else
            {
                DateTime dt = DateTime.UtcNow.AddHours(8);
                dt.GetDateTimeFormats('r')[0].ToString();
                Convert.ToDateTime(dt);
                
                user.Status = "R";
                user.UpdateUserID = user.UserID;
                user.UpdateDate = dt;
                user.RanCode = CreateRandomString();
                SellerPortalDB.Entry(user).State = EntityState.Modified;
                SellerPortalDB.SaveChanges();

                Models.Connector connector = new Models.Connector();
                Models.ActionResponse<Models.MailResult> mailResult = new Models.ActionResponse<Models.MailResult>();

                Models.Mail mail = new Models.Mail();
                mail.MailType = Models.Mail.MailTypeEnum.ResetPassword;
                mail.UserEmail = user.UserEmail;
                mailResult = connector.SendMail(null, null, mail);

                result.Code = (int)UserLoginingResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Password Reset Successfully";
                result.Body = new Models.ResetPasswordResult();

                result.Body.UserID = user.UserID;
                result.Body.SendTime = mailResult.Body.SendTime;
                result.Body.MailSubject = mailResult.Body.MailSubject;
                result.Body.MailContent = mailResult.Body.MailContent;
            }
            return result;
        }

        /// <summary>
        /// 更改使用者狀態
        /// </summary>
        /// <param name="userChangeStatus"></param>
        /// <returns></returns>
        public Models.ActionResponse<TWNewEgg.API.Models.UserCheckStatusResult> ChangeUserStatus(TWNewEgg.API.Models.UserChangeStatus userChangeStatus)
        {
            Models.ActionResponse<Models.UserCheckStatusResult> result = new Models.ActionResponse<Models.UserCheckStatusResult>();

            DB.TWSellerPortalDBContext SellerPortalDB = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = SellerPortalDB.Seller_User.Where(x => x.UserEmail == userChangeStatus.UserEmail).FirstOrDefault();

            if (user == null)
            {
                result.Code = (int)UserLoginingResponseCode.UserNotFound;
                result.IsSuccess = false;
                result.Msg = "The user \"" + userChangeStatus.UserEmail + "\" does not exist.";
            }
            else
            {
                DateTime dt = DateTime.UtcNow.AddHours(8);
                dt.GetDateTimeFormats('r')[0].ToString();
                Convert.ToDateTime(dt);

                user.UpdateUserID = userChangeStatus.UpdateUserID;
                user.UpdateDate = dt;
                user.Status = userChangeStatus.Status;
                SellerPortalDB.Entry(user).State = EntityState.Modified;
                SellerPortalDB.SaveChanges();

                result.Code = (int)UserLoginingResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Status Changed Successfully";
                result.Body = new Models.UserCheckStatusResult();

                result.Body.Status = user.Status;
                result.Body.UserID = user.UserID;
            }

            return result;
        }
        /// <summary>
        /// 檢查帳號跟Vendor and Seller 是否正確
        /// </summary>
        /// <param name="_strEmail"></param>
        /// <param name="_strVenSeller"></param>
        /// <returns></returns>
        public bool VendoeSellerCheck(string _strEmail, string _strVenSeller)
        {
            TWNewEgg.DB.TWSellerPortalDBContext sellerDB = new DB.TWSellerPortalDBContext();
            var _intSellerID = sellerDB.Seller_User.Where(p => p.UserEmail == _strEmail).Select(p => p.SellerID).FirstOrDefault();
            
            if (_intSellerID == -1)
            {
                var checkMinus1 = sellerDB.Seller_BasicInfo.Where(p => p.SellerEmail == _strEmail && p.AccountTypeCode == _strVenSeller).FirstOrDefault();
                if (checkMinus1 == null)
                {
                    return false;
                }
            }
            else
            {
                var _strAccountTypeCode = sellerDB.Seller_BasicInfo.Where(p => p.SellerID == _intSellerID && p.AccountTypeCode == _strVenSeller).FirstOrDefault();
                if (_strAccountTypeCode == null)
                {
                    return false;
                }
            }
            return true;
        }
        private string CreateRandomString()
        {
            int rand;
            char word;
            string randomString = String.Empty;
            // 生成啟用連結路徑中的亂數
            System.Random random = new Random();
            for (int i = 0; i < 30; i++)
            {
                rand = random.Next();
                if (rand % 3 == 0)
                {
                    word = (char)('A' + (char)(rand % 26));
                }
                else if (rand % 3 == 1)
                {
                    word = (char)('a' + (char)(rand % 26));
                }
                else
                {
                    word = (char)('0' + (char)(rand % 10));
                }

                randomString += word.ToString();
            }

            return randomString;
        }

        /// <summary>
        /// 供 Filter 使用，利用UserEmail、AccessToken 取得 Seller 基本資料進行比對
        /// </summary>
        /// <param name="UserEmail">Login UserEmail</param>
        /// <param name="AccessToken">存取的 Token</param>
        /// <returns></returns>
        public int GetSellerUserID(string UserEmail, string AccessToken)
        {
            int sellerUserid = 0;

            DB.TWSellerPortalDBContext sellerDB = new DB.TWSellerPortalDBContext();
            string _strDecodeEmail = AesEncryptor.AesDecrypt(UserEmail);
            var _checkLoginStatus = sellerDB.Seller_User.Where(p => p.AccessToken == AccessToken && p.UserEmail == _strDecodeEmail).FirstOrDefault();
            if (_checkLoginStatus != null)
            {
                if (_checkLoginStatus.SellerID == -1)
                {
                    //result.Body.SellerID = Convert.ToString(Encryption(Convert.ToString(sellerDB.Seller_BasicInfo.Where(p => p.SellerEmail == DataInsertToReturn.UserEmail).Select(p => p.SellerID).FirstOrDefault())));
                    sellerUserid = sellerDB.Seller_BasicInfo.Where(p => p.SellerEmail == _strDecodeEmail).Select(p => p.SellerID).FirstOrDefault();
                    return sellerUserid;
                }
                else
                {
                    sellerUserid = Convert.ToInt32(_checkLoginStatus.SellerID);
                    return sellerUserid;
                }
            }
            return sellerUserid;
        }

    }
}
