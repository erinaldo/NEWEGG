using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// 權限相關服務
    /// </summary>
    public class PurviewService
    {
        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

        /// <summary>
        /// 取得Seller的PurviewCount
        /// </summary>
        /// <param name="seller">seller</param>
        /// <returns>Models.ActionResponse</returns>
        public Models.ActionResponse<int> GetSeller_PurviewCount(string seller)
        {
            Models.ActionResponse<int> purviewCount = new Models.ActionResponse<int>();

            int id = -1;
            int.TryParse(seller, out id);
            purviewCount.Body = db.Seller_Purview.Where(r => r.SellerID == id).Count();

            // (from slr in db.Seller_BasicInfo
            //  join slrPrv in db.Seller_Purview on slr.SellerID equals slrPrv.SellerID
            //  where slrPrv.SellerID == id
            //  select slrPrv.SN).Count();
            return purviewCount;
        }

        /// <summary>
        /// 取得User的PurviewCount
        /// </summary>
        /// <param name="userID">userID</param>
        /// <returns>Models.ActionResponse</returns>
        public Models.ActionResponse<int> GetUser_PurviewCount(int userID)
        {
            Models.ActionResponse<int> purviewCount = new Models.ActionResponse<int>();
            purviewCount.Body = db.User_Purview.Where(r => r.UserID == userID).Count();

            // (from usr in db.Seller_User
            //  join usrPrv in db.User_Purview on usr.SellerID equals usrPrv.UserID
            //  where usrPrv.UserID == userID
            //  select usrPrv.SN).Count();
            return purviewCount;
        }

        #region Written by Jack Lin

        /// <summary>
        /// API ActionResponse Code
        /// </summary>
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        /// <summary>
        /// 取得用戶權限
        /// </summary>
        /// <param name="userID">userID</param>
        /// <returns>Models.ActionResponse</returns>
        public Models.ActionResponse<List<Models.GetPurviewResult>> GetUserPurview(int userID, string accounttypecode)
        {
            Models.ActionResponse<List<Models.GetPurviewResult>> apiResult = new Models.ActionResponse<List<Models.GetPurviewResult>>();

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            var userQuery = spdb.Seller_User.AsEnumerable();
            userQuery = userQuery.Where(x => x.UserID == userID);
            DB.TWSELLERPORTALDB.Models.Seller_User user = userQuery.SingleOrDefault();
            DB.TWSELLERPORTALDB.Models.Seller_BasicInfo _sellerBasicSellerIdMinusOne = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
            

            if (user.SellerID == -1)
            {
                _sellerBasicSellerIdMinusOne = spdb.Seller_BasicInfo.Where(p => p.SellerEmail == user.UserEmail && p.AccountTypeCode == accounttypecode).FirstOrDefault();
                user.SellerID = _sellerBasicSellerIdMinusOne.SellerID;
            }

            List<Models.GetPurviewResult> result = new List<Models.GetPurviewResult>();
            Models.GetPurviewResult temp = new Models.GetPurviewResult();

            var tempgroupPurviewQuery = spdb.Group_Purview.AsEnumerable();
            List<DB.TWSELLERPORTALDB.Models.Group_Purview> tempgroupPurviewList = new List<DB.TWSELLERPORTALDB.Models.Group_Purview>();
            string NullReplaceBool = "N";

            //若UserID = 7 則可能為Vender 或者 Seller 使用groupID = 1 or 8來組帳戶設定畫面

            if (user.GroupID == 7)
            {
                if (accounttypecode == "V")
                {
                    user.GroupID = 8;
                }
                else if (accounttypecode == "S")
                {
                    user.GroupID = 1;
                }
            }
            switch (user.PurviewType)
            {
                //權限參照 user purview
                case "U":
                    var userPurviewQuery = spdb.User_Purview.AsEnumerable();
                    userPurviewQuery = userPurviewQuery.Where(x => x.UserID == userID);
                    List<DB.TWSELLERPORTALDB.Models.User_Purview> userPurviewList = new List<DB.TWSELLERPORTALDB.Models.User_Purview>();
                    userPurviewList = userPurviewQuery.ToList();

                    //與GropID找出所屬於的GroupPurview                   
                    tempgroupPurviewQuery = tempgroupPurviewQuery.Where(x => x.GroupID == user.GroupID && x.Enable == "Y");
                    tempgroupPurviewList = tempgroupPurviewQuery.ToList();

                    //若userPurviewLsit 為空 則以"Y"補齊其權限
                    if (userPurviewList.Count == 0)
                    {
                        NullReplaceBool = "Y";
                    }

                    //與其屬於的GroupPurview left join 
                    var userCurrectPurview = (from a in tempgroupPurviewList
                                              join o in userPurviewList
                                             on a.FunctionID equals o.FunctionID
                                             into ps
                                              from item in ps.DefaultIfEmpty()
                                              select new
                                              {
                                                  FunctionID = a.FunctionID,
                                                  Enable = (item != null ? item.Enable : NullReplaceBool)
                                              }).ToList();

                    for (int i = 0; i < userCurrectPurview.Count(); i++)
                    {
                        temp = new Models.GetPurviewResult();
                        temp.FunctionID = userCurrectPurview[i].FunctionID;
                        temp.Enable = userCurrectPurview[i].Enable;
                        temp.PurviewType = "U";
                        temp.UserID = userID;
                        temp.UserEmail = user.UserEmail;

                        result.Add(temp);
                    }
                    break;

                //權限參照 seller purview
                case "S":
                    var sellerPurviewQuery = spdb.Seller_Purview.AsEnumerable();
                    sellerPurviewQuery = sellerPurviewQuery.Where(x => x.SellerID == user.SellerID);
                    List<DB.TWSELLERPORTALDB.Models.Seller_Purview> sellerPurviewList = new List<DB.TWSELLERPORTALDB.Models.Seller_Purview>();
                    sellerPurviewList = sellerPurviewQuery.ToList();


                    //與GropID找出所屬於的GroupPurview                   
                    tempgroupPurviewQuery = tempgroupPurviewQuery.Where(x => x.GroupID == user.GroupID && x.Enable == "Y");
                    tempgroupPurviewList = tempgroupPurviewQuery.ToList();

                    //若sellerPurviewList 為空 則以"Y"補齊其權限
                    if (sellerPurviewList.Count == 0)
                    {
                        NullReplaceBool = "Y";
                    }
                    //與其屬於的GroupPurview left join 
                    var sellerCurrectPurview = (from a in tempgroupPurviewList
                                                join o in sellerPurviewList
                                               on a.FunctionID equals o.FunctionID
                                               into ps
                                                from item in ps.DefaultIfEmpty()
                                                select new
                                                {
                                                    FunctionID = a.FunctionID,
                                                    Enable = (item != null ? item.Enable : NullReplaceBool)
                                                }).ToList();
                    if (sellerCurrectPurview.Count() > 0)
                    {
                        for (int i = 0; i < sellerCurrectPurview.Count(); i++)
                        {
                            temp = new Models.GetPurviewResult();
                            temp.FunctionID = sellerCurrectPurview[i].FunctionID;
                            temp.Enable = sellerCurrectPurview[i].Enable;
                            temp.PurviewType = "S";
                            temp.UserID = userID;
                            temp.UserEmail = user.UserEmail;

                            result.Add(temp);
                        }
                    }
                    else //沒有 Seller 權限時抓取 group purview
                    {
                        var sellerGroupPurviewQuery = spdb.Group_Purview.AsEnumerable();
                        sellerGroupPurviewQuery = sellerGroupPurviewQuery.Where(x => x.GroupID == user.GroupID);
                        List<DB.TWSELLERPORTALDB.Models.Group_Purview> sellerGroupPurviewList = new List<DB.TWSELLERPORTALDB.Models.Group_Purview>();
                        sellerGroupPurviewList = sellerGroupPurviewQuery.ToList();

                        for (int i = 0; i < sellerGroupPurviewList.Count(); i++)
                        {
                            temp = new Models.GetPurviewResult();
                            temp.FunctionID = sellerGroupPurviewList[i].FunctionID;
                            temp.Enable = sellerGroupPurviewList[i].Enable;
                            temp.PurviewType = "G";
                            temp.UserID = userID;
                            temp.UserEmail = user.UserEmail;

                            result.Add(temp);
                        }
                    }
                    break;

                //權限參照 group purview
                case "G":
                    var groupPurviewQuery = spdb.Group_Purview.AsEnumerable();
                    groupPurviewQuery = groupPurviewQuery.Where(x => x.GroupID == user.GroupID);
                    List<DB.TWSELLERPORTALDB.Models.Group_Purview> groupPurviewList = new List<DB.TWSELLERPORTALDB.Models.Group_Purview>();
                    groupPurviewList = groupPurviewQuery.ToList();

                    for (int i = 0; i < groupPurviewList.Count(); i++)
                    {
                        temp = new Models.GetPurviewResult();
                        temp.FunctionID = groupPurviewList[i].FunctionID;
                        temp.Enable = groupPurviewList[i].Enable;
                        temp.PurviewType = "G";
                        temp.UserID = userID;
                        temp.UserEmail = user.UserEmail;

                        result.Add(temp);
                    }
                    break;

                default:
                    apiResult.Code = (int)ResponseCode.Error;
                    apiResult.IsSuccess = false;
                    apiResult.Msg = "讀取失敗，不支援的PurviewType";
                    apiResult.Body = null;
                    return apiResult;
            }

            apiResult.Code = (int)ResponseCode.Success;
            apiResult.IsSuccess = true;
            apiResult.Msg = "讀取成功";
            apiResult.Body = result;

            return apiResult;
        }

        /// <summary>
        /// 儲存用戶權限
        /// </summary>
        /// <param name="userPurview"></param>
        /// <returns></returns>
        /// <returns>Models.ActionResponse</returns>
        public Models.ActionResponse<string> SaveUserPurview(Models.SaveUserPurview userPurview)
        {
            Models.ActionResponse<string> apiResult = new Models.ActionResponse<string>();

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.User_Purview dbUserPurview = new DB.TWSELLERPORTALDB.Models.User_Purview();

            //檢查資料是否合法
            int error = 0;
            int errorItem = 0;
            for (int i = 0; i < userPurview.PurviewList.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(userPurview.PurviewList[i].Enable))
                {
                    error = 1;
                    errorItem = userPurview.PurviewList[i].FunctionID;
                    break;
                }
                else
                    userPurview.PurviewList[i].Enable = userPurview.PurviewList[i].Enable.Trim();
            }

            if (error != 0)
            {
                apiResult.Code = (int)ResponseCode.Error;
                apiResult.IsSuccess = false;
                apiResult.Msg = "Commission rate field has " + error + "error(s).";
                apiResult.Body = errorItem.ToString();
            }
            else
            {
                var query = spdb.User_Purview.AsEnumerable();
                query = query.Where(x => x.UserID == userPurview.UserID);
                List<DB.TWSELLERPORTALDB.Models.User_Purview> userPurviewList = new List<DB.TWSELLERPORTALDB.Models.User_Purview>();
                userPurviewList = query.ToList();

                for (int i = 0; i < userPurview.PurviewList.Count(); i++)
                {
                    dbUserPurview = new DB.TWSELLERPORTALDB.Models.User_Purview();

                    var userPurviewQuery = (from list in userPurviewList
                                            where list.FunctionID == userPurview.PurviewList[i].FunctionID
                                            select list).SingleOrDefault();

                    if (userPurviewQuery == null) //先前未設定權限
                    {
                        dbUserPurview.UserID = userPurview.UserID;
                        dbUserPurview.FunctionID = userPurview.PurviewList[i].FunctionID;
                        dbUserPurview.Enable = userPurview.PurviewList[i].Enable;

                        dbUserPurview.InDate = DateTime.UtcNow.AddHours(8);
                        dbUserPurview.InUserID = userPurview.UpdateUserID;
                        dbUserPurview.UpdateDate = DateTime.UtcNow.AddHours(8);
                        dbUserPurview.UpdateUserID = userPurview.UpdateUserID;

                        spdb.User_Purview.Add(dbUserPurview);
                        spdb.SaveChanges();
                    }
                    else //舊有權限資料存在於資料庫
                    {
                        userPurviewQuery.Enable = userPurview.PurviewList[i].Enable;
                        userPurviewQuery.UpdateDate = DateTime.UtcNow.AddHours(8);
                        userPurviewQuery.UpdateUserID = userPurview.UpdateUserID;

                        spdb.Entry(userPurviewQuery).State = EntityState.Modified;
                        spdb.SaveChanges();
                    }
                }

                var userQuery = spdb.Seller_User.AsEnumerable();
                userQuery = userQuery.Where(x => x.UserID == userPurview.UserID);
                DB.TWSELLERPORTALDB.Models.Seller_User user = userQuery.SingleOrDefault();

                user.PurviewType = "U";
                user.UpdateUserID = userPurview.UpdateUserID;
                user.UpdateDate = DateTime.UtcNow.AddHours(8);
                spdb.Entry(user).State = EntityState.Modified;
                spdb.SaveChanges();

                apiResult.Code = (int)ResponseCode.Success;
                apiResult.IsSuccess = true;
                apiResult.Msg = "寫入成功";
                apiResult.Body = "";
            }

            return apiResult;
        }

        /// <summary>
        /// 取得Seller權限
        /// </summary>
        /// <param name="sellerID"></param>
        /// <returns>Models.ActionResponse</returns>
        public Models.ActionResponse<List<Models.GetPurviewResult>> GetSellerPurview(int sellerID)
        {
            Models.ActionResponse<List<Models.GetPurviewResult>> apiResult = new Models.ActionResponse<List<Models.GetPurviewResult>>();

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            var query = spdb.Seller_Purview.AsEnumerable();
            query = query.Where(x => x.SellerID == sellerID);
            List<DB.TWSELLERPORTALDB.Models.Seller_Purview> sellerPurviewList = new List<DB.TWSELLERPORTALDB.Models.Seller_Purview>();
            sellerPurviewList = query.ToList();

            List<Models.GetPurviewResult> result = new List<Models.GetPurviewResult>();
            Models.GetPurviewResult temp = new Models.GetPurviewResult();

            for (int i = 0; i < sellerPurviewList.Count(); i++)
            {
                temp = new Models.GetPurviewResult();
                temp.FunctionID = sellerPurviewList[i].FunctionID;
                temp.Enable = sellerPurviewList[i].Enable;
                temp.PurviewType = "S";

                result.Add(temp);
            }
            
            apiResult.Code = (int)ResponseCode.Success;
            apiResult.IsSuccess = true;
            apiResult.Msg = "讀取成功";
            apiResult.Body = result;

            return apiResult;
        }

        /// <summary>
        /// 取得Group權限
        /// <param name="groupID"></param>
        /// <returns>Models.ActionResponse</returns>
        public Models.ActionResponse<List<Models.GetPurviewResult>> GetGroupPurview(int groupID)
        {
            Models.ActionResponse<List<Models.GetPurviewResult>> apiResult = new Models.ActionResponse<List<Models.GetPurviewResult>>();

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            var query = spdb.Group_Purview.AsEnumerable();
            query = query.Where(x => x.GroupID == groupID);
            List<DB.TWSELLERPORTALDB.Models.Group_Purview> sellerPurviewList = new List<DB.TWSELLERPORTALDB.Models.Group_Purview>();
            sellerPurviewList = query.ToList();

            List<Models.GetPurviewResult> result = new List<Models.GetPurviewResult>();
            Models.GetPurviewResult temp = new Models.GetPurviewResult();

            for (int i = 0; i < sellerPurviewList.Count(); i++)
            {
                temp = new Models.GetPurviewResult();
                temp.FunctionID = sellerPurviewList[i].FunctionID;
                temp.Enable = sellerPurviewList[i].Enable;
                temp.PurviewType = "G";

                result.Add(temp);
            }
            
            apiResult.Code = (int)ResponseCode.Success;
            apiResult.IsSuccess = true;
            apiResult.Msg = "讀取成功";
            apiResult.Body = result;

            return apiResult;
        }

        /// <summary>
        /// 取得user列表
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>Models.ActionResponse</returns>
        public Models.ActionResponse<List<Models.GetUserListResult>> GetUserList(int sellerID)
        {
            Models.ActionResponse<List<Models.GetUserListResult>> apiResult = new Models.ActionResponse<List<Models.GetUserListResult>>();

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            List<Models.GetUserListResult> result = new List<Models.GetUserListResult>();
            Models.GetUserListResult temp;

            var query = spdb.Seller_User.AsEnumerable();
            query = query.Where(x => x.SellerID == sellerID && (x.GroupID == 1 || x.GroupID == 2)).OrderBy(x => x.UserID);
            List<DB.TWSELLERPORTALDB.Models.Seller_User> userList = new List<DB.TWSELLERPORTALDB.Models.Seller_User>();
            userList = query.ToList();

            for (int i = 0; i < userList.Count(); i++)
            {
                temp = new Models.GetUserListResult();
                temp.UserID = userList[i].UserID;
                temp.UserEmail = userList[i].UserEmail;
                temp.PurviewType = userList[i].PurviewType;
                temp.Status = userList[i].Status;
                temp.GroupID = userList[i].GroupID;

                result.Add(temp);
            }

            //檢查有無存在管理者，若無則用Email搜尋 2014.6.13
            if (!(result.Any(x => x.GroupID == 1)))
            {
                string sellerEmail;
                var sellerQuery = spdb.Seller_BasicInfo.AsEnumerable();
                sellerQuery = sellerQuery.Where(x => x.SellerID == sellerID);

                if (sellerQuery.Any())
                {
                    sellerEmail = sellerQuery.SingleOrDefault().SellerEmail;

                    var sellerUserQuery = spdb.Seller_User.AsEnumerable();
                    sellerUserQuery = sellerUserQuery.Where(x => x.UserEmail == sellerEmail);
                    DB.TWSELLERPORTALDB.Models.Seller_User sellerUserList = sellerUserQuery.SingleOrDefault();

                    if (sellerUserList != null)
                    {
                        temp = new Models.GetUserListResult();
                        temp.UserID = sellerUserList.UserID;
                        temp.UserEmail = sellerUserList.UserEmail;
                        temp.PurviewType = sellerUserList.PurviewType;
                        temp.Status = sellerUserList.Status;
                        temp.GroupID = sellerUserList.GroupID;

                        result.Add(temp);
                    }
                }
            }

            apiResult.Code = (int)ResponseCode.Success;
            apiResult.IsSuccess = true;
            apiResult.Msg = "讀取成功";
            apiResult.Body = result;

            return apiResult;
        }

        /// <summary>
        /// 取得Function列表
        /// </summary>
        /// <returns>Models.ActionResponse</returns>
        public Models.ActionResponse<List<Models.GetFunctionListResult>> GetFunctionList()
        {
            Models.ActionResponse<List<Models.GetFunctionListResult>> apiResult = new Models.ActionResponse<List<Models.GetFunctionListResult>>();

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            //var query = spdb.EDI_Seller_Function.AsEnumerable();
            //query = query.Where(x => x.Status == "E").OrderBy(x => x.CategoryID).ThenBy(x => x.FunctionID);
            //List<DB.TWSELLERPORTALDB.Models.EDI_Seller_Function> functionList = new List<DB.TWSELLERPORTALDB.Models.EDI_Seller_Function>();
            //functionList = query.ToList();
            var functionList2 = (from p in spdb.EDI_Seller_Function
                                join q in spdb.EDI_Seller_FunctionCategory_LocalizedRes
                                    on p.CategoryID equals q.ReferenceCategoryID
                                where p.IsShowOnDesktop == "Y" && q.LanguageCode == "zh-TW"
                                select new 
                                {
                                    CategoryID = p.CategoryID,
                                    FunctionID = p.FunctionID,
                                    FunctionName = p.FunctionName,
                                    CategoryName = q.CategoryName
                                }).ToList();
            var functionList = (from p in functionList2
                                 join q in spdb.EDI_Seller_Function_LocalizedRes on p.FunctionID equals q.ReferenceFunctionID
                                 where q.LanguageCode == "zh-TW"
                                 select new
                                 {
                                     CategoryID = p.CategoryID,
                                     FunctionID = p.FunctionID,
                                     FunctionName = p.FunctionName,
                                     CategoryName = p.CategoryName,
                                     FunctionNameTW = q.FunctionName
                                 }).ToList();
            //var functionList1 = (from f in spdb.EDI_Seller_Function
            //                    join c in spdb.EDI_Seller_FunctionCategory on f.CategoryID equals c.CategoryID
            //                    where f.IsShowOnDesktop == "Y"
            //                    select new { CategoryID = f.CategoryID, CategoryName = c.CategoryName, FunctionID = f.FunctionID, FunctionName = f.FunctionName })
            //             .OrderBy(x => x.CategoryID).ThenBy(x => x.FunctionID).ToList();

            //var functionList = (from f in spdb.EDI_Seller_Function
            //             join c in spdb.EDI_Seller_FunctionCategory on f.CategoryID equals c.CategoryID
            //             where f.IsShowOnDesktop == "Y"
            //             select new { CategoryID = f.CategoryID, CategoryName = c.CategoryName, FunctionID = f.FunctionID, FunctionName = f.FunctionName })
            //             .OrderBy(x=> x.CategoryID).ThenBy(x=>x.FunctionID).ToList();

            List<Models.GetFunctionListResult> result = new List<Models.GetFunctionListResult>();
            Models.GetFunctionListResult temp;

            for (int i = 0; i < functionList.Count(); i++)
            {
                //暫時忽略 Account Management 2014.6.13
                if (functionList[i].CategoryID == 27)
                {
                    continue;
                }

                temp = new Models.GetFunctionListResult();
                temp.CategotyID = functionList[i].CategoryID;
                temp.CategotyName = functionList[i].CategoryName;
                temp.FunctionID = functionList[i].FunctionID;
                temp.FunctionName = functionList[i].FunctionNameTW + ";" + functionList[i].FunctionName;
                
                result.Add(temp);
            }


            apiResult.Code = (int)ResponseCode.Success;
            apiResult.IsSuccess = true;
            apiResult.Msg = "讀取成功";
            apiResult.Body = result;

            return apiResult;
        }

        #endregion
    }
}
