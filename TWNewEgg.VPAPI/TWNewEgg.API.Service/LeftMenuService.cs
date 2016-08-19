using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using TWNewEgg.EDI_FunctionCategory_LocalizedResRepoAdapters.Interface;
using TWNewEgg.EDI_Seller_Function_LocalizedResRepoAdapters.Interface;
using TWNewEgg.EDI_Seller_FunctionRepoAdapters.Interface;
using TWNewEgg.Group_PurviewRepoAdapters.Interface;
using TWNewEgg.Seller_ActionRepoAdapters.Interface;
using TWNewEgg.Seller_BasicInfoRepoAdapters.Interface;
using TWNewEgg.Seller_PurviewRepoAdapters.Interface;
using TWNewEgg.Seller_UserRepoAdapters.Interface;
using TWNewEgg.User_PurviewRepoAdapters.Interface;

namespace TWNewEgg.API.Service
{
    public class LeftMenuService
    {

        #region RepoAdapter 宣告
        private ISeller_UserRepoAdapters _iSeller_UserRepoAdapters = TWNewEgg.Framework.Autofac.AutofacConfig.Container.Resolve<ISeller_UserRepoAdapters>();
        private ISeller_BasicInfoRepoAdapters _iSeller_BasicInfoRepoAdapters = TWNewEgg.Framework.Autofac.AutofacConfig.Container.Resolve<ISeller_BasicInfoRepoAdapters>();
        private ISeller_PurviewRepoAdapters _iSeller_PurviewRepoAdapters = TWNewEgg.Framework.Autofac.AutofacConfig.Container.Resolve<ISeller_PurviewRepoAdapters>();
        private IUser_PurviewRepoAdapters _iUser_PurviewRepoAdapters = TWNewEgg.Framework.Autofac.AutofacConfig.Container.Resolve<IUser_PurviewRepoAdapters>();
        private IGroup_PurviewRepoAdapters _iGroup_PurviewRepoAdapters = TWNewEgg.Framework.Autofac.AutofacConfig.Container.Resolve<IGroup_PurviewRepoAdapters>();
        private IEDI_Seller_Function_LocalizedResRepoAdapters _iEDI_Seller_Function_LocalizedResRepoAdapters = TWNewEgg.Framework.Autofac.AutofacConfig.Container.Resolve<IEDI_Seller_Function_LocalizedResRepoAdapters>();
        private IEDI_Seller_FunctionRepoAdapters _iEDI_Seller_FunctionRepoAdapters = TWNewEgg.Framework.Autofac.AutofacConfig.Container.Resolve<IEDI_Seller_FunctionRepoAdapters>();
        private ISeller_ActionRepoAdapters _iSeller_ActionRepoAdapters = TWNewEgg.Framework.Autofac.AutofacConfig.Container.Resolve<ISeller_ActionRepoAdapters>();
        private IEDI_FunctionCategory_LocalizedResRepoAdapters _iEDI_FunctionCategory_LocalizedResRepoAdapters = TWNewEgg.Framework.Autofac.AutofacConfig.Container.Resolve<IEDI_FunctionCategory_LocalizedResRepoAdapters>();
        #endregion
        /// <summary>
        /// 取得該使用者可用功能清單
        /// </summary>
        /// <param name="userLoginResult">使用者登入資訊</param>
        /// <returns>返回功能選單取得結果</returns>
        public Models.ActionResponse<List<MenuList>> GetLeftMenu(Models.UserLoginResult userLoginResult)
        {
            Models.ActionResponse<List<MenuList>> result = new Models.ActionResponse<List<MenuList>>();

            DB.TWSellerPortalDBContext db = new TWSellerPortalDBContext();
            // 找出該使用者的所屬權限的Table
            userLoginResult.PurviewType = db.Seller_User.Where(x => x.UserEmail == userLoginResult.UserEmail).Select(x => x.PurviewType).FirstOrDefault();
            // GroupID = 7 時，在Seller_User的表內 SellerID = -1，需利用 Email 至 Seller_BasicInfo and AccountTypeCode 判斷要拿哪一個 SellerID
            int groupID = 0;
            int.TryParse(userLoginResult.GroupID, out groupID);
            if (groupID == 7)
            {
                userLoginResult.SellerID = db.Seller_BasicInfo.Where(x => x.SellerEmail == userLoginResult.UserEmail && x.AccountTypeCode == userLoginResult.AccountTypeCode).Select(x => x.SellerID).FirstOrDefault().ToString();
            }

            List<int> functionIDSet = getFunctionID(userLoginResult);
            
            result.Body = getAction(functionIDSet);
            if (result.Body.Count != 0)
            {
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = string.Empty;
            }
            else
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Msg = "無法取得該用戶Menu";
            }

            return result;
        }

