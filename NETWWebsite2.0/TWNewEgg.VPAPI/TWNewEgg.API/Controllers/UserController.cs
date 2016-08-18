using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;

namespace TWNewEgg.API.Controllers
{
    
    public class UserController : Controller
    {
        [Attributes.ActionDescriptionAttribute("登入")]
        [HttpPost]
        public JsonResult Login(Models.UserLogin userLogin)
        {
            Models.ActionResponse<Models.UserLoginResult> result = new Models.ActionResponse<Models.UserLoginResult>();

            Service.UserService UserService = new Service.UserService();
            result = UserService.Login(userLogin);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Attributes.ActionDescriptionAttribute("查詢使用者狀態（邀請頁面起始用）")]
        [HttpPost]
        public JsonResult CheckStatus(Models.UserCheckStatus userCheckStatus)
        {
            Models.ActionResponse<Models.UserCheckStatusResult> result = new Models.ActionResponse<Models.UserCheckStatusResult>();

            Service.UserService UserService = new Service.UserService();
            result = UserService.CheckStatus(userCheckStatus);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Attributes.ActionDescriptionAttribute("查詢使用者存在與否（修改密碼頁面用）")]
        [HttpPost]
        public JsonResult CheckExist(string UserEmail)
        {
            Models.ActionResponse<int> result = new Models.ActionResponse<int>();

            Service.UserService UserService = new Service.UserService();
            result = UserService.CheckExist(UserEmail);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [Attributes.ActionDescriptionAttribute("新帳號啟用設定密碼")]
        [HttpPost]
        public JsonResult SetPassword(Models.UserChangePassword changePassword)
        {
            Models.ActionResponse<Models.UserLoginResult> result = new Models.ActionResponse<Models.UserLoginResult>();

            Service.UserService UserService = new Service.UserService();
            result = UserService.SetPassword(changePassword);
                       
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Attributes.ActionDescriptionAttribute("更改密碼")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult ChangeOldPassword(Models.UserChangePassword changePassword)
        {
            Models.ActionResponse<Models.UserLoginResult> result = new Models.ActionResponse<Models.UserLoginResult>();

            Service.UserService UserService = new Service.UserService();
            result = UserService.ChangeOldPassword(changePassword);
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Attributes.ActionDescriptionAttribute("建立Seller")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult CreateSeller(Models.SellerCreation sellerCreation)
        {
            Service.UserService UserService = new Service.UserService();
            Models.ActionResponse<Models.SellerCreationResult> result = new Models.ActionResponse<Models.SellerCreationResult>();

            #region 呼叫 CreateUser 的部分改在 UserService 執行 by Jack Lin 2014.5.14
            ////?+ 在BasicInfo中建立新資料，傳回SellerID
            //Models.ActionResponse<int> createSellerResult = new Models.ActionResponse<int>();
            //createSellerResult = UserService.CreateSeller(sellerCreation);

            //if (createSellerResult.IsSuccess == false)
            //{
            //    result.IsSuccess = createSellerResult.IsSuccess;
            //    result.Code = createSellerResult.Code;
            //    result.Msg = "建立Seller時發生錯誤：「" + createSellerResult.Msg + "」。";
                
            //    return Json(result, JsonRequestBehavior.AllowGet);
            //} 
            
            ////?+ 建立新User
            //Models.UserCreation userCreation = new Models.UserCreation();
            //userCreation.Email = sellerCreation.SellerEmail;
            //userCreation.SellerID = createSellerResult.Body; //SellerID來自剛剛的CreateSeller
            //userCreation.GroupID = sellerCreation.GroupID;
            //userCreation.InUserID = sellerCreation.InUserID;

            //Models.ActionResponse<Models.UserCreationResult> createUserResult = new Models.ActionResponse<Models.UserCreationResult>();
            //createUserResult = UserService.CreateUser(userCreation);

            //if(createUserResult.IsSuccess == false)
            //{
            //    result.IsSuccess = createUserResult.IsSuccess;
            //    result.Code = createUserResult.Code;
            //    result.Msg = "建立Seller完成，但建立User時發生錯誤：「" + createUserResult.Msg + "」，請執行 CreateUser API 以重新建立User。";
            //    result.Body = new Models.SellerCreationResult();

            //    result.Body.SellerID = createSellerResult.Body;
                
            //    return Json(result, JsonRequestBehavior.AllowGet);
            //}

            ////?+ 將兩個Service的結果合併
            ////Models.ActionResponse<Models.SellerCreationResult> result = new Models.ActionResponse<Models.SellerCreationResult>();
            //result.IsSuccess = true;
            //result.Code = createSellerResult.Code;
            //result.Msg = "建立完成";
            //result.Body = new Models.SellerCreationResult();
            
            //result.Body.SellerID = createSellerResult.Body; //SellerID來自CreateSeller
            //result.Body.UserID = createUserResult.Body.UserID;
            //result.Body.RanCode = createUserResult.Body.RanCode;
            
            #endregion

            using (TransactionScope scope = new TransactionScope())
            {
                result = UserService.CreateSeller(sellerCreation);

                if (result.IsSuccess == true)
                {
                    scope.Complete(); //TransactionScope 結束

                    ////2014.6.27 寫到前台seller的service add by ice begin
                    //Models.ActionResponse<string> massage = new Models.ActionResponse<string>();
                    //Service.TWService twser = new Service.TWService();
                    //massage = twser.UpdateTWSeller(sellerCreation.SellerEmail);
                    //if (massage.IsSuccess == false)
                    //{
                    //    result.Code = massage.Code;
                    //    result.IsSuccess = massage.IsSuccess;
                    //    result.Msg = massage.Msg;
                    //}
                    ////2014.6.27 寫到前台seller的service add by ice end
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Attributes.ActionDescriptionAttribute("建立User")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult CreateUser(Models.UserCreation userCreation)
        {
            Models.ActionResponse<Models.UserCreationResult> result = new Models.ActionResponse<Models.UserCreationResult>();

            Service.UserService UserService = new Service.UserService();

            using (TransactionScope scope = new TransactionScope())
            {
                result = UserService.CreateUser(userCreation);

                if (result.IsSuccess == true)
                {
                    scope.Complete(); //TransactionScope 結束
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 忘記/重設密碼
        /// </summary>
        /// <returns></returns>
        ///
        [Attributes.ActionDescriptionAttribute("忘記/重設密碼")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult ResetPassword(TWNewEgg.API.Models.ResetPassword resetPassword)
        {
            Service.UserService UserService = new Service.UserService();

            //回傳成功與否
            API.Models.ActionResponse<TWNewEgg.API.Models.ResetPasswordResult> apiResult = new API.Models.ActionResponse<TWNewEgg.API.Models.ResetPasswordResult>();
            apiResult = UserService.ResetPassword(resetPassword);

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更改使用者狀態
        /// </summary>
        /// <returns></returns>
        ///
        [Attributes.ActionDescriptionAttribute("更改使用者狀態")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult ChangeUserStatus(TWNewEgg.API.Models.UserChangeStatus userChangeStatus)
        {
            Service.UserService UserService = new Service.UserService();

            //回傳成功與否
            API.Models.ActionResponse<TWNewEgg.API.Models.UserCheckStatusResult> apiResult = new API.Models.ActionResponse<TWNewEgg.API.Models.UserCheckStatusResult>();
            apiResult = UserService.ChangeUserStatus(userChangeStatus);

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetSellerUserID(TWNewEgg.API.Models.Cookie _cookie)
        {
            Service.UserService UserService = new Service.UserService();
            API.Models.ActionResponse<int> result = new Models.ActionResponse<int>();
            int resultCheck = UserService.GetSellerUserID(_cookie.EmailCookie, _cookie.AccessToken);
            result.Body = resultCheck;
            return Json(result, JsonRequestBehavior.AllowGet);
            
        }
        [HttpPost]
        public JsonResult AutoLogin(TWNewEgg.API.Models.UserLogin _userLogin)
        {
            Models.ActionResponse<Models.UserLoginResult> result = new Models.ActionResponse<Models.UserLoginResult>();

            Service.UserService UserService = new Service.UserService();
            result = UserService.AutoLogin(_userLogin);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CreateVendorOrSeller(TWNewEgg.API.Models.SellerCreation sellerInfo, TWNewEgg.API.Models.SaveSellerCharge sellerCharge, int userid)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            Service.UserService userService = new Service.UserService();
            result = userService.CreateVendorOrSeller(sellerInfo, sellerCharge, userid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
    }
}
