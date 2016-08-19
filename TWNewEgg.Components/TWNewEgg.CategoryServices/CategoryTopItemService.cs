using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CategoryServices.Interface;
using TWNewEgg.CategoryRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Message;
using System.IO;


namespace TWNewEgg.CategoryServices
{
    public class CategoryTopItemService : ICategoryTopItemService
    {
        private ICategoryTopItemRepoAdapter _categoryTopItemSerAdp;
        private IAdLayer3Services _adLayer3Ser;
        public CategoryTopItemService(ICategoryTopItemRepoAdapter categoryTopItemSerAdp, IAdLayer3Services adLayer3Ser)
        {
            this._categoryTopItemSerAdp = categoryTopItemSerAdp;
            this._adLayer3Ser = adLayer3Ser;
        }
        public List<CategoryTopItemDM> GetCategoryTopItem(CategoryTopItemDM SearchCondition)
        {
            IQueryable<CategoryTopItem> SearchData = _categoryTopItemSerAdp.CategoryTopItem_GetAll().AsQueryable();

            List<CategoryTopItemDM> ReturnSearchData = new List<CategoryTopItemDM>();

            //類別銷售TOP資料
            if (SearchCondition.CategoryID != null)
            {
                SearchData = SearchData.Where(x => x.CategoryID == SearchCondition.CategoryID);
            }
            if (SearchCondition.ItemID != null)
            {
                SearchData = SearchData.Where(x => x.ItemID == SearchCondition.ItemID);
            }
            if (SearchCondition.ItemType != null)
            {
                SearchData = SearchData.Where(x => x.ItemType == SearchCondition.ItemType);
            }
            if (SearchCondition.ShowAll != null)
            {
                SearchData = SearchData.Where(x => x.ShowAll == SearchCondition.ShowAll);
            }
            if (SearchCondition.Showorder != null)
            {
                SearchData = SearchData.Where(x => x.Showorder == SearchCondition.Showorder);
            }
            if (!string.IsNullOrEmpty(SearchCondition.UpdateUser))
            {
                SearchData = SearchData.Where(x => x.UpdateUser == SearchCondition.UpdateUser);
            }

            if ((SearchData != null) && (SearchData.Count() > 0))
            {
                SearchData = SearchData.OrderBy(x => x.Showorder);
                ReturnSearchData = ModelConverter.ConvertTo<List<CategoryTopItemDM>>(SearchData.ToList());
            }

            return ReturnSearchData;
        }

        private List<CategoryTopItem> UpdateCategoryTopItemDMData(TopItemDM NewData)
        {
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            List<CategoryTopItem> ReData = new List<CategoryTopItem>();

            if (_categoryTopItemSerAdp.CategoryTopItem_GetAll().Where(x => x.CategoryID == NewData.CategoryID).Any())
            {
                _categoryTopItemSerAdp.DeleteAdLayer3Items(NewData.CategoryID.GetValueOrDefault(0));
            }

            foreach (CategoryTopItemDM item in NewData.ItemList)
            {
                item.CategoryID = NewData.CategoryID;
                item.ItemType = NewData.ItemType;
                CategoryTopItem iteminfo = new CategoryTopItem();
                ModelConverter.ConvertTo<CategoryTopItemDM, CategoryTopItem>(item, iteminfo);

                iteminfo = _categoryTopItemSerAdp.SaveCategoryTopItemData(iteminfo);
                ReData.Add(iteminfo);
            }

            return ReData;
        }

        public ResponseMessage<List<CategoryTopItemDM>> SaveCategoryTopItem(TopItemDM NewData)
        {
            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            List<CategoryTopItemDM> ReData = new List<CategoryTopItemDM>();
            List<CategoryTopItem> ReInfo = new List<CategoryTopItem>();

            TWNewEgg.Models.DBModels.TWSQLDB.CategoryTopItem Info = new TWNewEgg.Models.DBModels.TWSQLDB.CategoryTopItem();
            TWNewEgg.Models.DBModels.TWSQLDB.CategoryTopItem OldInfo = new TWNewEgg.Models.DBModels.TWSQLDB.CategoryTopItem();

            ResponseMessage<List<CategoryTopItemDM>> Result = new ResponseMessage<List<CategoryTopItemDM>>();

            List<int> avaItemList = new List<int>();
            List<int> bannerItemList = new List<int>();
            List<int> noExitItem = new List<int>();

            try
            {
                bannerItemList = NewData.ItemList.Select(x => x.ItemID.GetValueOrDefault()).ToList();
                avaItemList = _adLayer3Ser.GetAvailableAndVisibleItemID(bannerItemList);
                //判斷設定的item是否為有效的item
                if (bannerItemList.Count() != avaItemList.Count())
                {
                    string strNoExit = "";
                    noExitItem = (from old in bannerItemList
                                  where !(from pro in avaItemList
                                          select pro)
                                          .Contains(old)
                                  select old).ToList();

                    for (int i = 0; i < noExitItem.Count(); i++)
                    {
                        strNoExit = strNoExit + noExitItem[i].ToString();
                        if ((i + 1) < noExitItem.Count())
                            strNoExit = strNoExit + ", ";
                    }

                    Result.IsSuccess = false;
                    Result.Message = string.Format("CategoryID：{0}，有無效的Item：{1}!!", NewData.CategoryID, strNoExit);
                    throw new Exception(Result.Message);
                }


                if (!string.IsNullOrEmpty(NewData.CategoryID.ToString()) && (NewData.CategoryID > 0) && (NewData.ItemList != null))
                {
                    ReInfo = UpdateCategoryTopItemDMData(NewData);

                    ReData = ModelConverter.ConvertTo<List<CategoryTopItemDM>>(ReInfo);

                    Result.Data = ReData;
                    Result.IsSuccess = true;
                    Result.Message = string.Format("categoryID:{0}，更新成功^^", NewData.CategoryID);
                }
                else
                {
                    Result.Data = null;
                    Result.IsSuccess = false;
                    Result.Message = string.Format("無categoryID，更新失敗!!");
                    throw new Exception(Result.Message);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                Result.Message = ex.Message;
                return Result;
            }

            return Result;
        }

    }
}
