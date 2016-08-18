using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CategoryServices.Interface;
using TWNewEgg.CategoryRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Message;
using System.IO;
using TWNewEgg.ItemRepoAdapters.Interface;

namespace TWNewEgg.CategoryServices
{
    public class CategoryNewServices : ICategoryServices
    {
        private ICategoryRepoAdapter _categorySerAdp;
        private IItemRepoAdapter _adItemSerAdp;
        public CategoryNewServices(ICategoryRepoAdapter categorySerAdp, IItemRepoAdapter adItemSerAdp)
        {
            this._categorySerAdp = categorySerAdp;
            this._adItemSerAdp = adItemSerAdp;
        }
        public ResponseMessage<List<CategoryDM>> GetCategoryByParentID(int parentId)
        {
            ResponseMessage<List<CategoryDM>> Result = new ResponseMessage<List<CategoryDM>>();
            try
            {
                //選告輸出
                List<Category> Category = _categorySerAdp.LoadCategoryChildId(parentId).Where(x => x.ShowAll == 1).ToList();

                if (Category != null)
                {
                    List<CategoryDM> CategoryData = ModelConverter.ConvertTo<List<CategoryDM>>(Category);
                    Result.Data = CategoryData;
                    Result.IsSuccess = true;
                    Result.Message = "";

                    return Result;
                }
                else
                {
                    Result.Data = null;
                    Result.IsSuccess = false;
                    Result.Message = "查無categoryID:" + parentId + "的子類別!!";
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseMessage<CategoryDM> UpdateCategoryByCategoryID(CategoryDM NewData)
        {
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            string strfileName = System.Configuration.ConfigurationManager.AppSettings["HttpCategoryImagePath"];

            CategoryDM ReData = new CategoryDM();
            TWNewEgg.Models.DBModels.TWSQLDB.Category Info = new TWNewEgg.Models.DBModels.TWSQLDB.Category();
            TWNewEgg.Models.DBModels.TWSQLDB.Category ReInfo = new TWNewEgg.Models.DBModels.TWSQLDB.Category();
            TWNewEgg.Models.DBModels.TWSQLDB.Category ParentInfo = new TWNewEgg.Models.DBModels.TWSQLDB.Category();
            ResponseMessage<CategoryDM> Result = new ResponseMessage<CategoryDM>();

            Info = _categorySerAdp.LoadCategoryData(NewData.ID);
            ParentInfo = _categorySerAdp.LoadCategoryData(Info.ParentID);

            if (Info.UpdateDate != null)
            {
                //修改清單中更新時間小於DB更新時間，表示有人更新，不可修改此筆資料
                if (NewData.UpdateDate < Info.UpdateDate)
                {

                    Result.IsSuccess = false;
                    Result.Message = "CategoryID：" + NewData.ID + "，此筆資料，" + Info.UpdateUser + "此人於" + Info.UpdateDate + "同時更新，無法修改，請重新進入修改畫面!\r\n";
                    return Result;
                }
            }

            if (Info == null)
            {
                Result.Data = null;
                Result.IsSuccess = false;
                Result.Message = string.Format("查無categoryID:{0}{1}，無法更新!!", NewData.ID, NewData.Description);
            }
            else
            {
                ModelConverter.ConvertTo<CategoryDM, Category>(NewData, Info);

                if (NewData.ImagePath != null)
                {
                    if (NewData.ImagePath == " ")
                    {
                        Info.ImagePath = " ";
                    }
                    else
                    {
                        //圖檔更改檔名存置資料庫
                        //Info.ImagePath = strfileName + ParentInfo.Description + "/" + Info.ID + ".jpg";
                        Info.ImagePath = string.Format("{0}{1}/{2}{3}", strfileName, ParentInfo.ID, Info.ID, Path.GetExtension(NewData.ImagePath));  //副檔名改為user傳進的副檔名
                   }
                }

                Info.UpdateDate = dt;

                ReInfo = _categorySerAdp.UpdateCategory(Info);
                ReData = ModelConverter.ConvertTo<CategoryDM>(ReInfo);

                Result.Data = ReData;
                Result.IsSuccess = true;
                Result.Message = string.Format("categoryID:{0}{1}，更新成功!!", NewData.ID, NewData.Description);
            }

            return Result;
        }

        public ResponseMessage<List<CategoryDM>> UpdateCategoryByCategoryIDList(List<CategoryDM> NewDataList)
        {
            ResponseMessage<List<CategoryDM>> Result = new ResponseMessage<List<CategoryDM>>();

            foreach (CategoryDM NewData in NewDataList)
            {
                ResponseMessage<CategoryDM> ReData = new ResponseMessage<CategoryDM>();

                ReData = UpdateCategoryByCategoryID(NewData);

                Result.Data.Add(ReData.Data);
                Result.IsSuccess = true;
                Result.Message = Result.Message + ReData.Message + "\r\n";
            }

            return Result;
        }

        public CategoryDM GetCategoryByCategoryID(int Id)
        {
            Category LoadCategory = null;
            CategoryDM Result = null;
            try
            {
                LoadCategory = _categorySerAdp.LoadCategoryData(Id);
                
                if (LoadCategory != null)
                {
                    Result = ModelConverter.ConvertTo<CategoryDM>(LoadCategory);
                }
            }
            catch (Exception ex)
            { 
            throw new NotImplementedException(ex.Message, ex);
            }

            return Result;
        }

        public List<Category_TreeItem> GetAllParentCategoriesByCIDs(List<int> categoryIDs)
        {
            List<Category_TreeItem> results = new List<Category_TreeItem>();
            List<Category> currentCategories = this._categorySerAdp.Category_GetAll().Where(x =>
                categoryIDs.Contains(x.ID)).ToList();

            if (currentCategories.Count == 0)
            {
                return results;
            }

            List<int> parentIDs = currentCategories.Select(x => x.ParentID).ToList();

            List<Category> currentParentCategories = this._categorySerAdp.Category_GetAll().Where(x =>
                parentIDs.Contains(x.ID)).ToList();

            List<Category> currentGrandPaCategories = this._categorySerAdp.Category_GetAll().Where(x =>
                x.ParentID == 0).ToList();

            for (int i = 0; i < currentCategories.Count; i++)
            {
                results.Add(FindParentCategories(currentCategories[i], currentParentCategories, currentGrandPaCategories));
            }

            return results;
        }

        public List<Category_TreeItem> GetAllParentCategoriesByItemIDs(List<int> itemIDs)
        {
            List<int> categoryIDs = this._adItemSerAdp.GetAll().Where(x => itemIDs.Contains(x.ID)).Select(x => x.CategoryID).Distinct().ToList();

            List<Category_TreeItem> results = new List<Category_TreeItem>();

            results = this.GetAllParentCategoriesByCIDs(categoryIDs);

            return results;
        }

        private Category_TreeItem FindParentCategories(Category current, List<Category> currentParent, List<Category> currentGrandPa)
        {
            Category_TreeItem result = ConvertToCategoryTreeItem(current);
            if (result.category_layer == 0)
            {
                return result;
            }
            Category parentCategory = currentParent.Where(x => x.ID == result.category_parentid).FirstOrDefault();
            if (parentCategory == null)
            {
                return result;
            }
            Category_TreeItem parent = ConvertToCategoryTreeItem(parentCategory);

            Category grandPaCategory = currentGrandPa.Where(x => x.ID == parent.category_parentid).FirstOrDefault();
            if (grandPaCategory == null)
            {
                result.Parents = parent;
                return result;
            }
            Category_TreeItem grandPa = ConvertToCategoryTreeItem(grandPaCategory);
            parent.Parents = grandPa;
            result.Parents = parent;

            return result;
        }

        private Category_TreeItem ConvertToCategoryTreeItem(Category category)
        {
            Category_TreeItem result = new Category_TreeItem();
            result.category_categoryfromwsid = category.CategoryfromwsID;
            result.category_createdate = category.CreateDate;
            result.category_createuser = category.CreateUser;
            result.category_description = category.Description;
            result.category_deviceid = category.DeviceID;
            result.category_id = category.ID;
            result.category_layer = category.Layer;
            result.category_parentid = category.ParentID;
            result.category_sellerid = category.SellerID;
            result.category_showall = category.ShowAll;
            result.category_showorder = category.Showorder;
            result.category_title = category.Title;
            result.ClassName = category.ClassName;
            result.ImageHref = category.ImageHref;
            result.ImagePath = category.ImagePath;
            return result;
        }

    }
}
