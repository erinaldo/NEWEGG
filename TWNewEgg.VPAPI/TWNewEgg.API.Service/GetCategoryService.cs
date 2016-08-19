using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Service
{
    /*---------- add by thisway ----------*/
    /// <summary>
    /// Add Manage Items (APIService)
    /// <para>Website Page:Create Items / Manage Items</para>
    /// </summary>
    public class GetCategoryService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        ///<summary>
        ///Get DB's field 
        ///</summary>
        ///<returns></returns>
        public ActionResponse<List<CategoryResult>> GetProductCategory(CategoryResult ManageItems)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            ActionResponse<List<Models.CategoryResult>> Result = new ActionResponse<List<Models.CategoryResult>>();

            //搜尋DB數個欄位資料，並寫入Model
            Result.Body = new List<CategoryResult>();
            Result.Body.Add(new CategoryResult());

            Result.Body[0].Title = db.Category.Where(x => x.Title != null).Select(x => x.Title).ToList<string>();
            Result.Body[0].ProductDescription = db.Category.Where(x => x.Description != null).Select(x => x.Description).ToList<string>();
            Result.Body[0].Layer = db.Category.Where(x => x.Layer != null).Select(x => x.Layer).ToList<int>();
            Result.Body[0].ParentID = db.Category.Where(x => x.ParentID != null).Select(x => x.ParentID).ToList<int>();
            Result.Body[0].Showorder = db.Category.Where(x => x.Showorder != null).Select(x => x.Showorder).ToList<int>();
            Result.Body[0].ID = db.Category.Where(x => x.ID != null).Select(x => x.ID).ToList<int>();

            //若之後需要檢查創建商品資訊上傳是否有重覆，可修改，來獲取唯一值
            //Result.Body[0].Name = db.Product.Where(x => x.ID != null).Select(x => x.Name).ToList<string>();
            //Result.Body[0].NameTW = db.Product.Where(x => x.ID != null).Select(x => x.NameTW).ToList<string>();


            //顯示DB寫入狀態(成功或失敗)
            if (Result.Body == null)
            {
                Result.Msg = "失敗，無商品類別資料。";
                Result.Code = (int)ResponseCode.Error;
                Result.IsSuccess = false;
            }

            else if (Result.Body[0].Title.Count!=Result.Body[0].ProductDescription.Count &&
                Result.Body[0].ProductDescription.Count !=Result.Body[0].Layer.Count &&
                Result.Body[0].Layer.Count != Result.Body[0].ParentID.Count &&
                Result.Body[0].ParentID.Count != Result.Body[0].Showorder.Count &&
                Result.Body[0].Showorder.Count != Result.Body[0].ID.Count)
            {
                Result.Msg = "失敗，資料長度不一致。";
                Result.Code = (int)ResponseCode.Error;
                Result.IsSuccess = false;
            }

            else
            {
                Result.Msg = "商品類別資料導出成功！";
                Result.Code = (int)ResponseCode.Success;
                Result.IsSuccess = true;
            }

            return Result;
        }    
    }
    /*---------- end by thisway ----------*/
}