        /// <summary>
        /// 取得此使用者可使用的 Function ID List
        /// </summary>
        /// <param name="menuPurviewInfo">使用者資訊</param>
        /// <returns>Function ID List</returns>
        private List<int> getFunctionID(Models.UserLoginResult menuPurviewInfo)
        {
            List<int> functionIDSet = new List<int>();

            DB.TWSellerPortalDBContext db = new TWSellerPortalDBContext();
            int purviewUserID = 0;
            int purviewSellerID = 0;
            int.TryParse(menuPurviewInfo.UserID, out purviewUserID);
            int.TryParse(menuPurviewInfo.SellerID, out purviewSellerID);
            if (menuPurviewInfo.PurviewType == "U" && purviewUserID <= 0)
            {
                menuPurviewInfo.PurviewType = "S";
            }

            if (menuPurviewInfo.PurviewType == "S" && purviewSellerID <= 0)
            {
                menuPurviewInfo.PurviewType = "G";
            }

            switch (menuPurviewInfo.PurviewType)
            {
                case "U":
                    {
                        int userID = 0;
                        int.TryParse(menuPurviewInfo.UserID, out userID);
                        functionIDSet = db.User_Purview.Where(x => x.UserID == userID && x.Enable == "Y").Select(x => x.FunctionID).ToList();
                        if (functionIDSet.Count == 0)
                        {
                            menuPurviewInfo.PurviewType = "S";
                            functionIDSet = getFunctionID(menuPurviewInfo);
                        }

                        break;
                    }
                case "S":
                    {
                        int sellerID = 0;
                        int.TryParse(menuPurviewInfo.SellerID, out sellerID);
                        functionIDSet = db.Seller_Purview.Where(x => x.SellerID == sellerID && x.Enable == "Y").Select(x => x.FunctionID).ToList();
                        if (functionIDSet.Count == 0)
                        {
                            menuPurviewInfo.PurviewType = "G";
                            functionIDSet = getFunctionID(menuPurviewInfo);
                        }

                        break;
                    }
                //case "G":
                default:
                    {
                        int groupID = 0;
                        int.TryParse(menuPurviewInfo.GroupID, out groupID);
                        functionIDSet = db.Group_Purview.Where(x => x.GroupID == groupID && x.Enable == "Y").Select(x => x.FunctionID).ToList();
                        break;
                    }
            }

            return functionIDSet;
        }

        /// <summary>
        /// 取得選單名稱及網址位置
        /// </summary>
        /// <param name="functionIDSet">Function ID List</param>
        /// <returns>選單名稱及網址位置</returns>
        private List<MenuList> getAction(List<int> functionIDSet)
        {
            List<MenuList> menuSet = new List<MenuList>();

            DB.TWSellerPortalDBContext db = new TWSellerPortalDBContext();

            List<EDI_Seller_Function_LocalizedRes> eDISellerFunctionLocalizedResList = db.EDI_Seller_Function_LocalizedRes.Where(x => functionIDSet.Contains((int)x.ReferenceFunctionID) && x.LanguageCode == "zh-TW").ToList();
            List<EDI_Seller_Function> eDISellerFunctionList = db.EDI_Seller_Function.Where(x => functionIDSet.Contains(x.FunctionID)).ToList();
            List<Seller_Action> sellerActionList = db.Seller_Action.Where(x => functionIDSet.Contains(x.FunctionID) && x.FNActiveKey == "Menu").ToList();

            foreach (int functionID in functionIDSet)
            {
                string functionName = eDISellerFunctionLocalizedResList.Where(x => x.ReferenceFunctionID == functionID).Select(x => x.FunctionName).FirstOrDefault();

                int categoryID = eDISellerFunctionList.Where(x => x.FunctionID == functionID).Select(x => x.CategoryID).FirstOrDefault();

                string categoryName = db.EDI_Seller_FunctionCategory_LocalizedRes.Where(x => x.ReferenceCategoryID == categoryID && x.LanguageCode == "zh-TW").Select(x => x.CategoryName).FirstOrDefault();

                var actionSet = sellerActionList.Where(x => x.FunctionID == functionID).Select(x => new { x.ControllerName, x.ActionName }).FirstOrDefault();
                if (actionSet != null)
                {
                    string actionUrl = string.Format("/{0}/{1}", actionSet.ControllerName, actionSet.ActionName);

                    if (menuSet.Any(x => x.mainOption == categoryName))
                    {
                        menuSet.First(x => x.mainOption == categoryName).listOptions.Add(functionName, actionUrl);
                    }
                    else
                    {
                        MenuList menu = new MenuList();
                        menu.mainOption = categoryName;
                        menu.listOptions.Add(functionName, actionUrl);
                        menuSet.Add(menu);
                    }
                }
            }

            return menuSet;
        }


        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.MenuList>> GetLeftMenuV2(TWNewEgg.API.Models.UserLoginResult userLoginResult)
        {
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.MenuList>> result = new ActionResponse<List<MenuList>>();

            userLoginResult.PurviewType = this._iSeller_UserRepoAdapters.GetAllSellerUser().Where(p => p.UserEmail == userLoginResult.UserEmail).Select(p => p.PurviewType).FirstOrDefault();
            int groupID = 0;
            int.TryParse(userLoginResult.GroupID, out groupID);
            if (groupID == 7)
            {
                userLoginResult.SellerID = this._iSeller_BasicInfoRepoAdapters.GetAll().Where(x => x.SellerEmail == userLoginResult.UserEmail && x.AccountTypeCode == userLoginResult.AccountTypeCode).Select(x => x.SellerID).FirstOrDefault().ToString();
            }
            List<int> functionIDSet = this.getFunctionIDV2(userLoginResult);
            result.Body = this.getActionV2(functionIDSet);

            if (result.Body.Count != 0)
            {
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = string.Empty;
            }
            else
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Msg = "無法取得該用戶Menu";
            }

