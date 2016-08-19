using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Service
{
    public class NavigationMenuService
    {
        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();


        public Models.ActionResponse<List<Seller_FunctionJoinCategory>> GetSeller_FuctionBySellerLanguage(string sellerID, string languageCode)
        {
            Models.ActionResponse<List<Seller_FunctionJoinCategory>> Functions = new Models.ActionResponse<List<Seller_FunctionJoinCategory>>();
            try
            {
                int id = -1;
                int.TryParse(sellerID, out id);
                Functions.Body = (from slrPrv in db.Seller_Purview
                                  where slrPrv.SellerID == id
                                  // join act in db.Seller_Action on slrPrv.ActionID equals act.ActionID
                                  // where slrPrv.SellerID == id
                                  join func in db.EDI_Seller_Function on slrPrv.FunctionID equals func.FunctionID
                                  join funcRes in db.EDI_Seller_Function_LocalizedRes on func.FunctionID equals funcRes.ReferenceFunctionID
                                  where funcRes.LanguageCode.ToLower() == languageCode.ToLower()
                                  join fc in db.EDI_Seller_FunctionCategory on func.CategoryID equals fc.CategoryID
                                  join fcRes in db.EDI_Seller_FunctionCategory_LocalizedRes on fc.CategoryID equals fcRes.ReferenceCategoryID
                                  where fcRes.LanguageCode.ToLower() == languageCode.ToLower()
                                  select new Seller_FunctionJoinCategory()
                                  {
                                      FunctionID = func.FunctionID,
                                      FunctionName = funcRes.FunctionName,
                                      Order = func.Order,
                                      IsShowOnDesktop = func.IsShowOnDesktop,
                                      Status = func.Status,
                                      Url = func.Url,
                                      CategoryID = func.CategoryID,
                                      CategoryName = fcRes.CategoryName,
                                      InUserID = func.InUserID,
                                      InUserName = func.InUserName,
                                      Indate = func.Indate,
                                      EditDate = func.EditDate,
                                      EditUserID = func.EditUserID,
                                      EditUserName = func.EditUserName,
                                      LanguageCode = func.LanguageCode,
                                      CompanyCode = func.CompanyCode,
                                      CountryCode = func.CountryCode,
                                      IsRelease = func.IsRelease,
                                      FunctionType = func.FunctionType
                                  }).ToList<Seller_FunctionJoinCategory>();
                Functions.IsSuccess = true;
                Functions.Msg = "OK";
                Functions.Code = 0;

            }
            catch (Exception ex)
            {
                Functions.IsSuccess = false;
                Functions.Msg = ex.Message;
                Functions.Code = 0;
                Functions.Body = null;
            }
            return Functions;
        }


        // post 
        public Models.ActionResponse<List<Seller_FunctionJoinCategory>> GetSeller_FuctionsByQuery(QueryFunctionCondition query)
        {
            Models.ActionResponse<List<Seller_FunctionJoinCategory>> function = new Models.ActionResponse<List<Seller_FunctionJoinCategory>>();

            if (query == null)
            {
                query = new QueryFunctionCondition();
            }

            //if (query.FunctionPointGroupIDs == null)
            //{
            //    query.FunctionPointGroupIDs = new List<int>();
            //}

            bool IsEnglish = true;
            if (!string.IsNullOrEmpty(query.Language))
            {
                IsEnglish = query.Language.ToLower() == "en-us";
            }
            IEnumerable<Seller_FunctionJoinCategory> result = (from fc in db.EDI_Seller_FunctionCategory
                                                               join func in db.EDI_Seller_Function on fc.CategoryID equals func.CategoryID
                                                               where func.IsShowOnDesktop == "Y" && func.Status == "E" && fc.Status == "E"
                                                               //join fpg in db.EDI_Seller_FunctionPointGroup on func.FunctionID equals fpg.FunctionID
                                                               //where query.FunctionPointGroupIDs.Contains(fpg.FunctionPointGroupID)
                                                               select new Seller_FunctionJoinCategory()
                                                               {
                                                                   FunctionID = func.FunctionID,

                                                                   // 如果是英文介面 或 語系表對應語系沒有資料, 就用 db.EDI_Seller_Function.FunctionName ; 否則, 對照語系表查出對應語系
                                                                   FunctionName = IsEnglish || !db.EDI_Seller_Function_LocalizedRes.Where(r => r.LanguageCode.ToLower() == query.Language.ToLower() && r.ReferenceFunctionID == func.FunctionID).Any() ?
                                                                                  func.FunctionName :
                                                                                  (from funcRes in db.EDI_Seller_Function_LocalizedRes
                                                                                   where funcRes.LanguageCode.ToLower() == query.Language.ToLower() && funcRes.ReferenceFunctionID == func.FunctionID
                                                                                   select funcRes.FunctionName).FirstOrDefault(),

                                                                   Order = func.Order,
                                                                   IsShowOnDesktop = func.IsShowOnDesktop,
                                                                   Status = func.Status,
                                                                   Url = func.Url,
                                                                   CategoryID = func.CategoryID,

                                                                   // 如果是英文介面 或 語系表對應語系沒有資料, 就用 db.EDI_Seller_FunctionCategory.CategoryName ; 否則, 對照語系表查出對應語系
                                                                   CategoryName = IsEnglish || !db.EDI_Seller_FunctionCategory_LocalizedRes.Where(r => r.LanguageCode.ToLower() == query.Language.ToLower() && r.ReferenceCategoryID == fc.CategoryID).Any() ?
                                                                                  fc.CategoryName :
                                                                                  (from fcRes in db.EDI_Seller_FunctionCategory_LocalizedRes
                                                                                   where fcRes.LanguageCode.ToLower() == query.Language.ToLower() && fcRes.ReferenceCategoryID == fc.CategoryID
                                                                                   select fcRes.CategoryName).FirstOrDefault(),

                                                                   InUserID = func.InUserID,
                                                                   InUserName = func.InUserName,
                                                                   Indate = func.Indate,
                                                                   EditDate = func.EditDate,
                                                                   EditUserID = func.EditUserID,
                                                                   EditUserName = func.EditUserName,
                                                                   LanguageCode = func.LanguageCode,
                                                                   CompanyCode = func.CompanyCode,
                                                                   CountryCode = func.CountryCode,
                                                                   IsRelease = func.IsRelease,
                                                                   FunctionType = func.FunctionType
                                                               }).Distinct();

            List<int> authFunctions = new List<int>();
            //where usr.GroupID == groupID && grp.Enable == "Y" && grpPrv.Enable == "Y" 
            if (query.SellerID.HasValue)
            {
                authFunctions = db.Seller_Purview.Where(r => r.SellerID == query.SellerID && r.Enable == "Y").Select(r => r.FunctionID).ToList();
            }

            if (query.UserID.HasValue)
            {
                authFunctions = db.User_Purview.Where(r => r.UserID == query.UserID && r.Enable == "Y").Select(r => r.FunctionID).ToList();
            }

            if (query.GroupID.HasValue)
            {
                authFunctions = db.Group_Purview.Where(r => r.GroupID == query.GroupID && r.Enable == "Y").Select(r => r.FunctionID).ToList();
            }

            function.Body = result.Where(r => authFunctions.Contains(r.FunctionID)).ToList();

            return function;
        }


        public Models.ActionResponse<List<Seller_FunctionCategoryLocalized>> GetSeller_FunctionCategoryByLanguage(string language)
        {
            Models.ActionResponse<List<Seller_FunctionCategoryLocalized>> categories = new Models.ActionResponse<List<Seller_FunctionCategoryLocalized>>();

            bool IsEnglish = language.ToLower() == "en-us"; 
            categories.Body = (from cate in db.EDI_Seller_FunctionCategory
                               join cateRes in db.EDI_Seller_FunctionCategory_LocalizedRes on cate.CategoryID equals cateRes.ReferenceCategoryID
                               where cateRes.LanguageCode.ToLower() == language.ToLower() || cate.LanguageCode.ToLower() == language.ToLower()
                               select new Seller_FunctionCategoryLocalized()
                               {
                                   CategoryID = cate.CategoryID,
                                   CategoryName = IsEnglish ? cate.CategoryName : cateRes.CategoryName,
                                   CompanyCode = cate.CompanyCode,
                                   CountryCode = cate.CountryCode,
                                   EditDate = cate.EditDate,
                                   EditUserID = cate.EditUserID,
                                   EditUserName = cate.EditUserName,
                                   IconStyle = cate.IconStyle,
                                   Indate = cate.Indate,
                                   InUserID = cate.InUserID,
                                   InUserName = cate.InUserName,
                                   IsRelease = cate.IsRelease == "Y",
                                   LanguageCode = cate.LanguageCode,
                                   Level = cate.Level,
                                   Order = cate.Order,
                                   ParentCategoryID = cate.ParentCategoryID,
                                   Status = cate.Status
                               }).Distinct().ToList<Seller_FunctionCategoryLocalized>();

            return categories;
        }

    }
}