            return result;
        }

        private List<int> getFunctionIDV2(Models.UserLoginResult menuPurviewInfo)
        {
            List<int> functionIDSet = new List<int>();

            int purviewUserID = 0;
            int purviewSellerID = 0;
            int.TryParse(menuPurviewInfo.UserID, out purviewUserID);
            int.TryParse(menuPurviewInfo.SellerID, out purviewSellerID);
            if (menuPurviewInfo.PurviewType == "U" && purviewUserID <= 0)
            {
                menuPurviewInfo.PurviewType = "S";
            }

            if (menuPurviewInfo.PurviewType == "S" && purviewSellerID <= 0)
            {
                menuPurviewInfo.PurviewType = "G";
            }

            switch (menuPurviewInfo.PurviewType)
            {
                case "U":
                    {
                        int userID = 0;
                        int.TryParse(menuPurviewInfo.UserID, out userID);
                        functionIDSet = this._iUser_PurviewRepoAdapters.GetAll().Where(x => x.UserID == userID && x.Enable == "Y").Select(x => x.FunctionID).ToList();
                        if (functionIDSet.Count == 0)
                        {
                            menuPurviewInfo.PurviewType = "S";
                            functionIDSet = getFunctionID(menuPurviewInfo);
                        }
                        break;
                    }
                case "S":
                    {
                        int sellerID = 0;
                        int.TryParse(menuPurviewInfo.SellerID, out sellerID);
                        functionIDSet = this._iSeller_PurviewRepoAdapters.GetAll().Where(x => x.SellerID == sellerID && x.Enable == "Y").Select(x => x.FunctionID).ToList();
                        if (functionIDSet.Count == 0)
                        {
                            menuPurviewInfo.PurviewType = "G";
                            functionIDSet = getFunctionID(menuPurviewInfo);
                        }
                        break;
                    }
                default:
                    {
                        int groupID = 0;
                        int.TryParse(menuPurviewInfo.GroupID, out groupID);
                        functionIDSet = this._iGroup_PurviewRepoAdapters.GetAll().Where(x => x.GroupID == groupID && x.Enable == "Y").Select(x => x.FunctionID).ToList();
                        break;
                    }
            }
            return functionIDSet;
        }

        private List<MenuList> getActionV2(List<int> functionIDSet)
        {
            List<MenuList> menuSet = new List<MenuList>();
            List<TWNewEgg.VendorModels.DBModels.Model.EDI_Seller_Function_LocalizedRes> eDISellerFunctionLocalizedResList = this._iEDI_Seller_Function_LocalizedResRepoAdapters.GetAll().Where(x => functionIDSet.Contains((int)x.ReferenceFunctionID) && x.LanguageCode == "zh-TW").ToList();
            List<TWNewEgg.VendorModels.DBModels.Model.EDI_Seller_Function> eDISellerFunctionList = this._iEDI_Seller_FunctionRepoAdapters.GetAll().Where(x => functionIDSet.Contains(x.FunctionID)).ToList();
            List<TWNewEgg.VendorModels.DBModels.Model.Seller_Action> sellerActionList = this._iSeller_ActionRepoAdapters.GetAll().Where(x => functionIDSet.Contains(x.FunctionID) && x.FNActiveKey == "Menu").ToList();

            foreach (int functionID in functionIDSet)
            {
                string functionName = eDISellerFunctionLocalizedResList.Where(x => x.ReferenceFunctionID == functionID).Select(x => x.FunctionName).FirstOrDefault();

                int categoryID = eDISellerFunctionList.Where(x => x.FunctionID == functionID).Select(x => x.CategoryID).FirstOrDefault();

                string categoryName = this._iEDI_FunctionCategory_LocalizedResRepoAdapters.GetAll().Where(x => x.ReferenceCategoryID == categoryID && x.LanguageCode == "zh-TW").Select(x => x.CategoryName).FirstOrDefault();

                var actionSet = sellerActionList.Where(x => x.FunctionID == functionID).Select(x => new { x.ControllerName, x.ActionName }).FirstOrDefault();
                if (actionSet != null)
                {
                    string actionUrl = string.Format("/{0}/{1}", actionSet.ControllerName, actionSet.ActionName);

                    if (menuSet.Any(x => x.mainOption == categoryName))
                    {
                        menuSet.First(x => x.mainOption == categoryName).listOptions.Add(functionName, actionUrl);
                    }
                    else
                    {
                        MenuList menu = new MenuList();
                        menu.mainOption = categoryName;
                        menu.listOptions.Add(functionName, actionUrl);
                        menuSet.Add(menu);
                    }
                }
            }
            
            return menuSet;
        }
    }
}
